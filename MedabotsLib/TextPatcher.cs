using GBALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class TextPatcher
    {
        Dictionary<(int, int), byte[]> messages;
        Dictionary<(int, int), (byte[], int)> storedMessages;
        int offset;
        int dumpOffset;
        byte[] file;
        public TextPatcher(ref byte[] file, int offset, int dumpOffset, Dictionary<(int, int), byte[]> messages)
        {
            this.file = file;
            this.offset = offset;
            this.messages = messages;
            this.dumpOffset = dumpOffset;
            this.storeText();
        }

        private void storeText()
        {
            storedMessages = new Dictionary<(int, int), (byte[], int)>();
            foreach (KeyValuePair<(int, int), byte[]> entry in messages)
            {
                storedMessages.Add(entry.Key, (entry.Value, dumpOffset));
                Array.Copy(entry.Value, 0, file, dumpOffset, entry.Value.Length);
                this.dumpOffset += entry.Value.Length;
            }
        }

        public void PatchText()
        {
            foreach (KeyValuePair<(int, int), (byte[], int)> entry in storedMessages)
            {
                int originalTextPointer = getAddress(entry.Key);
                Game.GetInstance().Write(originalTextPointer, entry.Value.Item2 + 0x08000000);
            }
        }

        private int getAddress((int, int) id)
        {
            int subAdress = Game.GetInstance().ReadLocalAddress(this.offset + 4 * id.Item1);
            int actualAdress = subAdress + 4 * id.Item2;
            return actualAdress;
        }
    }
}
