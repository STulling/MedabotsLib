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
        public List<string> parseText(byte[] file, int offset, int numEntries = 0)
        {
            bool autoHalt = (numEntries == 0);
            int i = 0;
            List<string> result = new List<string>();
            while (i < numEntries || autoHalt)
            {
                Game.GetInstance().ReadAddress(offset + 4 * i);
            }
        }
    }
}
