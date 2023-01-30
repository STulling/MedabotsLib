using MedabotsLib.Utils;
using System;

namespace MedabotsLib.GameData.Wrappers
{
    /// <summary>
    /// A wrappers adds custom functionality to a binary data struct.
    /// </summary>
    public abstract class BaseWrapper<T> : IByteable where T : IByteable
    {
        protected T data;
        protected int id;
        protected Type wrappedType;

        protected BaseWrapper(int id, T data)
        {
            this.id = id;
            this.data = data;
            this.wrappedType = typeof(T);
        }

        public byte[] ToBytes() => data.ToBytes();
    }
}
