using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public struct BackRef
    {
        public int[] backrefs;
        public byte[] data;

        public BackRef(int[] backrefs, byte[] data) : this()
        {
            this.backrefs = backrefs;
            this.data = data;
        }
    }
}
