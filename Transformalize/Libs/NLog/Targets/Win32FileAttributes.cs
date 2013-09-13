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

using System;
using System.Diagnostics.CodeAnalysis;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Win32 file attributes.
    /// </summary>
    /// <remarks>
    ///     For more information see
    ///     <a
    ///         href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/createfile.asp">
    ///         http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/createfile.asp
    ///     </a>
    ///     .
    /// </remarks>
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "This set of flags matches Win32 API")]
    [Flags]
    public enum Win32FileAttributes
    {
        /// <summary>
        ///     Read-only file.
        /// </summary>
        ReadOnly = 0x00000001,

        /// <summary>
        ///     Hidden file.
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        ///     System file.
        /// </summary>
        System = 0x00000004,

        /// <summary>
        ///     File should be archived.
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        ///     Device file.
        /// </summary>
        Device = 0x00000040,

        /// <summary>
        ///     Normal file.
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        ///     File is temporary (should be kept in cache and not
        ///     written to disk if possible).
        /// </summary>
        Temporary = 0x00000100,

        /// <summary>
        ///     Sparse file.
        /// </summary>
        SparseFile = 0x00000200,

        /// <summary>
        ///     Reparse point.
        /// </summary>
        ReparsePoint = 0x00000400,

        /// <summary>
        ///     Compress file contents.
        /// </summary>
        Compressed = 0x00000800,

        /// <summary>
        ///     File should not be indexed by the content indexing service.
        /// </summary>
        NotContentIndexed = 0x00002000,

        /// <summary>
        ///     Encrypted file.
        /// </summary>
        Encrypted = 0x00004000,

        /// <summary>
        ///     The system writes through any intermediate cache and goes directly to disk.
        /// </summary>
        WriteThrough = unchecked((int) 0x80000000),

        /// <summary>
        ///     The system opens a file with no system caching.
        /// </summary>
        NoBuffering = 0x20000000,

        /// <summary>
        ///     Delete file after it is closed.
        /// </summary>
        DeleteOnClose = 0x04000000,

        /// <summary>
        ///     A file is accessed according to POSIX rules.
        /// </summary>
        PosixSemantics = 0x01000000,
    }
}

#endif