#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using System.Text.Encodings.Web;
using System.Text.Json;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {
   public class GeoJsonRoleStreamWriter : IWrite {
      private readonly Stream _stream;
      private readonly OutputContext _context;
      private readonly GeoJsonRoleMap _map;

      public GeoJsonRoleStreamWriter(OutputContext context, Stream stream) {
         _stream = stream;
         _context = context;
         _map = new GeoJsonRoleMap(context);
      }

      public void Write(IEnumerable<IRow> rows) {
         var options = new JsonWriterOptions { Indented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
         using (var jw = new Utf8JsonWriter(_stream, options)) {
            WriteCollectionStart(jw);
            foreach (var row in rows) {
               WriteFeature(jw, row);
               _context.Entity.Inserts++;
               jw.Flush();
            }
            WriteCollectionEnd(jw);
            jw.Flush();
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         var options = new JsonWriterOptions { Indented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
         var jw = new Utf8JsonWriter(_stream, options);
         WriteCollectionStart(jw);
         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            WriteFeature(jw, row);
            _context.Entity.Inserts++;
            jw.Flush();
         }
         WriteCollectionEnd(jw);
         await jw.FlushAsync(token).ConfigureAwait(false);
      }

      private void WriteCollectionStart(Utf8JsonWriter jw) {
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteStringValue("FeatureCollection");
         WriteBBox(jw);
         jw.WritePropertyName("features");
         jw.WriteStartArray();
      }

      private static void WriteCollectionEnd(Utf8JsonWriter jw) {
         jw.WriteEndArray();
         jw.WriteEndObject();
      }

      private void WriteFeature(Utf8JsonWriter jw, IRow row) {
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteStringValue("Feature");
         if (_map.IdField != null) {
            jw.WritePropertyName("id");
            WriteValue(jw, row[_map.IdField]);
         }

         jw.WritePropertyName("geometry");
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteStringValue("Point");
         jw.WritePropertyName("coordinates");
         jw.WriteStartArray();
         WriteValue(jw, row[_map.LongitudeField]);
         WriteValue(jw, row[_map.LatitudeField]);
         if (_map.AltitudeField != null) {
            WriteValue(jw, row[_map.AltitudeField]);
         }
         jw.WriteEndArray();
         jw.WriteEndObject();

         jw.WritePropertyName("properties");
         jw.WriteStartObject();
         foreach (var field in _map.PropertyFields) {
            jw.WritePropertyName(GeoJsonRoleMap.PropertyName(field));
            WriteValue(jw, row[field]);
         }
         jw.WriteEndObject();

         jw.WriteEndObject();
      }

      private void WriteBBox(Utf8JsonWriter jw) {
         if (_map.BBox == null) {
            return;
         }

         jw.WritePropertyName("bbox");
         jw.WriteStartArray();
         foreach (var number in _map.BBox) {
            jw.WriteNumberValue(number);
         }
         jw.WriteEndArray();
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
