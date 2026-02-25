#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Elasticsearch.Net;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elasticsearch {
   public class ElasticSchemaReader : ISchemaReader {
      private readonly IConnectionContext _input;
      private readonly string _index;
      private readonly IElasticLowLevelClient _client;

      public ElasticSchemaReader(IConnectionContext input, IElasticLowLevelClient client) {
         _input = input;
         _client = client;
         _index = input.Connection.Index;
      }

      public IEnumerable<Field> GetFields(string name) {

         var version = ElasticVersionParser.ParseVersion(_input);
         DynamicResponse response;

         response = _client.Indices.GetMapping<DynamicResponse>(_index);

         if (response.Success) {
            var properties = response.Body[_index]["mappings"][name]["properties"];
            if (properties != null && properties.HasValue) {
               return PropertiesToFields(name, properties.Value as IDictionary<string, object>);
            }
            _input.Error("Could not find properties for index {0} type {1}.", _index, name);
         } else {
            _input.Error(response.ToString());
         }

         return Enumerable.Empty<Field>();

      }

      private IEnumerable<Field> PropertiesToFields(string name, IDictionary<string, object> properties) {
         if (properties != null) {
            foreach (var field in properties) {
               var f = new Field { Name = field.Key };
               var attributes = field.Value as IDictionary<string, object>;
               if (attributes != null && attributes.ContainsKey("type")) {
                  f.Type = attributes["type"].ToString();
                  if (f.Type == "integer") {
                     f.Type = "int";
                  }
               } else {
                  _input.Warn("Could not find type for index {0} type {1} field {2}. Default is string.", _index, name, field.Key);
               }
               yield return f;
            }
         } else {
            _input.Error("Could not find fields for index {0} type {1}.", _index, name);
         }
      }

      public IEnumerable<Entity> GetEntities() {

         var version = ElasticVersionParser.ParseVersion(_input);
         DynamicResponse response;

         response = _client.Indices.GetMapping<DynamicResponse>(_index);

         if (response.Success) {
            var mappings = response.Body[_index]["mappings"];
            if (mappings != null && mappings.HasValue) {
               var types = mappings.Value as IDictionary<string, object>;
               if (types != null) {
                  foreach (var pair in types) {
                     var e = new Entity { Name = pair.Key };
                     var attributes = pair.Value as IDictionary<string, object>;
                     if (attributes != null && attributes.ContainsKey("properties")) {
                        e.Fields = PropertiesToFields(pair.Key, attributes["properties"] as IDictionary<string, object>).ToList();
                     } else {
                        _input.Error("Could not find properties for index {0} type {1}.", _input, pair.Key);
                     }
                     yield return e;
                  }
               } else {
                  _input.Error("Could not find types in index {0}.", _index);
               }
            } else {
               _input.Error("Could not find mappings for index {0}.", _index);
            }
         } else {
            _input.Error(response.ToString());
         }

      }

      public Schema Read() {
         var schema = new Schema { Connection = _input.Connection };
         schema.Entities.AddRange(GetEntities());
         return schema;
      }

      public Schema Read(Entity entity) {
         var schema = new Schema { Connection = _input.Connection };
         entity.Fields = GetFields(entity.Name).ToList();
         schema.Entities.Add(entity);
         return schema;
      }

   }
}
