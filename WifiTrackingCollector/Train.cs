using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Newtonsoft.Json;

namespace DataCollector
{
    class Train
    {
        public int RideNr { get; private set; }
        public int TrainUnitNr { get; private set; }
        public List<Coupe> Coupes { get; private set; }

        public Train(int rideNr, int trainUnitNr)
        {
            RideNr = rideNr;
            TrainUnitNr = trainUnitNr;
            Coupes = new List<Coupe>();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void AutoDiscoverWiFiTrackers()
        {
            SerialAutoDiscover autoDiscover = new SerialAutoDiscover();
            Dictionary<string, int> x = autoDiscover.GetAvailablePortsAndAutoDiscover();
            foreach (KeyValuePair<string, int> comPort in x)
            {
                foreach (Coupe coupe in Coupes)
                {
                    foreach (WiFiTracker wiFiTracker in coupe.WiFiTrackers)
                    {
                        if (wiFiTracker.TrackerID == comPort.Value)
                        {
                            Console.WriteLine(wiFiTracker.ComPort);

                            wiFiTracker.ComPort = comPort.Key;
                            Console.WriteLine(wiFiTracker.ComPort);
                        }
                    }
                }
            }
        }
    }
}
