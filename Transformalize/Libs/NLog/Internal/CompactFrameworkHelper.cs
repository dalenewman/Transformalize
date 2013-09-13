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

namespace Transformalize.Libs.NLog.Internal
{
#if NET_CF

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NLog.Internal
{
    /// <summary>
    /// Helpers for .NET Compact Framework.
    /// </summary>
    internal sealed class CompactFrameworkHelper
    {
        private static string exeName;
        private static string exeBaseDir;

        internal static string GetExeFileName()
        {
            if (exeName == null)
            {
                LoadExeInfo();
            }

            return exeName;
        }

        internal static string GetExeBaseDir()
        {
            if (exeName == null)
            {
                LoadExeInfo();
            }

            return exeBaseDir;
        }

        private static void LoadExeInfo()
        {
            lock (typeof(CompactFrameworkHelper))
            {
                if (exeName == null)
                {
                    StringBuilder sb = new StringBuilder(512);

                    // passing 0 as the first parameter gets us the name of the EXE
                    GetModuleFileName(IntPtr.Zero, sb, sb.Capacity);
                    exeName = sb.ToString();
                    exeBaseDir = Path.GetDirectoryName(exeName);
                }
            }
        }

        [DllImport("coredll.dll", CharSet = CharSet.Unicode)]
        private static extern int GetModuleFileName(IntPtr module, StringBuilder buffer, int capacity);
    }
}

#endif
}