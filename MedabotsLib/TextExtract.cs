using GBALib;
using MedabotsLib.GameData;
using System.Collections.Generic;
using System.Linq;

namespace MedabotsLib
{
    class TextExtract
    {
        public static List<Text> Extract(Game game, int offset, int numEntries = 0)
        {
            List<Text> result = new List<Text>();
            foreach (int strOffset in game.GetPtrTable(offset, numEntries))
            {
                byte[] encoded = game.ReadUntil(strOffset, new byte[] { 0xFE, 0xFF }, getnext: 2);
                if (encoded[encoded.Length-2] == 0xFE)
                {
                    encoded = encoded.SkipLast(2).ToArray();
                }
                result.Add(new Text(encoded));
            }
            return result;
        }
    }
}
