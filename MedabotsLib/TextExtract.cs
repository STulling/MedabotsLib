using GBALib;
using MedabotsLib.Data;
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
        public static List<Text> Extract(int offset, int numEntries = 0)
        {
            List<Text> result = new List<Text>();
            foreach (int strOffset in Game.GetInstance().GetPtrTable(offset, numEntries))
            {
                byte[] encoded = Game.GetInstance().ReadUntil(strOffset, (byte)0xFE, inclusive: true);
                result.Add(new Text(encoded));
            }
            return result;
        }
    }
}
