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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.GeoJson {

    public class GeoJsonMinimalProcessStreamWriter : IWrite {

        private readonly JsonTextWriter _writer;
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

        public GeoJsonMinimalProcessStreamWriter(IContext context, JsonTextWriter writer) {
            _context = context;
            _writer = writer;
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

            if (Equals(_context.Process.Entities.First(), _context.Entity)) {
                _writer.WriteStartObject(); //root

                _writer.WritePropertyName("type");
                _writer.WriteValue("FeatureCollection");

                _writer.WritePropertyName("features");
                _writer.WriteStartArray();  //features
            }

            foreach (var row in rows) {

                _writer.WriteStartObject(); //feature
                _writer.WritePropertyName("type");
                _writer.WriteValue("Feature");
                _writer.WritePropertyName("geometry");
                _writer.WriteStartObject(); //geometry 
                _writer.WritePropertyName("type");
                _writer.WriteValue("Point");

                _writer.WritePropertyName("coordinates");
                _writer.WriteStartArray();
                _writer.WriteValue(row[_longitudeField]);
                _writer.WriteValue(row[_latitudeField]);
                _writer.WriteEndArray();

                _writer.WriteEndObject(); //geometry

                _writer.WritePropertyName("properties");
                _writer.WriteStartObject(); //properties

                _writer.WritePropertyName("description");
                if (_hasDescription) {
                    _writer.WriteValue(row[_descriptionField]);
                } else {
                    _writer.WriteValue("add geojson-description to output");
                }

                if (_hasBatchValue) {
                    _writer.WritePropertyName("batch-value");
                    _writer.WriteValue(row[_batchField]);
                }

                if (_hasColor) {
                    _writer.WritePropertyName("marker-color");
                    _writer.WriteValue(row[_colorField]);
                }

                if (_hasSymbol) {
                    var symbol = row[_symbolField].ToString();
                    _writer.WritePropertyName("marker-symbol");
                    _writer.WriteValue(symbol);
                }

                _writer.WriteEndObject(); //properties

                _writer.WriteEndObject(); //feature
            }

            if (Equals(_context.Process.Entities.Last(), _context.Entity)) {
                _writer.WriteEndArray(); //features
                _writer.WriteEndObject(); //root
            }

            _writer.Flush();
        }
    }
}
