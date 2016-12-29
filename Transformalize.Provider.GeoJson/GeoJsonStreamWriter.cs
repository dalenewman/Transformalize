using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.GeoJson {

    public class GeoJsonStreamWriter : IWrite {

        private readonly Stream _stream;
        private readonly Field _latitudeField;
        private readonly Field _longitudeField;
        private readonly Field _titleField;
        private readonly Field _markerColorField;
        private readonly Field _markerSizeField;
        private readonly Field _markerSymbolField;
        private readonly OutputContext _context;

        public GeoJsonStreamWriter(OutputContext context, Stream stream) {
            _context = context;
            _stream = stream;
            _latitudeField = context.OutputFields.FirstOrDefault(f => f.Alias == "latitude") ?? context.OutputFields.FirstOrDefault(f => f.Alias.StartsWith("lat"));
            _longitudeField = context.OutputFields.FirstOrDefault(f => f.Alias == "longitude") ?? context.OutputFields.FirstOrDefault(f => f.Alias.StartsWith("lon"));
            _titleField = context.OutputFields.FirstOrDefault(f => f.Alias == "title") ?? context.OutputFields.FirstOrDefault(f => f.Alias == "name");
            _markerColorField = context.OutputFields.FirstOrDefault(f => f.Alias == "markercolor") ?? context.OutputFields.FirstOrDefault(f => f.Alias == "color");
            _markerSizeField = context.OutputFields.FirstOrDefault(f => f.Alias == "markersize") ?? context.OutputFields.FirstOrDefault(f => f.Alias == "size");
            _markerSymbolField = context.OutputFields.FirstOrDefault(f => f.Alias == "markersymbol") ?? context.OutputFields.FirstOrDefault(f => f.Alias == "symbol");
        }

        public void Write(IEnumerable<IRow> rows) {

            var textWriter = new StreamWriter(_stream);
            var jsonWriter = new JsonTextWriter(textWriter);
            var tableBuilder = new StringBuilder();

            jsonWriter.WriteStartObject(); //root

            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("FeatureCollection");

            jsonWriter.WritePropertyName("features");
            jsonWriter.WriteStartArray();  //features

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

                jsonWriter.WritePropertyName("title");
                if (_titleField == null) {
                    jsonWriter.WriteValue($"{row[_latitudeField]},{row[_longitudeField]}");
                } else {
                    jsonWriter.WriteValue(row[_titleField]);
                }

                jsonWriter.WritePropertyName("description");

                tableBuilder.Clear();
                tableBuilder.AppendLine("<table class=\"table\">");
                foreach (var field in _context.OutputFields) {
                    tableBuilder.AppendLine("<tr>");

                    tableBuilder.AppendLine("<td><strong>");
                    tableBuilder.AppendLine(field.Label);
                    tableBuilder.AppendLine(":</strong></td>");

                    tableBuilder.AppendLine("<td>");
                    var value = field.Format == string.Empty ? row[field].ToString() : string.Format(string.Concat("{0:", field.Format, "}"), row[field]);
                    tableBuilder.AppendLine(field.Raw ? value : System.Security.SecurityElement.Escape(value));
                    tableBuilder.AppendLine("</td>");

                    tableBuilder.AppendLine("</tr>");
                }
                tableBuilder.AppendLine("</table>");
                jsonWriter.WriteValue(tableBuilder.ToString());

                if (_markerColorField != null) {
                    jsonWriter.WritePropertyName("marker-color");
                    jsonWriter.WriteValue("#" + row[_markerColorField].ToString().TrimStart('#'));
                }

                if (_markerSizeField != null) {
                    jsonWriter.WritePropertyName("marker-size");
                    jsonWriter.WriteValue(row[_markerSizeField]);
                }

                if (_markerSymbolField != null) {
                    jsonWriter.WritePropertyName("marker-symbol");
                    jsonWriter.WriteValue(row[_markerSymbolField]);
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
