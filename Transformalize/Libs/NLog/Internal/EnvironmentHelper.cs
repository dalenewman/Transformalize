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