using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Data
{
    public abstract class BaseWrapper<T> : IByteable where T : IByteable
    {
        protected T data;
        protected int id;

        protected BaseWrapper(int id, T data)
        {
            this.id = id;
            this.data = data;
        }

        public byte[] ToBytes() => data.ToBytes();
    }
}
