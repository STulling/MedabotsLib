using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    struct RefData
    {
        public int backref;
        public byte[] data;

        public RefData(int backref, byte[] data) : this()
        {
            this.backref = backref;
            this.data = data;
        }
    }
}
