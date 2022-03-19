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
        Game game;
        public MemWriter(Game game, int offset)
        {
            this.game = game;
            this.offset = offset;
        }

        public int PatchMemory(byte[] data)
        {
            int memoryOffset = this.offset;

            game.WritePayload(memoryOffset - 0x8000000, data);
            this.offset += data.Length;

            return memoryOffset;
        }

        public void PatchMemoryAndStoreAddress(byte[] data, int address)
        {
            int stored_offset = PatchMemory(data);
            game.Write(address, stored_offset);
        }
    }
}
