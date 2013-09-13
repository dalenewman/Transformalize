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

using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.NLog;

namespace Transformalize.Main
{
    public class MetaDataWriter
    {
        private readonly IEntityAutoFieldReader _entityAutoFieldReader;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;

        public MetaDataWriter(Process process, IEntityAutoFieldReader entityAutoFieldReader)
        {
            _process = process;
            _entityAutoFieldReader = entityAutoFieldReader;
        }

        public string Write()
        {
            var content = new StringBuilder();
            content.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            content.AppendLine("<process>");
            content.AppendLine("  <entities>");

            var count = 0;
            foreach (var entity in _process.Entities)
            {
                var fields = _entityAutoFieldReader.Read(entity, count == 0);
                content.AppendFormat("    <add name=\"{0}\">\r\n", entity.Name);
                AppendPrimaryKey(fields, entity, content);
                AppendFields(fields, content);
                content.AppendLine("    </add>");
                count++;
            }

            content.AppendLine("  </entities>");
            content.AppendLine("</process>");
            return content.ToString();
        }

        private void AppendFields(IFields all, StringBuilder content)
        {
            var fields = new FieldSqlWriter(all).FieldType(FieldType.Field).Context();
            _log.Debug("Entity auto found {0} field{1}.", fields.Count, fields.Count == 1 ? string.Empty : "s");

            content.AppendLine("      <fields>");
            foreach (var f in fields)
            {
                AppendField(content, f);
            }
            content.AppendLine("      </fields>");
        }

        private void AppendPrimaryKey(IFields all, Entity entity, StringBuilder content)
        {
            var keys = new FieldSqlWriter(all).FieldType(FieldType.PrimaryKey, FieldType.MasterKey).Context();
            if (!keys.Any())
            {
                _log.Warn(
                    "Entity auto could not find a primary key on {0}.  You will need to define one in <fields><primaryKey> element.",
                    entity.Name);
            }
            content.AppendLine("      <primaryKey>");
            foreach (var f in keys)
            {
                AppendField(content, f);
            }
            content.AppendLine("      </primaryKey>");
        }

        private static void AppendField(StringBuilder content, KeyValuePair<string, Field> f)
        {
            content.AppendFormat("        <add name=\"{0}\" {1}{2}{3}{4}></add>\r\n",
                                 f.Value.Name,
                                 f.Value.SimpleType.Equals("string") ? string.Empty : "type=\"" + f.Value.Type + "\" ",
                                 f.Value.Length != "0" ? "length=\"" + f.Value.Length + "\" " : string.Empty,
                                 f.Value.SimpleType == "decimal" && f.Value.Precision > 0
                                     ? "precision=\"" + f.Value.Precision + "\" "
                                     : string.Empty,
                                 f.Value.SimpleType == "decimal" && f.Value.Scale > 0
                                     ? "scale=\"" + f.Value.Scale + "\""
                                     : string.Empty);
        }
    }
}