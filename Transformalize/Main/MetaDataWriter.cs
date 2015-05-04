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
using System.Linq;
using System.Text;
using System.Web;
using Transformalize.Logging;

namespace Transformalize.Main {
    public class MetaDataWriter {

        private readonly Process _process;
        private static readonly char[] BadCharacters = { ' ', '(', ')', '/', '\\' };

        public MetaDataWriter(Process process) {
            _process = process;
        }

        public string Write() {
            var content = new StringBuilder();
            content.AppendLine("<tfl><processes><add name=\"metadata\"><entities>");

            var count = 0;
            foreach (var entity in _process.Entities) {
                var firstConnection = entity.Input.First().Connection;
                var e = entity;
                var fields = firstConnection.GetEntitySchema(_process, e, count == 0);
                content.AppendFormat("    <add name=\"{0}\">\r\n", entity.Name);
                AppendFields(fields.WithOutput(), content);
                content.AppendLine("    </add>");
                count++;
            }

            content.AppendLine("</entities></add></processes></tfl>");
            return content.ToString();
        }

        private void AppendFields(OrderedFields fields, StringBuilder content) {
            var entity = fields.Any() ? fields.First().Entity : string.Empty;
            _process.Logger.EntityDebug(entity, "Entity auto found {0} field{1}.", fields.Count, fields.Count == 1 ? string.Empty : "s");

            content.AppendLine("      <fields>");
            foreach (var f in fields) {
                AppendField(content, f);
            }
            content.AppendLine("      </fields>");
        }

        private static void AppendField(StringBuilder content, Field f) {
            if (String.IsNullOrEmpty(f.Label) || f.Label.Equals(f.Alias)) {
                f.Label = AddSpacesToSentence(f.Alias, true).Replace("_", " ");
            }
            content.AppendFormat("        <add name=\"{0}\"{1}{2}{3}{4}{5}{6}{7}{8}{9}></add>\r\n",
                f.Name,
                BadCharacters.Any(c => f.Name.Contains(c)) ? " alias=\"" + ReplaceBadCharacters(f.Name) + "\" " : " ",
                f.SimpleType.Equals("string") ? string.Empty : "type=\"" + f.SimpleType + "\" ",
                !f.Length.Equals("0") && !f.Length.Equals(string.Empty) && !f.Length.Equals("64") ? "length=\"" + f.Length + "\" " : string.Empty,
                f.SimpleType == "decimal" && f.Precision > 0 ? "precision=\"" + f.Precision + "\" " : string.Empty,
                f.SimpleType == "decimal" && f.Scale > 0 ? "scale=\"" + f.Scale + "\" " : string.Empty,
                f.FieldType.HasFlag(FieldType.PrimaryKey) || f.FieldType.HasFlag(FieldType.MasterKey) ? "primary-key=\"true\" " : string.Empty,
                f.IsQuoted() ? string.Format("quoted-with=\"{0}\" ", HttpUtility.HtmlEncode(f.QuotedWith)) : string.Empty,
                !f.Input ? string.Format("input=\"false\" ") : string.Empty,
                !f.Alias.Equals(f.Label) ? string.Format("label=\"{0}\" ", f.Label) : string.Empty
            );
        }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms) {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++) {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        private static string ReplaceBadCharacters(string input) {
            var builder = new StringBuilder(input);
            foreach (var c in BadCharacters) {
                builder.Replace(c, ' ');
            }
            builder.Replace(" ", string.Empty);
            return builder.ToString();
        }
    }
}