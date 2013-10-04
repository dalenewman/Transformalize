#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    internal static class NativeMethods
    {
        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);

        // creates duplicate token handle
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DuplicateToken(IntPtr existingTokenHandle, int impersonationLevel, out IntPtr duplicateTokenHandle);

        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "We specifically need this API")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern void OutputDebugString(string message);

#if !NET_CF
        [DllImport("kernel32.dll")]
#else
        [DllImport("coredll.dll")]
#endif
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceCounter(out ulong lpPerformanceCount);

#if !NET_CF
        [DllImport("kernel32.dll")]
#else
        [DllImport("coredll.dll")]
#endif
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceFrequency(out ulong lpPerformanceFrequency);

#if !NET_CF
        [DllImport("kernel32.dll")]
#else
        [DllImport("coredll.dll")]
#endif
        internal static extern int GetCurrentProcessId();

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
#if !NET_CF
        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true, CharSet = CharSet.Unicode)]
#else
        [DllImport("coredll.dll", SetLastError = true, PreserveSig = true, CharSet = CharSet.Unicode)]
#endif
        internal static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

#if !NET_CF
        [DllImport("ole32.dll")]
        internal static extern int CoGetObjectContext(ref Guid iid, out AspHelper.IObjectContext g);
#endif
    }
}

#endif