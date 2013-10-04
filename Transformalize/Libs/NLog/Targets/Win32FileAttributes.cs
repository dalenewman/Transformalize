#region License
// /*
// See license included in this library folder.
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