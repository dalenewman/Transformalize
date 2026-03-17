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
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {

   /// <summary>
   /// Write an entity's output as GeoJson to a stream with an emphasis on a light payload
   /// </summary>
   public class GeoJsonMinimalEntityStreamWriter : IWrite {

      private readonly Stream _stream;
      private readonly Field _latitudeField;
      private readonly Field _longitudeField;
      private readonly Field _colorField;
      private readonly Field _symbolField;

      private readonly Field _descriptionField;
      private readonly Field _batchField;
      private readonly bool _hasColor;
      private readonly bool _hasSymbol;
      private readonly bool _hasDescription;
      private readonly bool _hasBatchValue;
      private readonly IContext _context;

      /// <summary>
      /// Given a context, and a stream, prepare to write GeoJson
      /// </summary>
      /// <param name="context">a transformalize context</param>
      /// <param name="stream">a stream to write to</param>
      public GeoJsonMinimalEntityStreamWriter(IContext context, Stream stream) {
         _context = context;
         _stream = stream;
         var fields = context.GetAllEntityFields().ToArray();

         _latitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
         _longitudeField = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
         _colorField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-color");
         _symbolField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-symbol");

         _descriptionField = fields.FirstOrDefault(f => f.Alias.ToLower() == "geojson-description");
         _batchField = fields.FirstOrDefault(f => f.Alias.ToLower() == "batchvalue");

         _hasColor = _colorField != null;
         _hasSymbol = _symbolField != null;
         _hasDescription = _descriptionField != null;
         _hasBatchValue = _batchField != null;
      }

      /// <summary>
      /// Write rows to GeoJson stream
      /// </summary>
      /// <param name="rows">transformalize rows</param>
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

            jw.WritePropertyName("description");
            WriteValue(jw, _hasDescription ? row[_descriptionField] : "add geojson-description to output");

            if (_hasBatchValue) {
               jw.WritePropertyName("batch-value");
               WriteValue(jw, row[_batchField]);
            }

            if (_hasColor) {
               jw.WritePropertyName("marker-color");
               WriteValue(jw, row[_colorField]);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               jw.WritePropertyName("marker-symbol");
               jw.WriteStringValue(symbol);
            }

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

            jw.WritePropertyName("description");
            WriteValue(jw, _hasDescription ? row[_descriptionField] : "add geojson-description to output");

            if (_hasBatchValue) {
               jw.WritePropertyName("batch-value");
               WriteValue(jw, row[_batchField]);
            }

            if (_hasColor) {
               jw.WritePropertyName("marker-color");
               WriteValue(jw, row[_colorField]);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               jw.WritePropertyName("marker-symbol");
               jw.WriteStringValue(symbol);
            }

            jw.WriteEndObject(); //properties
            jw.WriteEndObject(); //feature
            _context.Entity.Inserts++;
            jw.Flush();
         }

         jw.WriteEndArray(); //features
         jw.WriteEndObject(); //root
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
