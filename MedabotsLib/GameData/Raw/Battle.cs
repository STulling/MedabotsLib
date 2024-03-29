﻿using System.Runtime.InteropServices;
using MedabotsLib.Utils;

namespace MedabotsLib.GameData.Raw
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Battle : IByteable
    {
        public byte characterId;
        public byte unknown_1;
        public byte number_of_bots;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Medabot[] bots;
        public byte always_0;
    }
}
