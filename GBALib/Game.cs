using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GBALib
{
    public class Game
    {
        private Game() { }

        private static Game _instance;
        private byte[] ROM;

        public static void Create(byte[] ROM)
        {
            if (_instance == null)
            {
                _instance = new Game
                {
                    ROM = ROM
                };
            }
            else
            {
                throw new Exception("Cannot create the ROM twice.");
            }
        }

        public static Game GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Game();
            }
            return _instance;
        }

        public T[] ReadUntil<T>(int offset, T trigger, bool inclusive = false)
        {
            return ReadUntil(offset, new T[] { trigger }, inclusive);
        }

        public T[] ReadUntil<T>(int offset, T[] trigger, bool inclusive = false)
        {
            int size = Marshal.SizeOf(default(T));
            int i = 0;
            List<T> result = new List<T>();
            while (true)
            {
                T item = Read<T>(offset + i * size);
                if (trigger.Contains(item))
                {
                    if (inclusive)
                    {
                        result.Add(item);
                    }
                    return result.ToArray();
                }
                result.Add(item);
            }
        }

        public List<int> GetPtrTable(int offset, int amount = 0)
        {
            List<int> result = new List<int>();
            int i = 0;
            while (amount == 0 || i < amount)
            {
                int address = Read<int>(offset + 4 * i);
                if (!Utils.IsAddress(address))
                {
                    break;
                }
                result.Add(address);
                i++;
            }
            return result;
        }

        public List<T> ReadObjects<T>(int offset, int amount, bool is_ptr_array = false)
        {
            List<T> result = new List<T>();
            int size = Marshal.SizeOf(default(T));
            for (int i = 0; i <= amount; i++)
            {
                int address;
                if (is_ptr_array)
                {
                    address = ReadLocalAddress(offset + 4 * i);
                }
                else
                {
                    address = offset + size * i;
                }
                result.Add(Utils.Read<T>(ROM, address));
            }
            return result;
        }

        public void WritePatches(Dictionary<int, ushort> codePatches)
        {
            foreach (KeyValuePair<int, ushort> entry in codePatches)
            {
                Utils.Write(ROM, entry.Key, entry.Value);
            }
        }

        public int ReadLocalAddress(int offset)
        {
            return Utils.ToLocal(Utils.Read<int>(ROM, offset));
        }

        public void Write<T>(int offset, T data)
        {
            Utils.Write(ROM, offset, data);
        }

        public T Read<T>(int offset)
        {
            return Utils.Read<T>(ROM, offset);
        }

        public void WritePayload<T>(int offset, T[] payload)
        {
            Utils.WritePayload(ROM, offset, payload);
        }

        public List<int> SearchAll(byte[] needle)
        {
            return Utils.SearchAll(ROM, needle);
        }
    }
}
