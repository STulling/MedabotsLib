using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static MedabotsLib.IdTranslator;

namespace MedabotsLib.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Legs : IByteable
    {
        public byte medal_compatibility;
        public byte legtype;
        public byte speciality;
        public byte gender;
        public byte armor;
        public byte propulsion;
        public byte evasion;
        public byte defense;
        public byte proximity;
        public byte remoteness;
        public byte unknown3;
        public byte unknown4;
        public byte unknown5;
        public byte unknown6;
        public byte unknown7;
        public byte unknown8;
    }
}
