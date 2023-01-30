using System.Runtime.InteropServices;
using MedabotsLib.Utils;

namespace MedabotsLib.GameData.Raw
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Encounters : IByteable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] battleId;
    }
}
