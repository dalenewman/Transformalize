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

using System.Text;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     URL Encoding helper.
    /// </summary>
    internal class UrlHelper
    {
        private static string safeUrlPunctuation = ".()*-_!'";
        private static string hexChars = "0123456789abcdef";

        internal static string UrlEncode(string str, bool spaceAsPlus)
        {
            var result = new StringBuilder(str.Length + 20);
            for (var i = 0; i < str.Length; ++i)
            {
                var ch = str[i];

                if (ch == ' ' && spaceAsPlus)
                {
                    result.Append('+');
                }
                else if (IsSafeUrlCharacter(ch))
                {
                    result.Append(ch);
                }
                else if (ch < 256)
                {
                    result.Append('%');
                    result.Append(hexChars[(ch >> 4) & 0xF]);
                    result.Append(hexChars[(ch >> 0) & 0xF]);
                }
                else
                {
                    result.Append('%');
                    result.Append('u');
                    result.Append(hexChars[(ch >> 12) & 0xF]);
                    result.Append(hexChars[(ch >> 8) & 0xF]);
                    result.Append(hexChars[(ch >> 4) & 0xF]);
                    result.Append(hexChars[(ch >> 0) & 0xF]);
                }
            }

            return result.ToString();
        }

        private static bool IsSafeUrlCharacter(char ch)
        {
            if (ch >= 'a' && ch <= 'z')
            {
                return true;
            }

            if (ch >= 'A' && ch <= 'Z')
            {
                return true;
            }

            if (ch >= '0' && ch <= '9')
            {
                return true;
            }

            return safeUrlPunctuation.IndexOf(ch) >= 0;
        }
    }
}