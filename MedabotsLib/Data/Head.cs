using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static MedabotsLib.IdTranslator;

namespace MedabotsLib.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Head : IByteable
    {
        public byte medal_compatibility;
        public byte technique;
        public byte speciality;
        public byte gender;
        public byte armor;
        public byte RoS;
        public byte power;
        public byte chain_reaction;
        public byte amount_of_uses;
        public byte unknown2;
        public byte unknown3;
        public byte unknown4;
        public byte unknown5;
        public byte unknown6;
        public byte unknown7;
        public byte unknown8;
    }
}
