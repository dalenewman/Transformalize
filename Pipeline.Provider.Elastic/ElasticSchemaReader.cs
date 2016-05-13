#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using Cfg.Net.Ext;
using Nest;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Provider.Elastic {
    public class ElasticSchemaReader : ISchemaReader {
        private readonly IConnectionContext _input;
        private readonly IElasticClient _elastic;
        private readonly string _index;

        public ElasticSchemaReader(IConnectionContext input, IElasticClient elastic) {
            _input = input;
            _elastic = elastic;
            _index = input.Connection.Index;
        }

        private IEnumerable<Configuration.Field> GetFields(string name) {
            var response = _elastic.GetMapping(new GetMappingRequest(_index, name));
            foreach (var pair in response.Mappings) {
                yield return new Configuration.Field { Name = pair.Key, Type = pair.Value.First().Analyzer.ToLower() }.WithDefaults();
            }
        }

        private IEnumerable<Entity> GetEntities() {
            var response = _elastic.GetMapping(new GetMappingRequest(_index, "_all"));
            foreach (var pair in response.Mappings) {
                yield return new Entity { Name = pair.Key, Fields = GetFields(pair.Key).ToList() }.WithDefaults();
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
