#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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

        public void Write(IEnumerable<IRow> rows) {

            var textWriter = new StreamWriter(_stream);
            var jsonWriter = new JsonTextWriter(textWriter);

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

                jsonWriter.WritePropertyName("description");
                if (_hasDescription) {
                    jsonWriter.WriteValue(row[_descriptionField]);
                } else {
                    jsonWriter.WriteValue("add geojson-description to output");
                }

                if (_hasBatchValue) {
                    jsonWriter.WritePropertyName("batch-value");
                    jsonWriter.WriteValue(row[_batchField]);
                }

                if (_hasColor) {
                    jsonWriter.WritePropertyName("marker-color");
                    jsonWriter.WriteValue(row[_colorField]);
                }

                if (_hasSymbol) {
                    var symbol = row[_symbolField].ToString();
                    jsonWriter.WritePropertyName("marker-symbol");
                    jsonWriter.WriteValue(symbol);
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
