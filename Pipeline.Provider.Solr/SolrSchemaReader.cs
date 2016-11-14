#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;
using SolrNet;

namespace Pipeline.Provider.Solr {
    public class SolrSchemaReader : ISchemaReader {
        private readonly Connection _c;
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrSchemaReader(Connection c, ISolrReadOnlyOperations<Dictionary<string, object>> solr) {
            _c = c;
            _solr = solr;
        }

        private IEnumerable<Field> GetFields()
        {
            string type;
            var s = _solr.GetSchema(_c.SchemaFileName);
            var typeSet = Constants.TypeSet();
            foreach (var field in s.SolrFields.Where(f => f.IsStored)) {
                type = field.Type.Name.ToLower();
                yield return new Field { Name = field.Name, Type = typeSet.Contains(type) ? type : "string" }.WithDefaults();
            }
            foreach (var field in s.SolrCopyFields) {
                var d = s.FindSolrFieldByName(field.Destination);
                type = d.Type.Name.ToLower();
                yield return new Field { Name = field.Destination, Type = typeSet.Contains(type) ? type : "string" }.WithDefaults();
            }
            foreach (var field in s.SolrDynamicFields) {
                yield return new Field { Name = field.Name, Type = "string" }.WithDefaults();
            }
        }

        private IEnumerable<Entity> GetEntities() {
            yield return new Entity {
                Name = "Fields",
                Fields = GetFields().ToList()
            }.WithDefaults();
        }

        public Schema Read() {
            var schema = new Schema { Connection = _c };
            schema.Entities.AddRange(GetEntities());
            return schema;
        }

        public Schema Read(Entity entity) {
            var schema = new Schema { Connection = _c };
            entity.Fields = GetFields().ToList();
            schema.Entities.Add(entity);
            return schema;
        }

    }
}
