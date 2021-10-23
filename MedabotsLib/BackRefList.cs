using MedabotsLib.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class BackRefList<T> : TrackingList<T>, ITypedList<T> where T : Byteable
    {
        int offset;
        public Type ListType { get; }

        public BackRefList(List<T> list, int offset, Type type)
        {
            this.original = list;
            this.offset = offset;
            this.ListType = type;
            this.Lock();
        }

        public Type GetListType()
        {
            return ListType;
        }

        public List<BackRef> ToBackRefs()
        {
            List<BackRef> result = new List<BackRef>();
            foreach (KeyValuePair<int, T> item in this.replacement)
            {
                result.Add(new BackRef(this.offset + item.Key, item.Value.ToBytes()));
            }
            return result;
        }
        
    }
}
