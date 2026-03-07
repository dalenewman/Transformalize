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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
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
         using (var textWriter = new StreamWriter(_stream, new UTF8Encoding(false), 1024, true))
         using (var jw = new JsonTextWriter(textWriter)) {
            WriteCollectionStart(jw);
            foreach (var row in rows) {
               WriteFeature(jw, row);
               _context.Entity.Inserts++;
               jw.Flush();
            }
            WriteCollectionEnd(jw);
            jw.Flush();
            textWriter.Flush();
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         using (var textWriter = new StreamWriter(_stream, new UTF8Encoding(false), 1024, true))
         using (var jw = new JsonTextWriter(textWriter)) {
            await WriteCollectionStartAsync(jw, token).ConfigureAwait(false);
            foreach (var row in rows) {
               token.ThrowIfCancellationRequested();
               await WriteFeatureAsync(jw, row, token).ConfigureAwait(false);
               _context.Entity.Inserts++;
               await jw.FlushAsync(token).ConfigureAwait(false);
            }
            await WriteCollectionEndAsync(jw, token).ConfigureAwait(false);
            await jw.FlushAsync(token).ConfigureAwait(false);
            await textWriter.FlushAsync().ConfigureAwait(false);
         }
      }

      private void WriteCollectionStart(JsonWriter jw) {
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteValue("FeatureCollection");
         WriteBBox(jw);
         jw.WritePropertyName("features");
         jw.WriteStartArray();
      }

      private async Task WriteCollectionStartAsync(JsonWriter jw, CancellationToken token) {
         await jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await jw.WriteValueAsync("FeatureCollection", token).ConfigureAwait(false);
         await WriteBBoxAsync(jw, token).ConfigureAwait(false);
         await jw.WritePropertyNameAsync("features", token).ConfigureAwait(false);
         await jw.WriteStartArrayAsync(token).ConfigureAwait(false);
      }

      private static void WriteCollectionEnd(JsonWriter jw) {
         jw.WriteEndArray();
         jw.WriteEndObject();
      }

      private static async Task WriteCollectionEndAsync(JsonWriter jw, CancellationToken token) {
         await jw.WriteEndArrayAsync(token).ConfigureAwait(false);
         await jw.WriteEndObjectAsync(token).ConfigureAwait(false);
      }

      private void WriteFeature(JsonWriter jw, IRow row) {
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteValue("Feature");
         if (_map.IdField != null) {
            jw.WritePropertyName("id");
            jw.WriteValue(row[_map.IdField]);
         }

         jw.WritePropertyName("geometry");
         jw.WriteStartObject();
         jw.WritePropertyName("type");
         jw.WriteValue("Point");
         jw.WritePropertyName("coordinates");
         jw.WriteStartArray();
         jw.WriteValue(row[_map.LongitudeField]);
         jw.WriteValue(row[_map.LatitudeField]);
         if (_map.AltitudeField != null) {
            jw.WriteValue(row[_map.AltitudeField]);
         }
         jw.WriteEndArray();
         jw.WriteEndObject();

         jw.WritePropertyName("properties");
         jw.WriteStartObject();
         foreach (var field in _map.PropertyFields) {
            jw.WritePropertyName(GeoJsonRoleMap.PropertyName(field));
            jw.WriteValue(row[field]);
         }
         jw.WriteEndObject();

         jw.WriteEndObject();
      }

      private async Task WriteFeatureAsync(JsonWriter jw, IRow row, CancellationToken token) {
         await jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await jw.WriteValueAsync("Feature", token).ConfigureAwait(false);
         if (_map.IdField != null) {
            await jw.WritePropertyNameAsync("id", token).ConfigureAwait(false);
            await jw.WriteValueAsync(row[_map.IdField], token).ConfigureAwait(false);
         }

         await jw.WritePropertyNameAsync("geometry", token).ConfigureAwait(false);
         await jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await jw.WriteValueAsync("Point", token).ConfigureAwait(false);
         await jw.WritePropertyNameAsync("coordinates", token).ConfigureAwait(false);
         await jw.WriteStartArrayAsync(token).ConfigureAwait(false);
         await jw.WriteValueAsync(row[_map.LongitudeField], token).ConfigureAwait(false);
         await jw.WriteValueAsync(row[_map.LatitudeField], token).ConfigureAwait(false);
         if (_map.AltitudeField != null) {
            await jw.WriteValueAsync(row[_map.AltitudeField], token).ConfigureAwait(false);
         }
         await jw.WriteEndArrayAsync(token).ConfigureAwait(false);
         await jw.WriteEndObjectAsync(token).ConfigureAwait(false);

         await jw.WritePropertyNameAsync("properties", token).ConfigureAwait(false);
         await jw.WriteStartObjectAsync(token).ConfigureAwait(false);
         foreach (var field in _map.PropertyFields) {
            await jw.WritePropertyNameAsync(GeoJsonRoleMap.PropertyName(field), token).ConfigureAwait(false);
            await jw.WriteValueAsync(row[field], token).ConfigureAwait(false);
         }
         await jw.WriteEndObjectAsync(token).ConfigureAwait(false);

         await jw.WriteEndObjectAsync(token).ConfigureAwait(false);
      }

      private void WriteBBox(JsonWriter jw) {
         if (_map.BBox == null) {
            return;
         }

         jw.WritePropertyName("bbox");
         jw.WriteStartArray();
         foreach (var number in _map.BBox) {
            jw.WriteValue(number);
         }
         jw.WriteEndArray();
      }

      private async Task WriteBBoxAsync(JsonWriter jw, CancellationToken token) {
         if (_map.BBox == null) {
            return;
         }

         await jw.WritePropertyNameAsync("bbox", token).ConfigureAwait(false);
         await jw.WriteStartArrayAsync(token).ConfigureAwait(false);
         foreach (var number in _map.BBox) {
            await jw.WriteValueAsync(number, token).ConfigureAwait(false);
         }
         await jw.WriteEndArrayAsync(token).ConfigureAwait(false);
      }
   }
}
