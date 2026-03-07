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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
         using (var textWriter = new StreamWriter(_stream, new UTF8Encoding(false), 1024, true))
         using (var jw = new JsonTextWriter(textWriter)) {
            WriteCore(jw, rows);
            jw.Flush();
            textWriter.Flush();
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         using (var textWriter = new StreamWriter(_stream, new UTF8Encoding(false), 1024, true))
         using (var jw = new JsonTextWriter(textWriter)) {
            await WriteCoreAsync(jw, rows, token).ConfigureAwait(false);
            await jw.FlushAsync(token).ConfigureAwait(false);
            await textWriter.FlushAsync().ConfigureAwait(false);
         }
      }

      private void WriteCore(JsonWriter jw, IEnumerable<IRow> rows) {
         jw.WriteStartObject(); //root

         jw.WritePropertyName("type");
         jw.WriteValue("FeatureCollection");

         jw.WritePropertyName("features");
         jw.WriteStartArray(); //features

         foreach (var row in rows) {
            jw.WriteStartObject(); //feature
            jw.WritePropertyName("type");
            jw.WriteValue("Feature");
            jw.WritePropertyName("geometry");
            jw.WriteStartObject(); //geometry
            jw.WritePropertyName("type");
            jw.WriteValue("Point");

            jw.WritePropertyName("coordinates");
            jw.WriteStartArray();
            jw.WriteValue(row[_longitudeField]);
            jw.WriteValue(row[_latitudeField]);
            jw.WriteEndArray();

            jw.WriteEndObject(); //geometry

            jw.WritePropertyName("properties");
            jw.WriteStartObject(); //properties

            jw.WritePropertyName("description");
            jw.WriteValue(_hasDescription ? row[_descriptionField] : "add geojson-description to output");

            if (_hasBatchValue) {
               jw.WritePropertyName("batch-value");
               jw.WriteValue(row[_batchField]);
            }

            if (_hasColor) {
               jw.WritePropertyName("marker-color");
               jw.WriteValue(row[_colorField]);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               jw.WritePropertyName("marker-symbol");
               jw.WriteValue(symbol);
            }

            jw.WriteEndObject(); //properties
            jw.WriteEndObject(); //feature
            _context.Entity.Inserts++;
            jw.Flush();
         }

         jw.WriteEndArray(); //features
         jw.WriteEndObject(); //root
      }

      private async Task WriteCoreAsync(JsonWriter jw, IEnumerable<IRow> rows, CancellationToken token) {
         await jw.WriteStartObjectAsync(token).ConfigureAwait(false); //root

         await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
         await jw.WriteValueAsync("FeatureCollection", token).ConfigureAwait(false);

         await jw.WritePropertyNameAsync("features", token).ConfigureAwait(false);
         await jw.WriteStartArrayAsync(token).ConfigureAwait(false); //features

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();

            await jw.WriteStartObjectAsync(token).ConfigureAwait(false); //feature
            await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
            await jw.WriteValueAsync("Feature", token).ConfigureAwait(false);
            await jw.WritePropertyNameAsync("geometry", token).ConfigureAwait(false);
            await jw.WriteStartObjectAsync(token).ConfigureAwait(false); //geometry
            await jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
            await jw.WriteValueAsync("Point", token).ConfigureAwait(false);

            await jw.WritePropertyNameAsync("coordinates", token).ConfigureAwait(false);
            await jw.WriteStartArrayAsync(token).ConfigureAwait(false);
            await jw.WriteValueAsync(row[_longitudeField], token).ConfigureAwait(false);
            await jw.WriteValueAsync(row[_latitudeField], token).ConfigureAwait(false);
            await jw.WriteEndArrayAsync(token).ConfigureAwait(false);

            await jw.WriteEndObjectAsync(token).ConfigureAwait(false); //geometry

            await jw.WritePropertyNameAsync("properties", token).ConfigureAwait(false);
            await jw.WriteStartObjectAsync(token).ConfigureAwait(false); //properties

            await jw.WritePropertyNameAsync("description", token).ConfigureAwait(false);
            await jw.WriteValueAsync(_hasDescription ? row[_descriptionField] : "add geojson-description to output", token).ConfigureAwait(false);

            if (_hasBatchValue) {
               await jw.WritePropertyNameAsync("batch-value", token).ConfigureAwait(false);
               await jw.WriteValueAsync(row[_batchField], token).ConfigureAwait(false);
            }

            if (_hasColor) {
               await jw.WritePropertyNameAsync("marker-color", token).ConfigureAwait(false);
               await jw.WriteValueAsync(row[_colorField], token).ConfigureAwait(false);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               await jw.WritePropertyNameAsync("marker-symbol", token).ConfigureAwait(false);
               await jw.WriteValueAsync(symbol, token).ConfigureAwait(false);
            }

            await jw.WriteEndObjectAsync(token).ConfigureAwait(false); //properties
            await jw.WriteEndObjectAsync(token).ConfigureAwait(false); //feature
            _context.Entity.Inserts++;
            await jw.FlushAsync(token).ConfigureAwait(false);
         }

         await jw.WriteEndArrayAsync(token).ConfigureAwait(false); //features
         await jw.WriteEndObjectAsync(token).ConfigureAwait(false); //root
      }
   }
}
