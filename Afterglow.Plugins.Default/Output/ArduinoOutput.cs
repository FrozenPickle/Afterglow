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
using Afterglow.Core.Storage;
using Afterglow.Core;
using System.IO;

namespace Afterglow.Plugins.Output
{
    public class ArduinoOutput: BasePlugin, IOutputPlugin
    {
        SerialPort _port;
        byte[] _serialData;

        public ArduinoOutput()
        {
        }

        public ArduinoOutput(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties
        public override string Name
        {
            get { return "Arduino Output"; }
        }

        public override string Description
        {
            get { return "Output to the Afterglow Arduino 1.0 application"; }
        }

        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        [ConfigLookup(DisplayName = "Serial Port", RetrieveValuesFrom = "Ports")]
        public string Port
        {
            get { return Get(() => Port, () => Ports[0]); }
            set { Set(() => Port, value); }
        }

        public string[] Ports
        {
            get { return SerialPort.GetPortNames(); }
        }

        [ConfigNumber(DisplayName = "Baud Rate", Min = 0, Max = 999999)]
        public int? BaudRate
        {
            get { return Get(() => BaudRate, () => 115200); }
            set { Set(() => BaudRate, value); }
        }

        [ConfigString(DisplayName = "Magic Word", MaxLength = 3, Description = "This is a special value used to communicate with the arduino")]
        public string MagicWord
        {
            get { return Get(() => MagicWord, () => "Glo"); }
            set { Set(() => MagicWord, value); }
        }

        public override void Start()
        {
            //TODO: error checking and configuration of port
            
            if (Ports.Length == 0)
            {
                Logger.Warn("No serial ports found");
            }
            else
            {
                _port = new SerialPort(Port, BaudRate.Value);
                try
                {
                    _port.Open();
                }
                catch (IOException ex)
                {
                    Logger.Error(ex, "Arduino not found");
                }
            }
        }

        public void Output(List<Core.Light> leds)
        {
            if (_serialData == null || _serialData.Length != leds.Count)
            {
                _serialData = new byte[6 + leds.Count * 3];

                _serialData[0] = Convert.ToByte('G'); // Magic word
                _serialData[1] = Convert.ToByte('l');
                _serialData[2] = Convert.ToByte('o');
                _serialData[3] = (byte)((leds.Count) >> 8); // LED count high byte
                _serialData[4] = (byte)((leds.Count) & 0xff); // LED count low byte
                _serialData[5] = (byte) (_serialData[3] ^ _serialData[4] ^ 0x55); // Checksum
            }

            int serialDataPos = 6;
            foreach (var led in leds)
            {
                _serialData[serialDataPos++] = Convert.ToByte(led.LEDColour.R);
                _serialData[serialDataPos++] = Convert.ToByte(led.LEDColour.G);
                _serialData[serialDataPos++] = Convert.ToByte(led.LEDColour.B);
            }

            // Issue data to Arduino
            try
            {
                if (_port != null) _port.Write(_serialData, 0, _serialData.Length);
            }
            catch (Exception)
            {
                Logger.Warn("Adruino not found");
            }
        }

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
