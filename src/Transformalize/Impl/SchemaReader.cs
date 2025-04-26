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
using Cfg.Net.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize {
    public class SchemaReader : ISchemaReader {

        private readonly IConnectionContext _context;
        private readonly IRunTimeRun _runner;
        private readonly Process _process;

        public SchemaReader(IConnectionContext context, IRunTimeRun runner, Process process) {
            _context = context;
            _runner = runner;
            _process = process;
        }

        private void ModifyFields(IEnumerable<Field> fields) {

            var expanded = fields.ToArray();

            if (_context.Connection.MaxLength > 0) {
                foreach (var field in expanded) {
                    field.Length = _context.Connection.MaxLength.ToString();
                }
            }

            var checkLength = _context.Connection.MinLength > 0 || _context.Connection.MaxLength > 0;
            var checkTypes = _context.Connection.Types.Any(t => t.Type != "string");

            if (checkTypes || checkLength) {
                var rows = _runner.Run(_process).ToArray(); // can take a lot of memory
                if (rows.Length == 0)
                    return;

                if (checkLength) {
#if NETS10
                    foreach (var f in expanded) {
                        var length = _context.Connection.MaxLength == 0 ? rows.Max(row => row[f].ToString().Length) + 1 : Math.Min(rows.Max(row => row[f].ToString().Length) + 1, _context.Connection.MaxLength);
                        if (_context.Connection.MinLength > 0 && length < _context.Connection.MinLength) {
                            length = _context.Connection.MinLength;
                        }
                        f.Length = length.ToString();
                    }
#else
                    Parallel.ForEach(expanded, f => {
                        var length = _context.Connection.MaxLength == 0 ? rows.Max(row => row[f].ToString().Length) + 1 : Math.Min(rows.Max(row => row[f].ToString().Length) + 1, _context.Connection.MaxLength);
                        if (_context.Connection.MinLength > 0 && length < _context.Connection.MinLength) {
                            length = _context.Connection.MinLength;
                        }
                        f.Length = length.ToString();
                    });
#endif
                }

                if (checkTypes) {
                    var canConvert = Constants.CanConvert();
#if NETS10
                    foreach (var f in expanded) {
                        foreach (var dataType in _context.Connection.Types.Where(t => t.Type != "string")) {
                            if (rows.All(r => canConvert[dataType.Type](r[f].ToString()))) {
                                f.Type = dataType.Type;
                                break;
                            }
                        }
                    }
#else
                    Parallel.ForEach(expanded, f => {
                        foreach (var dataType in _context.Connection.Types.Where(t => t.Type != "string")) {
                            if (rows.All(r => canConvert[dataType.Type](r[f].ToString()))) {
                                f.Type = dataType.Type;
                                break;
                            }
                        }
                    });
#endif
                }
            }

        }

        public Schema Read() {
            var entity = _process.Entities.First();
            return Read(entity);
        }

        public Schema Read(Entity entity) {
            var fields = entity.Fields.Where(f => !f.System).ToList();
            if (!fields.Any() && _process.Entities.Any()) {
                fields = _process.Entities.First().Fields.Where(f => !f.System).ToList();
            }

            // clone fields, note: clone only clones cfg-net fields, so we have to copy indexes too
            var newFields = fields.Select(field => field.Clone()).ToList();
            for (int i = 0; i < fields.Count; i++) {
                newFields[i].Index = fields[i].Index;
                newFields[i].MasterIndex = fields[i].MasterIndex;
                newFields[i].KeyIndex = fields[i].KeyIndex;
            }

            ModifyFields(newFields);

            var newEntity = entity.Clone();
            newEntity.Fields = newFields;

            return new Schema {
                Connection = _process.Connections.FirstOrDefault(c => c.Name == entity.Input),
                Entities = new List<Entity> { newEntity }
            };
        }

    }
}
