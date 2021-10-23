using GBALib;
using MedabotsLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    class DataWriter
    {
        int offset;

        public DataWriter(int startOffset)
        {
            this.offset = startOffset;
        }

        public void Write(List<IList<IByteable>> allLists)
        {
            foreach (IList<IByteable> list in allLists) 
            {
                if (list.GetType() == typeof(OffsetList<IByteable>))
                {
                    OffsetList<IByteable> offList = list as OffsetList<IByteable>;
                    int offset = offList.offset;
                    foreach (IByteable byteable in offList)
                    {
                        byte[] data = byteable.ToBytes();
                        Game.GetInstance().Write(offset, data);
                        offset += data.Length;
                    }
                }
                else if (list.GetType() == typeof(BackRefList<IByteable>))
                {
                    BackRefList<IByteable> refList = list as BackRefList<IByteable>;
                    refList.Verify();
                    foreach (BackRef bref in refList.ToBackRefs())
                    {
                        Game.GetInstance().Write(this.offset, bref.data);
                        Game.GetInstance().Write(bref.backref, this.offset);
                        this.offset += bref.data.Length;
                    }
                }
                else
                {
                    throw new Exception("Incompatible list type");
                }
            }
        }
    }
}
