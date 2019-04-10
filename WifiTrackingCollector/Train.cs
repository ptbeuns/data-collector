using System;
using System.Collections.Generic;
using System.Text;

namespace WifiTrackingCollector
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

        public void ReadConfigAndSetSettings()
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
