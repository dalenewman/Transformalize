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

using System.Linq;
using System.Text;

namespace Transformalize.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void TrimStart(this StringBuilder sb, string trimChars)
        {
            var length = sb.Length;
            if (length != 0)
            {
                var i = 0;

                if (trimChars.Length == 1)
                {
                    while (i < length && sb[i] == trimChars[0])
                    {
                        i++;
                    }
                }
                else
                {
                    while (i < length && trimChars.Any(c => c.Equals(sb[i])))
                    {
                        i++;
                    }
                }

                if (i > 0)
                {
                    sb.Remove(0, i);
                }
            }
        }

        public static void TrimEnd(this StringBuilder sb, string trimChars)
        {
            var length = sb.Length;
            if (length != 0)
            {
                var i = length - 1;

                if (trimChars.Length == 1)
                {
                    while (i > -1 && sb[i] == trimChars[0])
                    {
                        i--;
                    }
                }
                else
                {
                    while (i > -1 && trimChars.Any(c => c.Equals(sb[i])))
                    {
                        i--;
                    }
                }

                if (i < (length - 1))
                {
                    sb.Remove(i + 1, (length - i) - 1);
                }
            }
        }

        public static void Trim(this StringBuilder sb, string trimChars)
        {
            sb.TrimStart(trimChars);
            sb.TrimEnd(trimChars);
        }

        public static void Substring(this StringBuilder sb, int startIndex, int length)
        {
            var capacity = sb.Length;
            if (capacity != 0 && startIndex < capacity)
            {
                if (startIndex + length > capacity)
                {
                    length = capacity - startIndex;
                }

                sb.Remove(startIndex + length, capacity - (startIndex + length));
                sb.Remove(0, startIndex);
            }
        }

        public static void PadLeft(this StringBuilder sb, int totalWidth, char paddingChar)
        {
            var sbLen = sb.Length;
            if (sbLen < totalWidth)
            {
                for (var i = 0; i < totalWidth - sbLen; i++)
                {
                    sb.Insert(0, paddingChar);
                }
            }
        }

        public static void PadRight(this StringBuilder sb, int totalWidth, char paddingChar)
        {
            var sbLen = sb.Length;
            if (sbLen < totalWidth)
            {
                for (var i = 0; i < totalWidth - sbLen; i++)
                {
                    sb.Append(paddingChar);
                }
            }
        }

        public static void Left(this StringBuilder sb, int length)
        {
            sb.Substring(0, length);
        }

        public static void Right(this StringBuilder sb, int length)
        {
            sb.Substring(sb.Length - length, length);
        }

        public static bool IsEqualTo(this StringBuilder sb, string value)
        {
            var valLen = value.Length;
            if (sb.Length == valLen)
            {
                for (var i = 0; i < valLen; i++)
                {
                    if (!sb[i].Equals(value[i]))
                    {
                        break;
                    }
                    if (i == valLen - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool StartsWith(this StringBuilder sb, string value)
        {
            var valLen = value.Length;
            if (valLen < sb.Length)
            {
                for (var i = 0; i < valLen; i++)
                {
                    if (!sb[i].Equals(value[i]))
                    {
                        break;
                    }
                    if (i == valLen - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool EndsWith(this StringBuilder sb, string value)
        {
            var sbLen = sb.Length;
            var valLen = value.Length;
            if (valLen < sbLen)
            {
                for (var i = 0; i < valLen; i++)
                {
                    if (!value[i].Equals(sb[sbLen - valLen + i]))
                    {
                        break;
                    }
                    if (i == valLen - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ToLower(this StringBuilder sb)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                if (char.IsUpper(sb[i]))
                {
                    sb[i] = char.ToLower(sb[i]);
                }
            }
        }

        public static void ToUpper(this StringBuilder sb)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                if (char.IsLower(sb[i]))
                {
                    sb[i] = char.ToUpper(sb[i]);
                }
            }
        }
    }
}