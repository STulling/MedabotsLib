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
        protected List<T> list;
        protected Dictionary<int, T> changes;
        protected bool locked = false;

        public TrackingList(List<T> list)
        {
            this.list = list;
            this.changes = new Dictionary<int, T>();
            this.Lock();
        }

        public TrackingList()
        {
            this.list = new List<T>();
            this.changes = new Dictionary<int, T>();
        }

        public T this[int i]
        {
            get { 
                return list[i];
            }
            set { 
                if (this.locked)
                {
                    if (locked)
                    {
                        changes.Add(i, value);
                    }
                    list[i] = value;
                }
                else
                {
                    list[i] = value;
                }
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public int Length { get; internal set; }

        public void Add(T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot add to locked TrackingList");
            }
            this.list.Add(item);
        }

        public void Lock()
        {
            if (this.locked)
            {
                throw new Exception("Cannot lock locked TrackingList again");
            }
            this.locked = true;
            this.list = new List<T>(this.list);
        }

        public void Clear()
        {
            if (this.locked)
            {
                throw new Exception("Cannot clear locked TrackingList");
            }
            this.list.Clear();
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
            return new TrackingListEnum<T>(this);
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot insert into locked TrackingList");
            }
            this.list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            if (this.locked)
            {
                throw new Exception("Cannot remove from locked TrackingList");
            }
            return this.list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (this.locked)
            {
                throw new Exception("Cannot remove at from locked TrackingList");
            }
            this.list.RemoveAt(index);
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
