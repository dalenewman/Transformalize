#region License
// /*
// See license included in this library folder.
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