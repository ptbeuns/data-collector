using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WifiTrackingCollector
{
    class Program
    {
        static public List<string> WifiTrackers { get; private set; }

        static void Main(string[] args)
        { 
            Console.WriteLine("Application startup");
            SerialAutoDiscover ad = new SerialAutoDiscover();
            WifiTrackers = ad.GetAvailablePortsAndSetAutoDiscover();
            foreach (string ComPort in WifiTrackers)
            {
                SerialInformation serialInformation = new SerialInformation();
                serialInformation.ReadFromPort(ComPort);
            }
            Console.ReadLine();
        }
    }
}
