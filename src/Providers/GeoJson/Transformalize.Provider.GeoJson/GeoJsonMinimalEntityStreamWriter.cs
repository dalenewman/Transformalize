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
using Transformalize.Configuration;
using Transformalize.Contracts;

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

         var textWriter = new StreamWriter(_stream);
         var jw = new JsonTextWriter(textWriter);

         jw.WriteStartObjectAsync().ConfigureAwait(false); //root

         jw.WritePropertyNameAsync("type").ConfigureAwait(false);
         jw.WriteValueAsync("FeatureCollection").ConfigureAwait(false);

         jw.WritePropertyNameAsync("features").ConfigureAwait(false);
         jw.WriteStartArrayAsync().ConfigureAwait(false);  //features

         foreach (var row in rows) {

            jw.WriteStartObjectAsync().ConfigureAwait(false); //feature
            jw.WritePropertyNameAsync("type").ConfigureAwait(false);
            jw.WriteValueAsync("Feature").ConfigureAwait(false);
            jw.WritePropertyNameAsync("geometry").ConfigureAwait(false);
            jw.WriteStartObjectAsync().ConfigureAwait(false); //geometry 
            jw.WritePropertyNameAsync("type").ConfigureAwait(false);
            jw.WriteValueAsync("Point").ConfigureAwait(false);

            jw.WritePropertyNameAsync("coordinates").ConfigureAwait(false);
            jw.WriteStartArrayAsync().ConfigureAwait(false);
            jw.WriteValueAsync(row[_longitudeField]).ConfigureAwait(false);
            jw.WriteValueAsync(row[_latitudeField]).ConfigureAwait(false);
            jw.WriteEndArrayAsync().ConfigureAwait(false);

            jw.WriteEndObjectAsync().ConfigureAwait(false); //geometry

            jw.WritePropertyNameAsync("properties").ConfigureAwait(false);
            jw.WriteStartObjectAsync().ConfigureAwait(false); //properties

            jw.WritePropertyNameAsync("description").ConfigureAwait(false);
            if (_hasDescription) {
               jw.WriteValueAsync(row[_descriptionField]).ConfigureAwait(false);
            } else {
               jw.WriteValueAsync("add geojson-description to output").ConfigureAwait(false);
            }

            if (_hasBatchValue) {
               jw.WritePropertyNameAsync("batch-value").ConfigureAwait(false);
               jw.WriteValueAsync(row[_batchField]).ConfigureAwait(false);
            }

            if (_hasColor) {
               jw.WritePropertyNameAsync("marker-color").ConfigureAwait(false);
               jw.WriteValueAsync(row[_colorField]).ConfigureAwait(false);
            }

            if (_hasSymbol) {
               var symbol = row[_symbolField].ToString();
               jw.WritePropertyNameAsync("marker-symbol").ConfigureAwait(false);
               jw.WriteValueAsync(symbol).ConfigureAwait(false);
            }

            jw.WriteEndObjectAsync().ConfigureAwait(false); //properties

            jw.WriteEndObjectAsync().ConfigureAwait(false); //feature

            _context.Entity.Inserts++;

            jw.FlushAsync().ConfigureAwait(false);
         }

         jw.WriteEndArrayAsync().ConfigureAwait(false); //features

         jw.WriteEndObjectAsync().ConfigureAwait(false); //root
         jw.FlushAsync().ConfigureAwait(false);

      }
   }
}
