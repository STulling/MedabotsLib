using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Utils
{
    /// <summary>
    /// An interface for structs that can be modified and need to be tracked for changes
    /// </summary>
    public interface ICanGetDirty
    {
        public bool IsDirty { get; }
    }
}
