using GBALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class MemWriter
    {
        int offset;
        byte[] file;
        public MemWriter(byte[] file, int offset)
        {
            this.file = file;
            this.offset = offset;
        }

        public int PatchMemory(byte[] data)
        {
            int memoryOffset = this.offset;

            Game.GetInstance().WritePayload(memoryOffset - 0x8000000, data);
            this.offset += data.Length;

            return memoryOffset;
        }

        public void PatchMemoryAndStoreAddress(byte[] data, int address)
        {
            int stored_offset = PatchMemory(data);
            Game.GetInstance().Write(address, stored_offset);
        }
    }
}
