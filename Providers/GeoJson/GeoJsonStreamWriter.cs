#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.GeoJson {

    public class GeoJsonStreamWriter : IWrite {

        private readonly Stream _stream;
        private readonly Field _latitudeField;
        private readonly Field _longitudeField;
        private readonly Field _colorField;
        private readonly Field _sizeField;
        private readonly Field _symbolField;
        private readonly Field[] _propertyFields;
        private readonly bool _hasStyle;
        private readonly Dictionary<string, string> _scales = new Dictionary<string, string> {
            {"0.75","small"},
            {"1.0","medium"},
            {"1.25","large"}
        };
        private readonly HashSet<string> _sizes = new HashSet<string> { "small", "medium", "large" };

        public GeoJsonStreamWriter(IContext context, Stream stream) {
            _stream = stream;
            var fields = context.GetAllEntityFields().ToArray();

            _latitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
            _longitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
            _colorField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-color") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "color");
            _sizeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-size") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "size");
            _symbolField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-symbol") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "symbol");
            _hasStyle = _colorField != null || _sizeField != null || _symbolField != null;
            _propertyFields = fields.Where(f => f.Output && !f.System && !f.Alias.ToLower().StartsWith("kml-") || f.Alias == "BatchValue").Except(new[] { _latitudeField, _longitudeField, _colorField, _sizeField, _symbolField }).ToArray();

        }

        public void Write(IEnumerable<IRow> rows) {

            var textWriter = new StreamWriter(_stream);
            var jsonWriter = new JsonTextWriter(textWriter);

            jsonWriter.WriteStartObject(); //root

            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("FeatureCollection");

            jsonWriter.WritePropertyName("features");
            jsonWriter.WriteStartArray();  //features

            var tableBuilder = new StringBuilder();

            foreach (var row in rows) {

                jsonWriter.WriteStartObject(); //feature
                jsonWriter.WritePropertyName("type");
                jsonWriter.WriteValue("Feature");
                jsonWriter.WritePropertyName("geometry");
                jsonWriter.WriteStartObject(); //geometry 
                jsonWriter.WritePropertyName("type");
                jsonWriter.WriteValue("Point");

                jsonWriter.WritePropertyName("coordinates");
                jsonWriter.WriteStartArray();
                jsonWriter.WriteValue(row[_longitudeField]);
                jsonWriter.WriteValue(row[_latitudeField]);
                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject(); //geometry

                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteStartObject(); //properties

                foreach (var field in _propertyFields) {
                    jsonWriter.WritePropertyName(field.Label);
                    jsonWriter.WriteValue(field.Format == string.Empty ? row[field] : string.Format(string.Concat("{0:", field.Format, "}"), row[field]));
                }

                jsonWriter.WritePropertyName("description");
                tableBuilder.Clear();
                tableBuilder.AppendLine("<table class=\"table table-striped table-condensed\">");
                foreach (var field in _propertyFields.Where(f=>f.Alias != "BatchValue")) {
                    tableBuilder.AppendLine("<tr>");

                    tableBuilder.AppendLine("<td><strong>");
                    tableBuilder.AppendLine(field.Label);
                    tableBuilder.AppendLine(":</strong></td>");

                    tableBuilder.AppendLine("<td>");
                    tableBuilder.AppendLine(field.Raw ? row[field].ToString() : System.Security.SecurityElement.Escape(row[field].ToString()));
                    tableBuilder.AppendLine("</td>");

                    tableBuilder.AppendLine("</tr>");
                }
                tableBuilder.AppendLine("</table>");
                jsonWriter.WriteValue(tableBuilder.ToString());

                if (_hasStyle) {
                    if (_colorField != null) {
                        jsonWriter.WritePropertyName("marker-color");
                        var color = row[_colorField].ToString().TrimStart('#').Right(6);
                        jsonWriter.WriteValue("#" + (color.Length == 6 ? color : "0080ff"));
                    }

                    if (_sizeField != null) {
                        jsonWriter.WritePropertyName("marker-size");
                        var size = row[_sizeField].ToString().ToLower();
                        if (_sizes.Contains(size)) {
                            jsonWriter.WriteValue(size);
                        } else {
                            jsonWriter.WriteValue(_scales.ContainsKey(size) ? _scales[size] : "medium");
                        }
                    }

                    if (_symbolField != null) {
                        var symbol = row[_symbolField].ToString();
                        if (symbol.StartsWith("http")) {
                            symbol = "marker";
                        }
                        jsonWriter.WritePropertyName("marker-symbol");
                        jsonWriter.WriteValue(symbol);
                    }

                }

                jsonWriter.WriteEndObject(); //properties

                jsonWriter.WriteEndObject(); //feature
            }

            jsonWriter.WriteEndArray(); //features

            jsonWriter.WriteEndObject(); //root
            jsonWriter.Flush();

        }
    }
}
