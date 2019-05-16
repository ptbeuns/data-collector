using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Linq;

namespace DataCollector
{
    class Coupe
    {
        public int CoupeNr { get; private set; }
        public List<WiFiTracker> WiFiTrackers { get; private set; }
        public List<PhysicalAddress> MacAddresses { get; private set; }
        public int UniqueMacCount { get; private set; }
        public int ChairsOccupied { get; private set; }

        public Coupe(int coupeNr, int[] wiFiTrackers)
        {
            CoupeNr = coupeNr;
            WiFiTrackers = new List<WiFiTracker>();
            MacAddresses = new List<PhysicalAddress>();
            foreach (int tracker in wiFiTrackers)
            {
                Console.WriteLine(tracker);
                WiFiTrackers.Add(new WiFiTracker(tracker));
            }
            UniqueMacCount = 0;
            ChairsOccupied = 0;
        }

        public void CollectCrowd()
        {
            UniqueMacCount = 0;
            ChairsOccupied = 0;
            MacAddresses.Clear();
            foreach (WiFiTracker wiFiTracker in WiFiTrackers)
            {
                if (wiFiTracker.ComPort != null && wiFiTracker.ComPort != "")
                {
                    MacAddresses.AddRange(wiFiTracker.Collect());
                }
            }
            UniqueMacCount = (from x in MacAddresses select x).Distinct().Count();
            //foreach (PhysicalAddress mac in MacAddresses)
            //{
            //    Console.WriteLine(mac.ToString());
            //}
            //Foreach I2C slave (stoelensysteem)
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
