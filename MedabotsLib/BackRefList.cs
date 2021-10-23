using MedabotsLib.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class BackRefList<T> : TrackingList<T> where T : IByteable
    {
        int offset;

        public BackRefList(List<T> list, int offset)
        {
            this.original = list;
            this.offset = offset;
        }

        public List<RefData> ToBackRefs()
        {
            List<RefData> result = new List<RefData>();
            foreach (KeyValuePair<int, T> item in this.replacement)
            {
                result.Add(new RefData(this.offset + item.Key, item.Value.ToBytes()));
            }
            return result;
        }
        
    }
}
