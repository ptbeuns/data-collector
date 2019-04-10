using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;

namespace WifiTrackingCollector
{
    class Coupe
    {
        public List<WiFiTracker> WiFiTrackers { get; private set; }
        public List<PhysicalAddress> MacAddresses { get; private set; }
        public int UniqueMacCount { get; private set; }
        public int ChairsOccupied { get; private set; }

        public Coupe()
        {
            WiFiTrackers = new List<WiFiTracker>();
            MacAddresses = new List<PhysicalAddress>();
            UniqueMacCount = 0;
            ChairsOccupied = 0;
        }

        public void CollectCrowd()
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
