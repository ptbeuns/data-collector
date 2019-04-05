using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace WifiTrackingCollector
{
    class SerialAutoDiscover
    {
        List<string> ports;
        SerialPort autoDiscoverPort;

        public List<string> GetAvailablePortsAndSetAutoDiscover()
        {
            ports = new List<string>();
            foreach (string portName in SerialPort.GetPortNames())
            {
                autoDiscoverPort = new SerialPort(portName, 115200, Parity.None, 8);
                try
                {
                    autoDiscoverPort.StopBits = StopBits.One;
                    autoDiscoverPort.DataReceived += SerialPortAutoDiscover;
                    autoDiscoverPort.Open();
                    autoDiscoverPort.WriteLine("#DISCOVERY$");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Unauthorized port or port already in use.");
                }
                while (autoDiscoverPort.IsOpen)
                {
                    //Nothing
                }
            }
            return ports;
        }

        private void SerialPortAutoDiscover(object sender, SerialDataReceivedEventArgs e)
        {
            //SerialPort currentPort = (SerialPort)sender;
            string data = autoDiscoverPort.ReadExisting();
            //System.Threading.Thread.Sleep(5000);
            try
            {
                //data = data.Substring(data.IndexOf("\r\n"));
                data = data.Substring(data.IndexOf('#') + 1, data.IndexOf('$') - data.IndexOf('#') - 1);
            }
            catch (Exception)
            {

               // throw;
            }

            if (data == "NodeMcu")
            {
                Console.WriteLine("nodemcu");
                autoDiscoverPort.Write("#ACK$");
                ports.Add(autoDiscoverPort.PortName);
                System.Threading.Thread.Sleep(500);
                autoDiscoverPort.Close();
                autoDiscoverPort.Dispose();
            }
        }
    }
}
