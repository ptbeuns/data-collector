using System;
using System.Collections.Generic;
using System.Text;

namespace WifiTrackingCollector
{
    class WiFiTracker
    {
        public int TrackerID { get; private set; }
        public int CoupeNr { get; private set; }
        public int ComPort { get; private set; }

        public WiFiTracker(int trackerID, int coupeNr)
        {

        }

        public void Collect()
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
