namespace GBALib
{
    public class Octree<T> 
    {
        public Octree<T> Parent { get; private set; }
        public Octree<T>[] Children { get; private set; }
        public T Value { get; set; }

        public Octree(Octree<T> parent, T value)
        {
            Parent = parent;
            Value = value;
            Children = new Octree<T>[8];
        }

        public Octree<T> this[int index]
        {
            get { return Children[index]; }
            set { Children[index] = value; }
        }

        public Octree<T> this[int x, int y, int z]
        {
            get { return Children[x + y * 2 + z * 4]; }
            set { Children[x + y * 2 + z * 4] = value; }
        }
    }
}
