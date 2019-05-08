using System;
//using System.IO.Ports;
using RJCP.IO.Ports;
using System.Collections.Generic;

namespace DataCollector
{
    class SerialInformation
    {
        SerialPortStream serialPort;

        public void ReadFromPort(string port)
        {
            serialPort = new SerialPortStream(port, 115200, 8, Parity.None, StopBits.One);
            serialPort.DataReceived += SerialPortDataReceived;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    serialPort.Open();
                    i = 4;
                    Console.WriteLine("Serialopen");
                    serialPort.Write("#COLLECT$");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Unauthorized port or port already in use!");
                    i++;
                }
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            try
            {
                if(data.Contains("ACK"))
                {
                    data = data.Substring(5);
                }
                data = data.Substring(data.IndexOf('#') + 1, data.IndexOf('$') - data.IndexOf('#') - 1);
            }
            catch (Exception)
            {
                // throw;
            }

            if (data.StartsWith("DONE:"))
            {
                serialPort.Write("#SEND$");
            }
            else if (data.StartsWith("MAC:"))
            {
                serialPort.WriteLine("#ACK$");
            }
            Console.WriteLine(data);
        }
    }
}
