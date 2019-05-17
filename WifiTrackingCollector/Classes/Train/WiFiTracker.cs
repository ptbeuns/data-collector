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
        private string data;

        public WiFiTracker(int trackerID)
        {
            TrackerID = trackerID;
            ComPort = null;
            Serial = null;

            serialState = SerialState.Idle;
            countMacsForReceive = 0;
            macList = new List<PhysicalAddress>();
            data = "";
        }

        public void StartSerialConnection()
        {
            if (ComPort != null)
            {
                Serial = new SerialPortStream(ComPort, 115200, 8, Parity.None, StopBits.One)
                {
                    StopBits = StopBits.One,
                    WriteTimeout = 10000,
                    ReadTimeout = 10000
                };
                Serial.DataReceived += SerialPortDataReceived;
                Serial.Open();
                //Werkt iets niet 
            }
        }

        public List<PhysicalAddress> Collect()
        {
            macList.Clear();
            StartSerialConnection();
            Serial.WriteLine(SerialMessageParser.Encode("DISCOVERY"));
            Console.WriteLine("1");
            serialState = SerialState.Initialize;
            while (serialState == SerialState.Initialize)
            {
                
            }
            System.Threading.Thread.Sleep(1000);
            Serial.Write(SerialMessageParser.Encode("COLLECT"));
            Console.WriteLine("2");
            serialState = SerialState.Collecting;
            while (serialState == SerialState.Collecting || serialState == SerialState.Receiving)
            {
                
            }
            Console.WriteLine("99");
            Serial.Close();
            Serial.Dispose();
            return macList;
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("3");
            data = SerialMessageParser.Parse(Serial.ReadExisting());
            Console.WriteLine(data);
            Console.WriteLine(ComPort);
            Console.WriteLine(serialState);
            if (data.Contains("HEARTBEAT"))
            {
                Serial.Write(SerialMessageParser.Encode("ACK"));
                data = "";
            }
            switch (serialState)
            {
                case SerialState.Idle:
                    break;
                case SerialState.Initialize:
                    Console.WriteLine("6");
                    if (data.StartsWith("NodeMcu"))
                    {
                        Console.WriteLine("7");
                        Serial.Write(SerialMessageParser.Encode("ACK"));
                        Console.WriteLine("Nodemcu ack");
                        serialState = SerialState.Idle;
                    }
                    else if (data.Contains("HEARTBEAT"))
                    {
                        Console.WriteLine("8");
                        Serial.Write(SerialMessageParser.Encode("ACK"));
                        Serial.Write(SerialMessageParser.Encode("COLLECT"));
                        serialState = SerialState.Collecting;
                    }
                    break;
                case SerialState.Collecting:
                    Console.WriteLine("HALLO YEET HIER BEN IK");
                    Console.WriteLine(data);
                    Console.WriteLine("9");
                    if (data.StartsWith("DONE:"))
                    {
                        Console.WriteLine("10");
                        Console.WriteLine("gedont");
                        Int32.TryParse(SerialMessageParser.GetValue(data), out countMacsForReceive);
                        Serial.Write(SerialMessageParser.Encode("ACK"));
                        Serial.Write(SerialMessageParser.Encode("SEND"));
                        serialState = SerialState.Receiving;
                    }
                    break;
                case SerialState.Receiving:
                    Console.WriteLine("11");
                    if (data.Contains("HEARTBEAT"))
                    {
                        Console.WriteLine("12");
                        Serial.Write(SerialMessageParser.Encode("ACK"));
                        Serial.Write(SerialMessageParser.Encode("SEND"));
                    }
                    Console.WriteLine("Receiving");
                    if (data == "ACK")
                    {
                        Console.WriteLine("13");
                        Serial.Write(SerialMessageParser.Encode("ACK"));
                    }
                    if (data.StartsWith("MAC:"))
                    {
                        try
                        {
                            //data = data.Substring(4);
                            //data = data.Replace(':', '-').ToUpper();
                            macList.Add(PhysicalAddress.Parse(data.Substring(4).Replace(':', '-').ToUpper()));
                            Serial.Write(SerialMessageParser.Encode("ACK"));
                        }
                        catch (FormatException)
                        {
                            Serial.Write(SerialMessageParser.Encode("NACK"));
                        }
                    }
                    else if (data == "FINISH")
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
