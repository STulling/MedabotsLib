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
        bool wrote = false;

        public DataWriter(int startOffset)
        {
            this.offset = startOffset;
            this.data = new List<RefData>();
        }

        public void Add(RefData refData)
        {
            data.Add(refData);
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
