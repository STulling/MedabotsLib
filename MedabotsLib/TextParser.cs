using GBALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class TextParser
    {
        public Dictionary<(int, int), string> origMessages;
        Dictionary<(int, int), string> messages;
        byte[] file;
        int offset;
        public TextParser(byte[] file, int offset)
        {
            this.file = file;
            this.offset = offset;
            messages = new Dictionary<(int, int), string>();
            origMessages = parseAll();
        }
        public Dictionary<(int, int), byte[]> getEncodedMessages()
        {
            Dictionary<(int, int), byte[]> result = new Dictionary<(int, int), byte[]>();
            foreach (KeyValuePair<(int, int), string> entry in messages)
            {
                result.Add(entry.Key, Encoding.Encode(entry.Value));
            }
            return result;
        }

        public void addMessage((int, int) id, string message)
        {
            if (!messages.ContainsKey(id))
                messages.Add(id, message);
        }

        private Dictionary<(int, int), string> parseAll()
        {
            Dictionary<(int, int), string> textAdresses = new Dictionary<(int, int), string>();
            int amount_of_ptrs = 15;
            for (int i = 0; i <= amount_of_ptrs; i++)
            {
                int textPtrOffset = Utils.GetAdressAtPosition(file, this.offset + 4 * i);
                int j = 0;
                while (true)
                {
                    int textOffset = Utils.GetAdressAtPosition(file, textPtrOffset + 4 * j);
                    if (textOffset == -0x08000000) break;
                    textAdresses.Add((i, j), parseBytes(textOffset));
                    j++;
                }
            }
            return textAdresses;
        }

        public string parseBytes(int textAddress)
        {
            List<byte> data = new List<byte>();
            int i = 0;
            while (true)
            {
                byte currByte = file[textAddress + i];
                if (currByte == 0xFF || currByte == 0xFE)
                {
                    data.Add(currByte);
                    i++;
                    data.Add(file[textAddress + i]);
                    break;
                }
                else if (currByte == 0xF7 || currByte == 0xFA || currByte == 0xF9)
                {
                    data.Add(currByte);
                    data.Add(file[textAddress + i + 1]);
                    i += 2;
                }
                else if (currByte == 0xFB)
                {
                    data.Add(currByte);
                    data.Add(file[textAddress + i + 1]);
                    data.Add(file[textAddress + i + 2]);
                    data.Add(file[textAddress + i + 3]);
                    i += 4;
                }
                else
                {
                    data.Add(currByte);
                    i++;
                }
            }
            return Encoding.Decode(data.ToArray());
        }
    }
}
