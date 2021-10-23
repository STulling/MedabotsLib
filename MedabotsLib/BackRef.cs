using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public struct BackRef
    {
        public int backref;
        public byte[] data;

        public BackRef(int backref, byte[] data) : this()
        {
            this.backref = backref;
            this.data = data;
        }
    }
}
