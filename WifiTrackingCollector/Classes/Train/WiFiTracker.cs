using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using RJCP.IO.Ports;

namespace DataCollector
{
    public class WiFiTracker
    {
        public int TrackerID { get; private set; }
        public string ComPort { get; set; }
        public SerialPortStream Serial { get; private set; }
        private SerialState serialState;
        private int countMacsForReceive;
        private List<PhysicalAddress> macList;

        public WiFiTracker(int trackerID)
        {
            TrackerID = trackerID;
            ComPort = null;
            Serial = null;

            serialState = SerialState.Idle;
            countMacsForReceive = 0;
            macList = new List<PhysicalAddress>();
        }

        public void StartSerialConnection()
        {
            if (ComPort != null)
            {
                Serial = new SerialPortStream(ComPort, 115200, 8, Parity.None, StopBits.One)
                {
                    StopBits = StopBits.One,
                    WriteTimeout = 1000,
                    ReadTimeout = 3000
                };
                Serial.DataReceived += SerialPortDataReceived;
                Serial.Open();
                Serial.WriteLine(SerialMessageParser.Encode("DISCOVERY"));
                //Werkt iets niet 
            }
        }

        public List<PhysicalAddress> Collect()
        {
            //Check if serial isopen
            Serial.Write(SerialMessageParser.Encode("COLLECT"));
            serialState = SerialState.Collecting;
            while (serialState == SerialState.Collecting || serialState == SerialState.Receiving)
            {
                Console.WriteLine(Serial.IsOpen);
            }
            return macList;
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = Serial.ReadExisting();
            data = SerialMessageParser.Parse(data);
            if (data == "HARTBEAT")
            {
                Serial.Write(SerialMessageParser.Encode("ACK"));
            }
            else if (data.StartsWith("NodeMcu"))
            {
                SerialMessageParser.Encode("ACK");
            }

            switch (serialState)
            {
                case SerialState.Idle:
                    break;
                case SerialState.Collecting:
                    Console.WriteLine("Collecting");
                    if (data.StartsWith("DONE:"))
                    {
                        Int32.TryParse(SerialMessageParser.GetValue(data), out countMacsForReceive);
                        Serial.Write(SerialMessageParser.Encode("SEND"));
                        serialState = SerialState.Receiving;
                    }
                    break;
                case SerialState.Receiving:
                    Console.WriteLine("Receiving");
                    if (data.StartsWith("MAC:"))
                    {
                        try
                        {
                            macList.Add(PhysicalAddress.Parse(SerialMessageParser.GetValue(data)));
                            Serial.Write(SerialMessageParser.Encode("ACK"));
                        }
                        catch (FormatException)
                        {
                            Serial.Write(SerialMessageParser.Encode("NACK"));
                        }
                    }
                    else if (data == "FINSIH")
                    {
                        //TODO check macList.Count == countMacsForReceive (not in WiFiTracker yet)
                        serialState = SerialState.Idle;
                    }
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
