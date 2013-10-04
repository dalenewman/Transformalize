#region License
// /*
// See license included in this library folder.
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