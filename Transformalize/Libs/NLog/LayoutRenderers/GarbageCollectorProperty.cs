#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion


#if !NET_CF && !MONO

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Gets or sets the property of System.GC to retrieve.
    /// </summary>
    public enum GarbageCollectorProperty
    {
        /// <summary>
        ///     Total memory allocated.
        /// </summary>
        TotalMemory,

        /// <summary>
        ///     Total memory allocated (perform full garbage collection first).
        /// </summary>
        TotalMemoryForceCollection,

        /// <summary>
        ///     Gets the number of Gen0 collections.
        /// </summary>
        CollectionCount0,

        /// <summary>
        ///     Gets the number of Gen1 collections.
        /// </summary>
        CollectionCount1,

        /// <summary>
        ///     Gets the number of Gen2 collections.
        /// </summary>
        CollectionCount2,

        /// <summary>
        ///     Maximum generation number supported by GC.
        /// </summary>
        MaxGeneration,
    }
}

#endif