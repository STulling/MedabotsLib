using MedabotsLib.Utils;
using System;
using System.Collections.Generic;

namespace MedabotsLib.DataStructures
{
    public class RandomAccessBackRefList<T> : BackRefList<T> where T : IByteable
    {
        List<int> refs;
        public RandomAccessBackRefList() : base()
        {
            this.refs = new List<int>();
        }

        public new void Add(T item)
        {
            throw new Exception("Use the Add(item, address) method.");
        }

        public void Add(T item, int address)
        {
            if (locked)
            {
                throw new Exception("Cannot add to locked list.");
            }
            base.Add(item);
            refs.Add(address);
        }

        public void Merge(RandomAccessBackRefList<T> other)
        {
            if (this.locked || other.locked)
            {
                throw new Exception("Cannot merge joined lists.");
            }

            for (int i = 0; i < other.Count; i++)
            {
                this.Add(other[i], other.refs[i]);
            }

        }

        public override List<BackRef> ToBackRefs()
        {
            if (this.Count != refs.Count) throw new Exception("Lists don't line up, something went wrong");

            List<BackRef> result = new List<BackRef>();
            foreach (KeyValuePair<int, T> item in this.changes)
            {
                result.Add(new BackRef(refs[item.Key], item.Value.ToBytes()));
            }
            return result;
        }
    }
}
