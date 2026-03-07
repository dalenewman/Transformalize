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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.GeoJson {

   /// <summary>
   /// Write a process' output as GeoJson to a stream with an emphasis on a light payload
   /// </summary>
   public class GeoJsonMinimalProcessStreamWriter : IWrite {

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
      private readonly JsonWriter _jw;
      private readonly Field[] _properties;

      /// <summary>
      /// Given a context and a JSON Writer, prepare to write
      /// </summary>
      /// <param name="context"></param>
      /// <param name="jsonWriter"></param>
      public GeoJsonMinimalProcessStreamWriter(IContext context, JsonWriter jsonWriter) {
         _context = context;
         _jw = jsonWriter;

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

         _properties = fields.Where(f => f.Property).Except(new[] { _descriptionField, _colorField, _symbolField, _batchField }.Where(f => f != null)).ToArray();
      }

      /// <summary>
      /// Write transformalize rows as GeoJson to a stream
      /// </summary>
      /// <param name="rows">transformalize rows</param>
      public void Write(IEnumerable<IRow> rows) {
         if (Equals(_context.Process.Entities.First(), _context.Entity)) {
            _jw.WriteStartObject(); //root
            _jw.WritePropertyName("type");
            _jw.WriteValue("FeatureCollection");
            _jw.WritePropertyName("features");
            _jw.WriteStartArray(); //features
         }

         foreach (var row in rows) {
            _jw.WriteStartObject(); //feature
            _jw.WritePropertyName("type");
            _jw.WriteValue("Feature");
            _jw.WritePropertyName("geometry");
            _jw.WriteStartObject(); //geometry
            _jw.WritePropertyName("type");
            _jw.WriteValue("Point");

            _jw.WritePropertyName("coordinates");
            _jw.WriteStartArray();
            _jw.WriteValue(row[_longitudeField]);
            _jw.WriteValue(row[_latitudeField]);
            _jw.WriteEndArray();

            _jw.WriteEndObject(); //geometry

            _jw.WritePropertyName("properties");
            _jw.WriteStartObject(); //properties

            if (_hasDescription) {
               _jw.WritePropertyName("description");
               _jw.WriteValue(row[_descriptionField]);
            }

            if (_hasBatchValue) {
               _jw.WritePropertyName("batch-value");
               _jw.WriteValue(row[_batchField]);
            }

            if (_hasColor) {
               _jw.WritePropertyName("marker-color");
               _jw.WriteValue(row[_colorField]);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               _jw.WritePropertyName("marker-symbol");
               _jw.WriteValue(symbol);
            }

            foreach (var field in _properties) {
               var name = field.Label == string.Empty ? field.Alias : field.Label;
               _jw.WritePropertyName(name);
               _jw.WriteValue(row[field]);
            }

            _jw.WriteEndObject(); //properties
            _jw.WriteEndObject(); //feature
            _context.Entity.Inserts++;
            _jw.Flush();
         }

         if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
            _jw.WriteEndArray(); //features
            _jw.WriteEndObject(); //root
         }

         _jw.Flush();
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         if (Equals(_context.Process.Entities.First(), _context.Entity)) {
            await _jw.WriteStartObjectAsync(token).ConfigureAwait(false); //root
            await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
            await _jw.WriteValueAsync("FeatureCollection", token).ConfigureAwait(false);
            await _jw.WritePropertyNameAsync("features", token).ConfigureAwait(false);
            await _jw.WriteStartArrayAsync(token).ConfigureAwait(false); //features
         }

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();

            await _jw.WriteStartObjectAsync(token).ConfigureAwait(false); //feature
            await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
            await _jw.WriteValueAsync("Feature", token).ConfigureAwait(false);
            await _jw.WritePropertyNameAsync("geometry", token).ConfigureAwait(false);
            await _jw.WriteStartObjectAsync(token).ConfigureAwait(false); //geometry
            await _jw.WritePropertyNameAsync("type", token).ConfigureAwait(false);
            await _jw.WriteValueAsync("Point", token).ConfigureAwait(false);

            await _jw.WritePropertyNameAsync("coordinates", token).ConfigureAwait(false);
            await _jw.WriteStartArrayAsync(token).ConfigureAwait(false);
            await _jw.WriteValueAsync(row[_longitudeField], token).ConfigureAwait(false);
            await _jw.WriteValueAsync(row[_latitudeField], token).ConfigureAwait(false);
            await _jw.WriteEndArrayAsync(token).ConfigureAwait(false);

            await _jw.WriteEndObjectAsync(token).ConfigureAwait(false); //geometry

            await _jw.WritePropertyNameAsync("properties", token).ConfigureAwait(false);
            await _jw.WriteStartObjectAsync(token).ConfigureAwait(false); //properties

            if (_hasDescription) {
               await _jw.WritePropertyNameAsync("description", token).ConfigureAwait(false);
               await _jw.WriteValueAsync(row[_descriptionField], token).ConfigureAwait(false);
            }

            if (_hasBatchValue) {
               await _jw.WritePropertyNameAsync("batch-value", token).ConfigureAwait(false);
               await _jw.WriteValueAsync(row[_batchField], token).ConfigureAwait(false);
            }

            if (_hasColor) {
               await _jw.WritePropertyNameAsync("marker-color", token).ConfigureAwait(false);
               await _jw.WriteValueAsync(row[_colorField], token).ConfigureAwait(false);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               await _jw.WritePropertyNameAsync("marker-symbol", token).ConfigureAwait(false);
               await _jw.WriteValueAsync(symbol, token).ConfigureAwait(false);
            }

            foreach (var field in _properties) {
               var name = field.Label == string.Empty ? field.Alias : field.Label;
               await _jw.WritePropertyNameAsync(name, token).ConfigureAwait(false);
               await _jw.WriteValueAsync(row[field], token).ConfigureAwait(false);
            }

            await _jw.WriteEndObjectAsync(token).ConfigureAwait(false); //properties
            await _jw.WriteEndObjectAsync(token).ConfigureAwait(false); //feature
            _context.Entity.Inserts++;
            await _jw.FlushAsync(token).ConfigureAwait(false);
         }

         if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
            await _jw.WriteEndArrayAsync(token).ConfigureAwait(false); //features
            await _jw.WriteEndObjectAsync(token).ConfigureAwait(false); //root
         }

         await _jw.FlushAsync(token).ConfigureAwait(false);
      }
   }
}
