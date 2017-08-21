#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Xml;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Kml {
    public class KmlStreamWriter : IWrite {

        private readonly Field _latitudeField;
        private readonly Field _longitudeField;
        private readonly Field _colorField;
        private readonly Field _sizeField;
        private readonly Field _symbolField;
        private readonly Field[] _propertyFields;
        private readonly XmlWriter _xmlWriter;
        private readonly Field _nameField;
        private readonly bool _hasStyle;
        private readonly Dictionary<string, string> _sizes = new Dictionary<string, string> {
            {"small","0.75"},
            {"medium","1.0"},
            {"large","1.25"}
        };

        public string DefaultOpacity { get; set; } = "C9";

        public KmlStreamWriter(IContext context, Stream stream) {
            var fields = context.GetAllEntityFields().ToArray();

            _latitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
            _longitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
            _colorField = fields.FirstOrDefault(f => f.Alias.ToLower() == "kml-color") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "color");
            _sizeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "kml-size") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "size");
            _symbolField = fields.FirstOrDefault(f => f.Alias.ToLower() == "kml-symbol") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "symbol");
            _hasStyle = _colorField != null || _sizeField != null || _symbolField != null;
            _propertyFields = fields.Where(f => f.Output && !f.System && !f.Alias.ToLower().StartsWith("geojson-")).Except(new[] { _latitudeField, _longitudeField, _colorField, _sizeField, _symbolField }).ToArray();
            _nameField = _propertyFields.FirstOrDefault(f => f.Alias.ToLower() == "name");
            _xmlWriter = XmlWriter.Create(stream);
        }

        public void Write(IEnumerable<IRow> rows) {

            var tableBuilder = new StringBuilder();

            _xmlWriter.WriteStartElement(string.Empty, "kml", "http://www.opengis.net/kml/2.2");
            _xmlWriter.WriteStartElement("Document");

            foreach (var row in rows) {
                var coordinates = $"{row[_longitudeField]},{row[_latitudeField]},0";
                _xmlWriter.WriteStartElement("Placemark");

                if (_nameField != null) {
                    _xmlWriter.WriteStartElement("name");
                    _xmlWriter.WriteString(row[_nameField].ToString());
                    _xmlWriter.WriteEndElement(); //name
                }

                _xmlWriter.WriteStartElement("description");
                tableBuilder.Clear();
                tableBuilder.AppendLine("<table>");
                foreach (var field in _propertyFields) {
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
                _xmlWriter.WriteCData(tableBuilder.ToString());
                _xmlWriter.WriteEndElement(); //description

                if (_hasStyle) {
                    _xmlWriter.WriteStartElement("Style");
                    _xmlWriter.WriteStartElement("IconStyle");
                    if (_colorField != null) {

                        var color = row[_colorField].ToString().Trim('#');
                        if (color.Length == 6) {
                            color = DefaultOpacity + color;
                        }
                        _xmlWriter.WriteElementString("color", color);
                    }
                    if (_symbolField != null) {
                        // e.g. http://maps.google.com/mapfiles/kml/pushpin/wht-pushpin.png
                        var marker = row[_symbolField].ToString();
                        if (marker.StartsWith("http")) {
                            _xmlWriter.WriteStartElement("Icon");
                            _xmlWriter.WriteElementString("href", row[_symbolField].ToString());
                            _xmlWriter.WriteEndElement();//Icon
                        }
                    }
                    if (_sizeField != null) {
                        var size = row[_sizeField].ToString().ToLower();
                        if (_sizes.ContainsKey(size)) {
                            size = _sizes[size];
                        }
                        _xmlWriter.WriteElementString("scale", size);
                    }
                    _xmlWriter.WriteEndElement(); //IconStyle
                    _xmlWriter.WriteEndElement(); //Style
                }

                _xmlWriter.WriteStartElement("Point");
                _xmlWriter.WriteStartElement("coordinates");
                _xmlWriter.WriteString(coordinates);
                _xmlWriter.WriteEndElement(); //coordinates
                _xmlWriter.WriteEndElement(); //Point

                _xmlWriter.WriteEndElement(); //Placemark
            }

            _xmlWriter.WriteEndElement(); //Document
            _xmlWriter.WriteEndElement();//kml
            _xmlWriter.Flush();

        }

    }
}
