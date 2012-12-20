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

namespace Afterglow.Plugins.Output
{
    /// <summary>
    /// Arduino Output
    /// </summary>
    public class ArduinoOutput: BasePlugin, IOutputPlugin
    {
        private SerialPort _port;
        private byte[] _serialData;
        
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        public override string Name
        {
            get { return "Arduino Output"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        public override string Description
        {
            get { return "Output to the Afterglow Arduino 1.0 application"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }
        /// <summary>
        /// A website for further information
        /// </summary>
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 0, 1); }
        }
        #endregion

        [Required]
        [Display(Name = "Serial Port", Order = 100)]
        [ConfigLookup(RetrieveValuesFrom = "Ports")]
        public string Port
        {
            get { return Get(() => Port, () => Ports[0]); }
            set { Set(() => Port, value); }
        }

        /// <summary>
        /// Gets the available Serial/USB ports that
        /// If none are found it is possible the driver is not installed
        /// </summary>
        public string[] Ports
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        [Required]
        [Display(Name = "Baud Rate", Order = 200)]
        [Range(0, 999999)]
        public int BaudRate
        {
            get { return Get(() => BaudRate, () => 115200); }
            set { Set(() => BaudRate, value); }
        }

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

        /// <summary>
        /// Start this Plugin
        /// </summary>
        public override void Start()
        {
            //TODO: error checking and configuration of port
            //TODO: possibly check device manager and see if Arduino is founds just not installed
            if (Ports.Length == 0)
            {
                //Logger.Warn("No serial ports found");
                throw new Exception("No serial ports found");
            }
            else
            {
                _port = new SerialPort(Port, BaudRate);
                _port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
                try
                {
                    _port.Open();
                }
                catch (IOException e)
                {
                    //TODO: try and reset the serial connection so the user does not need to disconnect and reattach the cable
                    string message = "Please un plug and re attach the cable";
                    
                    throw new Exception(message, e);
                    
                    //Logger.Error(ex, "Arduino not found");
                }
            }
        }

        void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Send the light information to the arduino
        /// </summary>
        /// <param name="lights"></param>
        public void Output(List<Core.Light> lights)
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
                foreach (var led in lights.OrderBy(l => l.Index))
                {
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.R);
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.G);
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.B);
                }

                // Issue data to Arduino
                try
                {
                    if (_port != null) _port.Write(_serialData, 0, _serialData.Length);
                }
                catch (Exception)
                {
                    //Logger.Warn("Adruino not found");
                }
            }
            else
            {
                Stop();
                Start();
            }
        }

        /// <summary>
        /// Stop this Plugin
        /// </summary>
        public override void Stop()
        {
            if (_port != null)
            {
                _port.Close();
                _port.Dispose();
                _port = null;
            }
        }
    }
}
