using MedabotsLib.Utils;
using System.Collections.Generic;

namespace MedabotsLib.DataStructures
{
    public class SequentialBackRefList<T> : BackRefList<T> where T : IByteable
    {
        int offset;
        public SequentialBackRefList(List<T> list, int offset) : base(list)
        {
            this.offset = offset;
        }

        public override List<BackRef> ToBackRefs()
        {
            List<BackRef> result = new List<BackRef>();
            foreach (KeyValuePair<int, T> item in this.changes)
            {
                result.Add(new BackRef(this.offset + item.Key * 4, item.Value.ToBytes()));
            }
            return result;
        }

        public RandomAccessBackRefList<T> ToRandomAccess()
        {
            RandomAccessBackRefList<T> ra = new RandomAccessBackRefList<T>();

            for (int i = 0; i < this.Count; i++)
            {
                ra.Add(this[i], this.offset + i * 4);
            }

            return ra;
        }
    }
}
