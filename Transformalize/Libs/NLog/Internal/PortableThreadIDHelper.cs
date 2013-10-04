#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

#if !SILVERLIGHT && !NET_CF

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Portable implementation of <see cref="ThreadIDHelper" />.
    /// </summary>
    internal class PortableThreadIDHelper : ThreadIDHelper
    {
        private const string UnknownProcessName = "<unknown>";

        private readonly int currentProcessID;

        private string currentProcessBaseName;
        private string currentProcessName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PortableThreadIDHelper" /> class.
        /// </summary>
        public PortableThreadIDHelper()
        {
            currentProcessID = Process.GetCurrentProcess().Id;
        }

        /// <summary>
        ///     Gets current thread ID.
        /// </summary>
        /// <value></value>
        public override int CurrentThreadID
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }

        /// <summary>
        ///     Gets current process ID.
        /// </summary>
        /// <value></value>
        public override int CurrentProcessID
        {
            get { return currentProcessID; }
        }

        /// <summary>
        ///     Gets current process name.
        /// </summary>
        /// <value></value>
        public override string CurrentProcessName
        {
            get
            {
                GetProcessName();
                return currentProcessName;
            }
        }

        /// <summary>
        ///     Gets current process name (excluding filename extension, if any).
        /// </summary>
        /// <value></value>
        public override string CurrentProcessBaseName
        {
            get
            {
                GetProcessName();
                return currentProcessBaseName;
            }
        }

        /// <summary>
        ///     Gets the name of the process.
        /// </summary>
        private void GetProcessName()
        {
            if (currentProcessName == null)
            {
                try
                {
                    currentProcessName = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    currentProcessName = UnknownProcessName;
                }

                currentProcessBaseName = Path.GetFileNameWithoutExtension(currentProcessName);
            }
        }
    }
}

#endif