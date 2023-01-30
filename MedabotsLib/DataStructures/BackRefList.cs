using MedabotsLib.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedabotsLib.Utils;

namespace MedabotsLib.DataStructures
{
    /// <summary>
    /// A list of binary structs that refer to specific offsets in the ROM
    /// </summary>
    public abstract class BackRefList<T> : TrackingList<T> where T : IByteable
    {

        public BackRefList(List<T> list) : base(list)
        {
        }

        public BackRefList() : base()
        {}

        public void Verify()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] is ICanGetDirty)
                {
                    ICanGetDirty testobj = this[i] as ICanGetDirty;
                    if (testobj.IsDirty)
                    {
                        this.changes.Add(i, this[i]);
                    }
                }
            }
        }

        public abstract List<BackRef> ToBackRefs();
        
    }
}
