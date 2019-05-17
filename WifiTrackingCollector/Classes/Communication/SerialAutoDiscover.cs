using System;
using System.Collections.Generic;
using System.Diagnostics;
using RJCP.IO.Ports;

namespace DataCollector
{
    class SerialAutoDiscover
    {
        private Dictionary<string, int> trackers;
        private SerialPortStream autoDiscoverPort;
        private Stopwatch stopWatch;


        public Dictionary<string, int> GetAvailablePortsAndAutoDiscover()
        {
            trackers = new Dictionary<string, int>();
            foreach (string portName in SerialPortStream.GetPortNames())
            {
                Console.WriteLine("Autodiscover port:");
                Console.WriteLine(portName);
                try
                {
                    autoDiscoverPort = new SerialPortStream(portName, 115200, 8, Parity.None, StopBits.One)
                    {
                        StopBits = StopBits.One,
                        WriteTimeout = 1000,
                        ReadTimeout = 3000
                    };
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    autoDiscoverPort.DataReceived += SerialPortAutoDiscover;
                    autoDiscoverPort.Open();
                    autoDiscoverPort.WriteLine(SerialMessageParser.Encode("DISCOVERY"));
                }
                catch (UnauthorizedAccessException)
                {
                    autoDiscoverPort.Close();
                }
                catch (System.IO.IOException)
                {
                    autoDiscoverPort.Close();
                }
                catch (TimeoutException)
                {
                    autoDiscoverPort.Close();
                }

                while (autoDiscoverPort.IsOpen)
                {
                    if (stopWatch.ElapsedMilliseconds >= 3000)
                    {
                        autoDiscoverPort.Close();
                    }
                }
                autoDiscoverPort.Dispose();
            }
            return trackers;
        }

        private void SerialPortAutoDiscover(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("AutoDiscover readline");
            Console.WriteLine(autoDiscoverPort);
            int nodeid = 0;
            //TODO try catch serial disconnected/notfound
            string data = autoDiscoverPort.ReadExisting();
            if (SerialMessageParser.Parse(data).StartsWith("NodeMcu"))
            {
                data = SerialMessageParser.Parse(data);
                int.TryParse(SerialMessageParser.GetValue(data), out nodeid);
                Console.WriteLine(data);
            }

            if (nodeid != 0)
            {
                Console.WriteLine("NodeMCU found");
                autoDiscoverPort.Write(SerialMessageParser.Encode("ACK"));
                trackers.Add(autoDiscoverPort.PortName, nodeid);
                autoDiscoverPort.Close();
            }
            Console.WriteLine("yeet");
        }
    }
}
