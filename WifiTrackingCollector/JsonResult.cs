using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollector
{
    class JsonResult
    {
        public int RideNr { get; set; }
        public int TrainUnitNr { get; set; }
        public List<Coupe> Coupes { get; set; }
        public int MyProperty { get; set; }
    }
}
