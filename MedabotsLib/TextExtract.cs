using GBALib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    class TextExtract
    {
        public static List<string> Extract(int offset, int numEntries = 0)
        {
            List<string> result = new List<string>();
            foreach (int strOffset in Game.GetInstance().GetPtrTable(offset, numEntries))
            {
                byte[] encoded = Game.GetInstance().ReadUntil(strOffset, (byte)0xFE);
                string decoded = Encoding.Decode(encoded);
                result.Add(decoded);
            }
            return result;
        }
    }
}
