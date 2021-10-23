using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    public class TrackingList<T> : IList<T>
    {
        List<T> original;
        Dictionary<int, T> replacement;
        List<T> applied;
        int offset;
        bool locked;

        public TrackingList(List<T> list, int offset)
        {
            this.original = list;
            this.offset = offset;
            this.locked = true;
        }

        public TrackingList(int offset)
        {
            this.original = new List<T>();
            this.offset = offset;
            this.locked = false;
        }

        public T this[int i]
        {
            get { 
                return applied[i];
            }
            set { 
                if (this.locked)
                {
                    replacement.Add(i, value);
                    applied[i] = value;
                }
                else
                {
                    original[i] = value;
                }
            }
        }

        public int Count => original.Count;

        public bool IsReadOnly => false;

        public int Length { get; internal set; }

        public void Add(T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot add to locked TrackingList");
            }
            this.original.Add(item);
        }

        public void Lock()
        {
            if (this.locked)
            {
                throw new Exception("Cannot lock locked TrackingList again");
            }
            this.locked = true;
            this.applied = new List<T>(this.original);
        }

        public void Clear()
        {
            if (this.locked)
            {
                throw new Exception("Cannot clear locked TrackingList");
            }
            this.original.Clear();
        }

        public bool Contains(T item)
        {
            return applied.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            applied.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TrackingListEnum<T>(this);
        }

        public int IndexOf(T item)
        {
            return applied.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot insert into locked TrackingList");
            }
            this.original.Insert(index, item);
        }

        public bool Remove(T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot remove from locked TrackingList");
            }
            return this.original.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (this.locked)
            {
                throw new Exception("Cannot remove at from locked TrackingList");
            }
            this.original.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    class TrackingListEnum<T> : IEnumerator<T>
    {
        public TrackingList<T> list;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public TrackingListEnum(TrackingList<T> list)
        {
            this.list = list;
        }

        public bool MoveNext()
        {
            position++;
            return position < list.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose() { }

        T IEnumerator<T>.Current
        {
            get
            {
                return Current;
            }
        }

        public T Current
        {
            get
            {
                return list[position];
            }
        }

        object IEnumerator.Current => this.Current;
    }
}
