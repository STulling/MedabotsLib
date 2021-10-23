using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Data
{
    public interface IByteable
    {
        public byte[] ToBytes();
    }
}
