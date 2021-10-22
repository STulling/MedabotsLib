using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GBALib
{
    public static class Utils
    {
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        public static byte[] GetHashSha256(string filename)
        {
            SHA256 Sha256 = SHA256.Create();
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }

        public static bool IsAddress(int address)
        {
            int norm = address - 0x08000000;
            return norm > 0 && norm < 0x7fffff;
        }

        public static void Write<T>(byte[] bytes, int offset, T opcode)
        {
            byte[] result = (byte[])typeof(BitConverter).GetMethod("GetBytes", new[] { typeof(T) })
                .Invoke(null, new object[] { opcode });
            for (int i = 0; i < result.Length; i++)
            {
                bytes[offset + i] = result[i];
            }
        }

        public static T FromBytes<T>(byte[] bytes)
        {
            GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T obj = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
            gcHandle.Free();
            return obj;
        }

        public static T Read<T>(byte[] file, int offset)
        {
            int size = Marshal.SizeOf(default(T));
            byte[] slice = new byte[size];
            Array.Copy(file, offset, slice, 0, size);
            return FromBytes<T>(slice);
        }

        public static void WritePayload<T>(byte[] bytes, int offset, T[] payload)
        {
            for (int i = 0; i < payload.Length; i++)
            {
                Write(bytes, offset + Marshal.SizeOf(default(T)) * i, payload[i]);
            }
        }

        public static string GetHash(string filename)
        {
            return BytesToString(GetHashSha256(filename));
        }

        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int ToLocal(int address)
        {
            return address - 0x08000000;
        }

        public static int Search(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (match(haystack, needle, i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<int> SearchAll(byte[] haystack, byte[] needle)
        {
            List<int> result = new List<int>();
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (match(haystack, needle, i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        private static bool match(byte[] haystack, byte[] needle, int start)
        {
            if (needle.Length + start > haystack.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (needle[i] != haystack[i + start])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] ReadBytes(byte[] file, int offset, int amount)
        {
            byte[] result = new byte[amount];
            Array.Copy(file, offset, result, 0, amount);
            return result;
        }
    }
}
