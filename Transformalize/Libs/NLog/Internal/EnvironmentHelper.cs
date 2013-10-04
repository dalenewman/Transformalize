#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Security;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Safe way to get environment variables.
    /// </summary>
    internal static class EnvironmentHelper
    {
        internal static string NewLine
        {
            get
            {
#if !SILVERLIGHT && !NET_CF
                var newline = Environment.NewLine;
#else
                string newline = "\r\n";
#endif

                return newline;
            }
        }

#if !NET_CF && !SILVERLIGHT
        internal static string GetSafeEnvironmentVariable(string name)
        {
            try
            {
                var s = Environment.GetEnvironmentVariable(name);

                if (s == null || s.Length == 0)
                {
                    return null;
                }

                return s;
            }
            catch (SecurityException)
            {
                return string.Empty;
            }
        }
#endif
    }
}