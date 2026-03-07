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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {
   public class GeoJsonRoleProcessStreamWriter : IWrite {
      private readonly OutputContext _context;
      private readonly JsonWriter _jw;
      private readonly GeoJsonRoleMap _map;

      public GeoJsonRoleProcessStreamWriter(OutputContext context, JsonWriter jsonWriter) {
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
            await WriteCollectionStartAsync(token).ConfigureAwait(false);
         }

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            await WriteFeatureAsync(row, token).ConfigureAwait(false);
            _context.Entity.Inserts++;
            await _jw.FlushAsync(token).ConfigureAwait(false);
         }

         if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
            await WriteCollectionEndAsync(token).ConfigureAwait(false);
         }

         await _jw.FlushAsync(token).ConfigureAwait(false);
      }

      private void WriteCollectionStart() {
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteValue("FeatureCollection");
         WriteBBox();
         _jw.WritePropertyName("features");
         _jw.WriteStartArray();
      }

      private async Task WriteCollectionStartAsync(CancellationToken token) {
         await _jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await _jw.WriteValueAsync("FeatureCollection", token).ConfigureAwait(false);
         await WriteBBoxAsync(token).ConfigureAwait(false);
         await _jw.WritePropertyNameAsync("features", token).ConfigureAwait(false);
         await _jw.WriteStartArrayAsync(token).ConfigureAwait(false);
      }

      private void WriteCollectionEnd() {
         _jw.WriteEndArray();
         _jw.WriteEndObject();
      }

      private async Task WriteCollectionEndAsync(CancellationToken token) {
         await _jw.WriteEndArrayAsync(token).ConfigureAwait(false);
         await _jw.WriteEndObjectAsync(token).ConfigureAwait(false);
      }

      private void WriteFeature(IRow row) {
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteValue("Feature");
         if (_map.IdField != null) {
            _jw.WritePropertyName("id");
            _jw.WriteValue(row[_map.IdField]);
         }

         _jw.WritePropertyName("geometry");
         _jw.WriteStartObject();
         _jw.WritePropertyName("type");
         _jw.WriteValue("Point");
         _jw.WritePropertyName("coordinates");
         _jw.WriteStartArray();
         _jw.WriteValue(row[_map.LongitudeField]);
         _jw.WriteValue(row[_map.LatitudeField]);
         if (_map.AltitudeField != null) {
            _jw.WriteValue(row[_map.AltitudeField]);
         }
         _jw.WriteEndArray();
         _jw.WriteEndObject();

         _jw.WritePropertyName("properties");
         _jw.WriteStartObject();
         foreach (var field in _map.PropertyFields) {
            _jw.WritePropertyName(GeoJsonRoleMap.PropertyName(field));
            _jw.WriteValue(row[field]);
         }
         _jw.WriteEndObject();

         _jw.WriteEndObject();
      }

      private async Task WriteFeatureAsync(IRow row, CancellationToken token) {
         await _jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await _jw.WriteValueAsync("Feature", token).ConfigureAwait(false);
         if (_map.IdField != null) {
            await _jw.WritePropertyNameAsync("id", token).ConfigureAwait(false);
            await _jw.WriteValueAsync(row[_map.IdField], token).ConfigureAwait(false);
         }

         await _jw.WritePropertyNameAsync("geometry", token).ConfigureAwait(false);
         await _jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await _jw.WriteValueAsync("Point", token).ConfigureAwait(false);
         await _jw.WritePropertyNameAsync("coordinates", token).ConfigureAwait(false);
         await _jw.WriteStartArrayAsync(token).ConfigureAwait(false);
         await _jw.WriteValueAsync(row[_map.LongitudeField], token).ConfigureAwait(false);
         await _jw.WriteValueAsync(row[_map.LatitudeField], token).ConfigureAwait(false);
         if (_map.AltitudeField != null) {
            await _jw.WriteValueAsync(row[_map.AltitudeField], token).ConfigureAwait(false);
         }
         await _jw.WriteEndArrayAsync(token).ConfigureAwait(false);
         await _jw.WriteEndObjectAsync(token).ConfigureAwait(false);

         await _jw.WritePropertyNameAsync("properties", token).ConfigureAwait(false);
         await _jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         foreach (var field in _map.PropertyFields) {
            await _jw.WritePropertyNameAsync(GeoJsonRoleMap.PropertyName(field), token).ConfigureAwait(false);
            await _jw.WriteValueAsync(row[field], token).ConfigureAwait(false);
         }
         await _jw.WriteEndObjectAsync(token).ConfigureAwait(false);

         await _jw.WriteEndObjectAsync(token).ConfigureAwait(false);
      }

      private void WriteBBox() {
         if (_map.BBox == null) {
            return;
         }

         _jw.WritePropertyName("bbox");
         _jw.WriteStartArray();
         foreach (var number in _map.BBox) {
            _jw.WriteValue(number);
         }
         _jw.WriteEndArray();
      }

      private async Task WriteBBoxAsync(CancellationToken token) {
         if (_map.BBox == null) {
            return;
         }

         await _jw.WritePropertyNameAsync("bbox", token).ConfigureAwait(false);
         await _jw.WriteStartArrayAsync(token).ConfigureAwait(false);
         foreach (var number in _map.BBox) {
            await _jw.WriteValueAsync(number, token).ConfigureAwait(false);
         }
         await _jw.WriteEndArrayAsync(token).ConfigureAwait(false);
      }
   }
}
