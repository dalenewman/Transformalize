﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Impl {
    public class CachingRunTimeSchemaReader : IRunTimeSchemaReader {

        private readonly IRunTimeSchemaReader _schemaReader;
        private readonly Dictionary<string, Schema> _cache = new Dictionary<string, Schema>();

        public CachingRunTimeSchemaReader(IRunTimeSchemaReader schemaReader) {
            _schemaReader = schemaReader;
        }

        public Process Process {
            get { return _schemaReader.Process;}
            set { _schemaReader.Process = value;}
        }

        public Schema Read(Process process) {
            Process = process;
            return Read();
        }

        public Schema Read() {
            if (_cache.ContainsKey(Process.Name)) {
                return _cache[Process.Name];
            }
            var schema = _schemaReader.Read();
            _cache[Process.Name] = schema;
            return schema;
        }

        public Schema Read(Entity entity) {
            if (_cache.ContainsKey(entity.Key)) {
                return _cache[entity.Key];
            }
            var schema = _schemaReader.Read(entity);
            _cache[entity.Key] = schema;
            return schema;
        }


    }
}