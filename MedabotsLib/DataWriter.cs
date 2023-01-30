using GBALib;
using MedabotsLib.DataStructures;
using MedabotsLib.Utils;
using System;

namespace MedabotsLib
{
    /// <summary>
    /// A class that writes data to the ROM
    /// It is given an offset to start writing new objects at
    /// It can write OffsetLists and BackRefLists.
    /// </summary>
    public class DataWriter
    {
        int offset;

        /// <summary>
        /// Creates a new DataWriter
        /// </summary>
        /// <param name="startOffset">The offset to start writing new objects at</param>
        public DataWriter(int startOffset)
        {
            this.offset = startOffset;
        }

        /// <summary>
        /// Writes a list of objects from the offsetlist to the ROM in order
        /// The changes will be written in place
        /// </summary>
        public void Write<T>(Game game, OffsetList<T> offsetList) where T : IByteable
        {
            int offset = offsetList.offset;
            foreach (IByteable byteable in offsetList)
            {
                byte[] data = byteable.ToBytes();
                game.Write(offset, data);
                offset += data.Length;
            }
        }

        /// <summary>
        /// Writes a list of objects from the backreflist to the ROM in order
        /// Any new objects will be written at the end of the ROM and their backrefs will be updated
        /// </summary>
        public void Write<T>(Game game, BackRefList<T> backRefList) where T : IByteable
        {
            backRefList.Verify();
            foreach (BackRef bref in backRefList.ToBackRefs())
            {
                game.Write(this.offset, bref.data);
                this.offset += bref.data.Length;
                game.Write(bref.backref, this.offset);
            }
        }
    }
}
