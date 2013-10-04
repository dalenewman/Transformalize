#region License
// /*
// See license included in this library folder.
// */
#endregion
#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Returns details about current process and thread in a portable manner.
    /// </summary>
    internal abstract class ThreadIDHelper
    {
        /// <summary>
        ///     Initializes static members of the ThreadIDHelper class.
        /// </summary>
        static ThreadIDHelper()
        {
#if NET_CF
            Instance = new Win32ThreadIDHelper();
#else
            if (PlatformDetector.IsWin32)
            {
                Instance = new Win32ThreadIDHelper();
            }
            else
            {
                Instance = new PortableThreadIDHelper();
            }
#endif
        }

        /// <summary>
        ///     Gets the singleton instance of PortableThreadIDHelper or
        ///     Win32ThreadIDHelper depending on runtime environment.
        /// </summary>
        /// <value>The instance.</value>
        public static ThreadIDHelper Instance { get; private set; }

        /// <summary>
        ///     Gets current thread ID.
        /// </summary>
        public abstract int CurrentThreadID { get; }

        /// <summary>
        ///     Gets current process ID.
        /// </summary>
        public abstract int CurrentProcessID { get; }

        /// <summary>
        ///     Gets current process name.
        /// </summary>
        public abstract string CurrentProcessName { get; }

        /// <summary>
        ///     Gets current process name (excluding filename extension, if any).
        /// </summary>
        public abstract string CurrentProcessBaseName { get; }
    }
}

#endif