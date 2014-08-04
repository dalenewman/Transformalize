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
using System.Web;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class MetaDataWriter {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly Process _process;
        private static readonly char[] BadCharacters = new[] { ' ', '(', ')', '/', '\\' };

        public MetaDataWriter(Process process) {
            _process = process;
        }

        public string Write() {
            var content = new StringBuilder();
            content.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            content.AppendLine("<process>");
            content.AppendLine("  <entities>");

            var count = 0;
            foreach (var entity in _process.Entities) {
                var firstConnection = entity.Input.First().Connection;
                var fields = firstConnection.GetEntitySchema(_process, entity.Name, entity.Schema, count == 0).Fields;
                content.AppendFormat("    <add name=\"{0}\">\r\n", entity.Name);
                AppendFields(fields.WithOutput(), content);
                content.AppendLine("    </add>");
                count++;
            }

            content.AppendLine("  </entities>");
            content.AppendLine("</process>");
            return content.ToString();
        }

        private void AppendFields(OrderedFields fields, StringBuilder content) {
            _log.Debug("Entity auto found {0} field{1}.", fields.Count, fields.Count == 1 ? string.Empty : "s");

            content.AppendLine("      <fields>");
            foreach (Field f in fields) {
                AppendField(content, f);
            }
            content.AppendLine("      </fields>");
        }

        private static void AppendField(StringBuilder content, Field f) {
            content.AppendFormat("        <add name=\"{0}\"{1}{2}{3}{4}{5}{6}{7}></add>\r\n",
                f.Name,
                BadCharacters.Any(c => f.Name.Contains(c)) ? " alias=\"" + ReplaceBadCharacters(f.Name) + "\" " : " ",
                f.SimpleType.Equals("string") ? string.Empty : "type=\"" + f.Type + "\" ",
                !f.Length.Equals("0") && !f.Length.Equals(string.Empty) && !f.Length.Equals("64") ? "length=\"" + f.Length + "\" " : string.Empty,
                f.SimpleType == "decimal" && f.Precision > 0 ? "precision=\"" + f.Precision + "\" " : string.Empty,
                f.SimpleType == "decimal" && f.Scale > 0 ? "scale=\"" + f.Scale + "\" " : string.Empty,
                f.FieldType.HasFlag(FieldType.PrimaryKey) || f.FieldType.HasFlag(FieldType.MasterKey) ? "primary-key=\"true\" " : string.Empty,
                f.IsQuoted() ? string.Format("quoted-with=\"{0}\"", HttpUtility.HtmlEncode(f.QuotedWith)) : string.Empty);
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