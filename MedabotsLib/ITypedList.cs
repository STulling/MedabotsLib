using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public interface ITypedList<T> : IList<T>
    {
        public Type ListType { get; }
    }
}
