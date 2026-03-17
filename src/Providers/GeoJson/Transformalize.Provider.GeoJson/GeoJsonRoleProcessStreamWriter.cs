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
using System.Linq;
using System.Text.Json;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {
   public class GeoJsonRoleProcessStreamWriter : IWrite {
      private readonly OutputContext _context;
      private readonly Utf8JsonWriter _jw;
      private readonly GeoJsonRoleMap _map;

      public GeoJsonRoleProcessStreamWriter(OutputContext context, Utf8JsonWriter jsonWriter) {
         _context = context;
         _jw = jsonWriter;
         _map = new GeoJsonRoleMap(context);
      }

      public void Write(IEnumerable<IRow> rows) {
         if (Equals(_context.Process.Entities.First(), _context.Entity)) {
            WriteCollectionStart();
         }

         foreach (var row in rows) {
            WriteFeature(row);
            _context.Entity.Inserts++;
            _jw.Flush();
         }

         if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
            WriteCollectionEnd();
         }

         _jw.Flush();
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         if (Equals(_context.Process.Entities.First(), _context.Entity)) {
            WriteCollectionStart();
         }

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            WriteFeature(row);
            _context.Entity.Inserts++;
            await _jw.FlushAsync(token).ConfigureAwait(false);
         }

         if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
            WriteCollectionEnd();
         }

         await _jw.FlushAsync(token).ConfigureAwait(false);
      }

      private void WriteCollectionStart() {
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteStringValue("FeatureCollection");
         WriteBBox();
         _jw.WritePropertyName("features");
         _jw.WriteStartArray();
      }

      private void WriteCollectionEnd() {
         _jw.WriteEndArray();
         _jw.WriteEndObject();
      }

      private void WriteFeature(IRow row) {
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteStringValue("Feature");
         if (_map.IdField != null) {
            _jw.WritePropertyName("id");
            WriteValue(_jw, row[_map.IdField]);
         }

         _jw.WritePropertyName("geometry");
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteStringValue("Point");
         _jw.WritePropertyName("coordinates");
         _jw.WriteStartArray();
         WriteValue(_jw, row[_map.LongitudeField]);
         WriteValue(_jw, row[_map.LatitudeField]);
         if (_map.AltitudeField != null) {
            WriteValue(_jw, row[_map.AltitudeField]);
         }
         _jw.WriteEndArray();
         _jw.WriteEndObject();

         _jw.WritePropertyName("properties");
         _jw.WriteStartObject();
         foreach (var field in _map.PropertyFields) {
            _jw.WritePropertyName(GeoJsonRoleMap.PropertyName(field));
            WriteValue(_jw, row[field]);
         }
         _jw.WriteEndObject();

         _jw.WriteEndObject();
      }

      private void WriteBBox() {
         if (_map.BBox == null) {
            return;
         }

         _jw.WritePropertyName("bbox");
         _jw.WriteStartArray();
         foreach (var number in _map.BBox) {
            _jw.WriteNumberValue(number);
         }
         _jw.WriteEndArray();
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
