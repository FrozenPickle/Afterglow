using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Plugins;
using System.IO.Ports;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Management;
using System.Xml.Serialization;
using System.ComponentModel.Composition;

namespace Afterglow.Plugins.Output
{
    /// <summary>
    /// Arduino Output
    /// </summary>
    [DataContract]
    [Export(typeof(IOutputPlugin))]
    public class ArduinoOutput: BasePlugin, IOutputPlugin, IDisposable
    {
        private SerialPort _port;
        private byte[] _serialData;
        private bool _running = false;
        object runLock = new object();
        
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [DataMember]
        public override string Name
        {
            get { return "Arduino Output"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        [DataMember]
        public override string Description
        {
            get { return "Output to the Afterglow Arduino 1.0 application"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [DataMember]
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }
        /// <summary>
        /// A website for further information
        /// </summary>
        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 1); }
        }
        #endregion

        #region Properties
        [DataMember]
        [Required]
        [Display(Name = "Serial Port", Order = 100)]
        [ConfigLookup(RetrieveValuesFrom = "Ports")]
        public string Port
        {
            get { return Get(() => Port, () => GetDefaultPort()); }
            set { Set(() => Port, value); }
        }

        /// <summary>
        /// Gets the first port found
        /// </summary>
        /// <returns>Port Name</returns>
        public string GetDefaultPort()
        {
            LookupItemString[] ports = this.Ports;

            if (ports.Any())
            {
                string id = (from p in ports
                             where p.Name.Contains("Arduino")
                             select p.Id).FirstOrDefault();
                if (string.IsNullOrEmpty(id))
                {
                    return ports.FirstOrDefault().Id;
                }
                else
                {
                    return id;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the available Serial/USB ports that
        /// If none are found it is possible the driver is not installed
        /// </summary>
        [XmlIgnore]
        public LookupItemString[] Ports
        {
            get
            {
                string[] portNames = SerialPort.GetPortNames();

                try
                {
                    if (portNames.Any())
                    {
                        using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
                        {
                            var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                            return (from n in portNames
                                    join p in ports on n equals p["DeviceID"].ToString()
                                    select new LookupItemString()
                                    {
                                        Id = n,
                                        Name = n + " - " + p["Caption"]
                                    }).ToArray();
                        }
                    }
                    else
                    {
                        return new LookupItemString[0];
                    }
                }
                catch (Exception)
                {
                    return (from p in portNames
                            select new LookupItemString()
                            {
                                Id = p,
                                Name = p
                            }).ToArray();
                }

            }
        }

        [DataMember]
        [Required]
        [Display(Name = "Baud Rate", Order = 200)]
        [Range(0, 999999)]
        public int BaudRate
        {
            get { return Get(() => BaudRate, () => 115200); }
            set { Set(() => BaudRate, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Magic Word", 
            Description = "This is a special value used to communicate with the arduino",
            Order = 300)]
        [StringLength(3)]
        public string MagicWord
        {
            get { return Get(() => MagicWord, () => "Glo"); }
            set { Set(() => MagicWord, value); }
        }
        #endregion

        /// <summary>
        /// Start this Plugin
        /// </summary>
        public override void Start()
        {
            TryStart();
        }

        public bool TryStart()
        {
            Stop();
            lock (runLock)
            {
                if (Ports.Length == 0)
                {
                    AfterglowRuntime.Logger.Error("No serial ports found");
                    _port = null;
                }
                else
                {
                    string[] portNames = SerialPort.GetPortNames();
                    if (!(from p in portNames
                          where p == this.Port
                          select p).Any())
                    {
                        AfterglowRuntime.Logger.Error(string.Format("Configured Port {0} was not found, please check the settings and connected devices", this.Port));
                    }

                    _port = new SerialPort(Port, BaudRate);
                    _port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
                    try
                    {
                        if (!_port.IsOpen)
                        {
                            _port.Open();
                            _running = true;
                        }
                        else
                        {
                            AfterglowRuntime.Logger.Error("Another process or application is using the port:{0}", this.Port);
                        }
                    }
                    catch (IOException e)
                    {
                        //TODO: give different error messages based on different exceptions
                        AfterglowRuntime.Logger.Error(string.Format("Error connecting to the Arduino. Exception: {0}", e));
                    }
                }
            }
            if (!_running)
            {
                //Call stop to try to clear the port for retry
                Stop();
            }

            return _running;
        }

        /// <summary>
        /// On COM error restart so user does not notice issue
        /// </summary>
        void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            AfterglowRuntime.Logger.Warn("Arduino Output - Error Received - Attempting restart. {0}", e);
            
            Start();
            if (!_running)
            {
                AfterglowRuntime.Logger.Error("Could not restart after error");
            }
        }

        /// <summary>
        /// Send the light information to the arduino
        /// </summary>
        /// <param name="lights"></param>
        public void Output(List<Core.Light> lights, LightData data)
        {
            if (_port != null && _port.IsOpen)
            {
                if (_serialData == null || _serialData.Length != lights.Count)
                {
                    _serialData = new byte[6 + lights.Count * 3];

                    _serialData[0] = Convert.ToByte(this.MagicWord.ToCharArray(0, 1)[0]); // Magic word
                    _serialData[1] = Convert.ToByte(this.MagicWord.ToCharArray(1, 1)[0]);
                    _serialData[2] = Convert.ToByte(this.MagicWord.ToCharArray(2, 1)[0]);
                    _serialData[3] = (byte)((lights.Count) >> 8); // LED count high byte
                    _serialData[4] = (byte)((lights.Count) & 0xff); // LED count low byte
                    _serialData[5] = (byte)(_serialData[3] ^ _serialData[4] ^ 0x55); // Checksum
                }

                int serialDataPos = 6;
                // Fast copy of data to serial buffer
                Buffer.BlockCopy(data.ColourData, 0, _serialData, serialDataPos, data.ColourData.Length);

                // Issue data to Arduino
                try
                {
                    if (_port != null)
                    {
                        _port.Write(_serialData, 0, _serialData.Length);
                    }
                }
                catch (Exception ex)
                {
                    AfterglowRuntime.Logger.Error(ex, "Arduino Output - Output - Write to port error");
                }
            }
            else
            {
                Start();
            }
        }

        /// <summary>
        /// Stop this Plugin
        /// </summary>
        public override void Stop()
        {
            lock (runLock)
            {
                if (_port != null)
                {
                    _port.Close();
                    _port.Dispose();
                    _port = null;
                }
                _running = false;
            }
        }

        /// <summary>
        /// Dispose so COM port is closed correctly
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }
    }
}
