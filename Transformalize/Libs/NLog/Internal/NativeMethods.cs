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