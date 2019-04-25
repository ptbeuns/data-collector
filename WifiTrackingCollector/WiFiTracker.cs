using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace DataCollector
{
    public class WiFiTracker
    {
        public int TrackerID { get; private set; }
        public string ComPort { get; set; }
        public SerialPort Serial { get; private set; }
        private SerialState serialState;

        public WiFiTracker(int trackerID)
        {
            TrackerID = trackerID;
            ComPort = null;
            Serial = null;
        }

        public void StartSerialConnection()
        {
            if(ComPort != null)
            {
                Serial = new SerialPort(ComPort, 115200, Parity.None, 8);
                Serial.DataReceived += SerialPortDataReceived;
                Serial.Open();
            }
        }

        public void Collect()
        {
            Serial.Write("#COLLECT$");
            serialState = SerialState.Collecting;
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = Serial.ReadExisting();
            if (data.Contains('#') && data.Contains('$'))
            {

            }

            try
            {
                if (data.Contains("ACK"))
                {
                    data = data.Substring(5);
                }
                data = data.Substring(data.IndexOf('#') + 1, data.IndexOf('$') - data.IndexOf('#') - 1);
            }
            catch (Exception)
            {
                // throw;
            }

            switch (serialState)
            {
                case SerialState.Idle:
                    break;
                case SerialState.Collecting:
                    break;
                case SerialState.Receiving:
                    break;
                default:
                    break;
            }
            

            if (data.StartsWith("DONE:"))
            {
                Serial.Write("#SEND$");
            }
            else if (data.StartsWith("MAC:"))
            {
                Serial.WriteLine("#ACK$");
            }
            Console.WriteLine(data);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
