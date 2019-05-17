using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollector
{
    class I2CChair
    {
        private I2CCommunication i2c;
        public int ChairSlaveId { get; private set; }

        public I2CChair(int chairSlaveId)
        {
            ChairSlaveId = chairSlaveId;
            i2c = new I2CCommunication();
            i2c.Open(ChairSlaveId);
        }

        public void ReadChairOccupation()
        {
            byte[] data = new byte[4];
            data = i2c.Read(4);
            int count = 0;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((data[i] & (0b1 << j)) != 0)
                    {
                        count++;
                    }
                }
            }
            Console.WriteLine(data[0]);
            Console.WriteLine(data[1]);
            Console.WriteLine(data[2]);
            Console.WriteLine(data[3]);
            Console.WriteLine("Ha mijn code is beter: " + count);
        }
    }
}
