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

        public void Write()
        {
            foreach (IList<Byteable> list in GameData.Data) 
            {
                if (list.GetType() == typeof(OffsetList<Byteable>))
                {
                    OffsetList<Byteable> offList = list as OffsetList<Byteable>;
                    int offset = offList.offset;
                    foreach (Byteable byteable in offList)
                    {
                        byte[] data = byteable.ToBytes();
                        Game.GetInstance().Write(offset, data);
                        offset += data.Length;
                    }
                }
                else if (list.GetType() == typeof(BackRefList<Byteable>))
                {
                    BackRefList<Byteable> refList = list as BackRefList<Byteable>;
                    foreach (BackRef bref in refList.ToBackRefs())
                    {
                        Game.GetInstance().Write(this.offset, bref.data);
                        Game.GetInstance().Write(bref.backref, this.offset);
                        this.offset += bref.data.Length;
                    }
                }
                else
                {
                    throw new Exception("Unknown list type");
                }
            }
        }
    }
}
