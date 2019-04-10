using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace WifiTrackingCollector
{
    public class WiFiTracker
    {
        public int TrackerID { get; private set; }
        public int CoupeNr { get; private set; }
        public string ComPort { get; set; }
        public SerialPort Serial { get; set; }

        public WiFiTracker(int trackerID, int coupeNr)
        {
            TrackerID = trackerID;
            CoupeNr = coupeNr;
            ComPort = "";
            Serial = null;
        }

        public void Collect()
        {l
           
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
