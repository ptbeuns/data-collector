﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataCollector
{
    class SerialAutoDiscover
    {
        private Dictionary<string, int> trackers;
        private SerialPort autoDiscoverPort;
        private Stopwatch stopWatch;


        public Dictionary<string, int> GetAvailablePortsAndAutoDiscover()
        {
            trackers = new Dictionary<string, int>();
            foreach (string portName in SerialPort.GetPortNames())
            {
                try
                {
                    autoDiscoverPort = new SerialPort(portName, 115200, Parity.None, 8)
                    {
                        StopBits = StopBits.One,
                        WriteTimeout = 1000,
                        ReadTimeout = 3000
                    };
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    autoDiscoverPort.DataReceived += SerialPortAutoDiscover;
                    autoDiscoverPort.Open();
                    autoDiscoverPort.WriteLine("#DISCOVERY$");
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
            }
            return trackers;
        }

        private void SerialPortAutoDiscover(object sender, SerialDataReceivedEventArgs e)
        {
            int nodeid = 0;
            //try catch serial disconnected/notfound
            string data = autoDiscoverPort.ReadExisting();
            if (data.Contains("#NodeMcu:") && data.Contains('$'))
            {
                data = data.Substring(data.IndexOf(':') + 1, data.IndexOf('$') - data.IndexOf(':') - 1);
                int.TryParse(data, out nodeid);
                Console.WriteLine(data);
            }

            if (nodeid != 0)
            {
                Console.WriteLine("nodemcu");
                autoDiscoverPort.Write("#ACK$");
                trackers.Add(autoDiscoverPort.PortName, nodeid);
                autoDiscoverPort.Close();
            }
        }
    }
}