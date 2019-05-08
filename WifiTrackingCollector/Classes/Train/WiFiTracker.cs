using System;
using System.Collections.Generic;
using System.Text;
//using System.IO.Ports;
using RJCP.IO.Ports;

namespace DataCollector
{
    public class WiFiTracker
    {
        public int TrackerID { get; private set; }
        public string ComPort { get; set; }
        public SerialPortStream Serial { get; private set; }
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
                Serial = new SerialPortStream(ComPort, 115200, 8, Parity.None, StopBits.One);
                Serial.DataReceived += SerialPortDataReceived;
                Serial.Open();
            }
        }

        public void Collect()
        {
            Serial.Write("#COLLECT$");
            serialState = SerialState.Collecting;
            while (serialState == SerialState.Collecting)
            {
                //Nothing :(
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = Serial.ReadExisting();
            data = SerialMessageParser.Parse(data);
            if (data.Contains('#') && data.Contains('$'))
            {
                if (data.Contains("#HEARTBEAT$"))
                {
                    Serial.Write(SerialMessageParser.Encode("ACK"));
                }
            }

            //try
            //{
            //    if (data.Contains("ACK"))
            //    {
            //        data = data.Substring(5);
            //    }
            //    data = data.Substring(data.IndexOf('#') + 1, data.IndexOf('$') - data.IndexOf('#') - 1);
            //}
            //catch (Exception)
            //{
            //    // throw;
            //}
            
            switch (serialState)
            {
                case SerialState.Idle:
                    break;
                case SerialState.Collecting:
                    //Wait for done message
                    //Split and save count
                    

                    break;
                case SerialState.Receiving:
                    //Wait for #MAC:xx:xx:xx:xx:xx$ and ACK
                    //If finish, check count
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
