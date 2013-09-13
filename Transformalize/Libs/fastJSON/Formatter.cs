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

namespace Transformalize.Libs.fastJSON
{
    internal static class Formatter
    {
        public static string Indent = "   ";

        public static void AppendIndent(StringBuilder sb, int count)
        {
            for (; count > 0; --count) sb.Append(Indent);
        }

        public static bool IsEscaped(StringBuilder sb, int index)
        {
            var escaped = false;
            while (index > 0 && sb[--index] == '\\') escaped = !escaped;
            return escaped;
        }

        public static string PrettyPrint(string input)
        {
            var output = new StringBuilder(input.Length*2);
            char? quote = null;
            var depth = 0;

            for (var i = 0; i < input.Length; ++i)
            {
                var ch = input[i];

                if (ch == '\"') // found string span
                {
                    var str = true;
                    while (str)
                    {
                        output.Append(ch);
                        ch = input[++i];
                        if (ch == '\\')
                        {
                            if (input[i + 1] == '\"')
                            {
                                output.Append(ch);
                                ch = input[++i];
                            }
                        }
                        else if (ch == '\"')
                            str = false;
                    }
                }

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            AppendIndent(output, ++depth);
                        }
                        break;
                    case '}':
                    case ']':
                        if (quote.HasValue)
                            output.Append(ch);
                        else
                        {
                            output.AppendLine();
                            AppendIndent(output, --depth);
                            output.Append(ch);
                        }
                        break;
                    case ',':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            AppendIndent(output, depth);
                        }
                        break;
                    case ':':
                        if (quote.HasValue) output.Append(ch);
                        else output.Append(" : ");
                        break;
                    default:
                        if (quote.HasValue || !char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }
    }
}