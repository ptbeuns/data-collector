using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace DataCollector
{
    class Program
    {
        static public List<string> WifiTrackers { get; private set; }
        static private string configFile = AppDomain.CurrentDomain.BaseDirectory + @"\TrainConfig.json";
        static void Main(string[] args)
        { 
            Console.WriteLine("Application startup");
            //SerialAutoDiscover ad = new SerialAutoDiscover();
            //WifiTrackers = ad.GetAvailablePortsAndAutoDiscover();
            //Temp:
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Train config file not found!");
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(0);
            }
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            //string config = @"{'RideNr':3933,'TrainUnitNr': 9596,'Coupes':[{'CoupeNr': 1, 'WiFiTrackers':[7725891, 7834854]},{ 'CoupeNr': 2, 'WiFiTrackers':[5495043, 5904544]}]}";
            //Train train = JsonConvert.DeserializeObject<Train>(config);
            Train train = JsonConvert.DeserializeObject<Train>(File.ReadAllText(configFile));
            train.AutoDiscoverWiFiTrackers();
            Console.WriteLine(train.Coupes[0].WiFiTrackers[0].TrackerID);
            Console.WriteLine(train.Coupes[0].WiFiTrackers[1].TrackerID);

            Console.ReadLine();
        }
    }
}
