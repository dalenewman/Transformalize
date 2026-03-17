#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2022 Dale Newman
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {

   /// <summary>
   /// It writes data as GeoJson to a stream, converting non-geojson stuff to html in the description
   /// </summary>
   public class GeoJsonStreamWriter : IWrite {

      private readonly Stream _stream;
      private readonly Field _latitudeField;
      private readonly Field _longitudeField;
      private readonly Field _colorField;
      private readonly Field _sizeField;
      private readonly Field _symbolField;
      private readonly Field[] _propertyFields;
      private readonly bool _hasStyle;
      private readonly IContext _context;
      private readonly Dictionary<string, string> _scales = new Dictionary<string, string> {
            {"0.75","small"},
            {"1.0","medium"},
            {"1.25","large"}
        };
      private readonly HashSet<string> _sizes = new HashSet<string> { "small", "medium", "large" };

      /// <summary>
      /// Prepare to write GeoJson to a stream
      /// </summary>
      /// <param name="context">a transformalize context</param>
      /// <param name="stream">whatever stream you want to write to</param>
      public GeoJsonStreamWriter(IContext context, Stream stream) {
         _stream = stream;
         _context = context;
         var fields = context.GetAllEntityFields().ToArray();

         _latitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
         _longitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
         _colorField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-color") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "color");
         _sizeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-size") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "size");
         _symbolField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-symbol") ?? fields.FirstOrDefault(f => f.Alias.ToLower() == "symbol");
         _hasStyle = _colorField != null || _sizeField != null || _symbolField != null;
         _propertyFields = fields.Where(f => f.Output && !f.System && !f.Alias.ToLower().StartsWith("kml-") || f.Alias == "BatchValue").Except(new[] { _latitudeField, _longitudeField, _colorField, _sizeField, _symbolField }).ToArray();
      }

      /// <summary>
      /// Write rows to as GeoJson to a stream
      /// </summary>
      /// <param name="rows"></param>
      public void Write(IEnumerable<IRow> rows) {
         var options = new JsonWriterOptions { Indented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
         using (var jw = new Utf8JsonWriter(_stream, options)) {
            WriteCore(jw, rows);
            jw.Flush();
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         var options = new JsonWriterOptions { Indented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
         var jw = new Utf8JsonWriter(_stream, options);
         WriteCoreSync(jw, rows, token);
         await jw.FlushAsync(token).ConfigureAwait(false);
      }

      private void WriteCore(Utf8JsonWriter jw, IEnumerable<IRow> rows) {
         jw.WriteStartObject(); //root

         jw.WritePropertyName("type");
         jw.WriteStringValue("FeatureCollection");

         jw.WritePropertyName("features");
         jw.WriteStartArray(); //features

         var tableBuilder = new StringBuilder();

         foreach (var row in rows) {
            jw.WriteStartObject(); //feature
            jw.WritePropertyName("type");
            jw.WriteStringValue("Feature");
            jw.WritePropertyName("geometry");
            jw.WriteStartObject(); //geometry
            jw.WritePropertyName("type");
            jw.WriteStringValue("Point");

            jw.WritePropertyName("coordinates");
            jw.WriteStartArray();
            WriteValue(jw, row[_longitudeField]);
            WriteValue(jw, row[_latitudeField]);
            jw.WriteEndArray();

            jw.WriteEndObject(); //geometry

            jw.WritePropertyName("properties");
            jw.WriteStartObject(); //properties

            foreach (var field in _propertyFields) {
               jw.WritePropertyName(field.Label);
               WriteValue(jw, field.Format == string.Empty ? row[field] : string.Format(string.Concat("{0:", field.Format, "}"), row[field]));
            }

            jw.WritePropertyName("description");
            tableBuilder.Clear();
            tableBuilder.AppendLine("<table class=\"table table-striped table-condensed\">");
            foreach (var field in _propertyFields.Where(f => f.Alias != "BatchValue")) {
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
            jw.WriteStringValue(tableBuilder.ToString());

            WriteStyle(jw, row);

            jw.WriteEndObject(); //properties
            jw.WriteEndObject(); //feature
            _context.Entity.Inserts++;
            jw.Flush();
         }

         jw.WriteEndArray(); //features
         jw.WriteEndObject(); //root
      }

      private void WriteCoreSync(Utf8JsonWriter jw, IEnumerable<IRow> rows, CancellationToken token) {
         jw.WriteStartObject(); //root

         jw.WritePropertyName("type");
         jw.WriteStringValue("FeatureCollection");

         jw.WritePropertyName("features");
         jw.WriteStartArray(); //features

         var tableBuilder = new StringBuilder();

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();

            jw.WriteStartObject(); //feature
            jw.WritePropertyName("type");
            jw.WriteStringValue("Feature");
            jw.WritePropertyName("geometry");
            jw.WriteStartObject(); //geometry
            jw.WritePropertyName("type");
            jw.WriteStringValue("Point");

            jw.WritePropertyName("coordinates");
            jw.WriteStartArray();
            WriteValue(jw, row[_longitudeField]);
            WriteValue(jw, row[_latitudeField]);
            jw.WriteEndArray();

            jw.WriteEndObject(); //geometry

            jw.WritePropertyName("properties");
            jw.WriteStartObject(); //properties

            foreach (var field in _propertyFields) {
               jw.WritePropertyName(field.Label);
               WriteValue(jw, field.Format == string.Empty ? row[field] : string.Format(string.Concat("{0:", field.Format, "}"), row[field]));
            }

            jw.WritePropertyName("description");
            tableBuilder.Clear();
            tableBuilder.AppendLine("<table class=\"table table-striped table-condensed\">");
            foreach (var field in _propertyFields.Where(f => f.Alias != "BatchValue")) {
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
            jw.WriteStringValue(tableBuilder.ToString());

            WriteStyle(jw, row);

            jw.WriteEndObject(); //properties
            jw.WriteEndObject(); //feature
            _context.Entity.Inserts++;
            jw.Flush();
         }

         jw.WriteEndArray(); //features
         jw.WriteEndObject(); //root
      }

      private void WriteStyle(Utf8JsonWriter jw, IRow row) {
         if (!_hasStyle) {
            return;
         }

         if (_colorField != null) {
            jw.WritePropertyName("marker-color");
            var color = row[_colorField].ToString().TrimStart('#').Right(6);
            jw.WriteStringValue("#" + (color.Length == 6 ? color : "0080ff"));
         }

         if (_sizeField != null) {
            jw.WritePropertyName("marker-size");
            var size = row[_sizeField].ToString().ToLower();
            if (_sizes.Contains(size)) {
               jw.WriteStringValue(size);
            } else {
               jw.WriteStringValue(_scales.ContainsKey(size) ? _scales[size] : "medium");
            }
         }

         if (_symbolField != null) {
            var symbol = row[_symbolField].ToString();
            if (symbol.StartsWith("http")) {
               symbol = "marker";
            }
            jw.WritePropertyName("marker-symbol");
            jw.WriteStringValue(symbol);
         }
      }

      private static void WriteValue(Utf8JsonWriter jw, object value) {
         switch (value) {
            case null: jw.WriteNullValue(); break;
            case bool b: jw.WriteBooleanValue(b); break;
            case int i: jw.WriteNumberValue(i); break;
            case long l: jw.WriteNumberValue(l); break;
            case float f: jw.WriteNumberValue(f); break;
            case double d: jw.WriteNumberValue(d); break;
            case decimal dec: jw.WriteNumberValue(dec); break;
            case DateTime dt: jw.WriteStringValue(dt.ToString("o")); break;
            case Guid g: jw.WriteStringValue(g.ToString()); break;
            default: jw.WriteStringValue(value.ToString()); break;
         }
      }
   }
}
