using GBALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    class DataWriter
    {
        int offset;
        List<RefData> data;

        public DataWriter(int startOffset)
        {
            this.offset = startOffset;
            this.data = new List<RefData>();
        }

        public void Add(RefData refData)
        {
            data.Add(refData);
        }

        public void Add(int refAddress, byte[] stuff)
        {
            data.Add(new RefData(refAddress, stuff));
        }

        public void Write()
        {
            foreach (RefData refData in data) 
            {
                Game.GetInstance().Write(this.offset, refData.data);
                Game.GetInstance().Write(refData.backref, this.offset);
                this.offset += refData.data.Length;
            }
        }
    }
}
