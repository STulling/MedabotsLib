using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public static class GameData
    {
        public static TrackingList<string> MedalNames = new TrackingList<string>(TextExtract.Extract(0x3b65b0), 0x3b65b0));
    }
}
