using System.Runtime.InteropServices;

namespace MedabotsLib.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Encounters : Byteable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] battleId;
    }
}
