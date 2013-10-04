#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Interface that provides parameters for create file function.
    /// </summary>
    internal interface ICreateFileParameters
    {
        int ConcurrentWriteAttemptDelay { get; }

        int ConcurrentWriteAttempts { get; }

        bool ConcurrentWrites { get; }

        bool CreateDirs { get; }

        bool EnableFileDelete { get; }

        int BufferSize { get; }

#if !NET_CF && !SILVERLIGHT
        Win32FileAttributes FileAttributes { get; }
#endif
    }
}