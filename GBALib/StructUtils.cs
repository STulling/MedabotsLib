using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GBALib
{
    public static class StructUtils
    {
        public static List<T> Populate_Data<T>(byte[] file, int amount, int size, int offset, bool is_ptr)
        {
            List<T> result = new List<T>();
            for (int i = 0; i <= amount; i++)
            {
                int address;
                if (is_ptr)
                {
                    address = Utils.GetAdressAtPosition(file, offset + 4 * i);
                }
                else
                {
                    address = offset + size * i;
                }
                byte[] slice = new byte[size];
                Array.Copy(file, address, slice, 0, size);
                result.Add((T)Activator.CreateInstance(typeof(T), new object[] { i, address, slice }));
            }
            return result;
        }

        public static byte[] getBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}
