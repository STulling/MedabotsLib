namespace MedabotsLib.DataStructures
{
    /// <summary>
    /// A class that represents an object and where it is referenced in the ROM
    /// </summary>
    public struct BackRef
    {
        public int backref;
        public byte[] data;

        public BackRef(int backref, byte[] data) : this()
        {
            this.backref = backref;
            this.data = data;
        }
    }
}
