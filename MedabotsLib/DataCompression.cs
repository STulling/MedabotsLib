using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public static class DataCompression
    {
        //TODO: Make smarter
        public static byte[] CompressNatsumeGBA(byte[] data)
        {
            byte[] padded = new byte[data.Length + 20];
            Array.Copy(data, 0, padded, 0, data.Length);
            List<byte> compressed = new List<byte>();
            compressed.Add((byte)'L');
            compressed.Add((byte)'e');
            byte[] intBytes = BitConverter.GetBytes(data.Length); 
            Array.Reverse(intBytes);
            foreach (byte b in intBytes)
            {
                compressed.Add(b);
            }
            int i = 0;
            while (i < data.Length)
            {
                compressed.Add(0xAA);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
            }
            return compressed.ToArray();
        }

        //TODO: Make smarter
        public static byte[] CompressLZ77(byte[] data)
        {
            byte[] padded = new byte[data.Length + 10];
            Array.Copy(data, 0, padded, 0, data.Length);
            List<byte> compressed = new List<byte>();
            compressed.Add(0x10);
            compressed.Add(0x00);
            compressed.Add(0x09);
            compressed.Add(0x00);
            int i = 0;
            while (i < data.Length)
            {
                compressed.Add(0x0);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
                compressed.Add(padded[i++]);
            }
            return compressed.ToArray();
        }
    }
}
