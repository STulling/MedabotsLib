using System.Collections;
using System.Collections.Generic;
using MedabotsLib.Utils;

namespace MedabotsLib.DataStructures
{
    /// <summary>
    /// A list of binary structs that start at a certain offset in the ROM
    /// </summary>
    /// <typeparam name="T">The binary struct of the list</typeparam>
    public class OffsetList<T> : IList<T> where T : IByteable
    {
        public int offset;
        public List<T> list;

        /// <summary>
        /// Creates a new OffsetList
        /// </summary>
        /// <param name="list">The list of structs</param>
        /// <param name="offset">The offset of the structs in the ROM</param>
        public OffsetList(List<T> list, int offset)
        {
            this.list = list;
            this.offset = offset;
        }

        public T this[int index] { get => list[index]; set => list[index] = value; }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
