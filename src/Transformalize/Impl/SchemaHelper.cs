#region license
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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Impl {

    /// <summary>
    /// This class tries to help discover input fields when none are specified
    /// </summary>
    public class SchemaHelper : ISchemaHelper {
        private readonly IContext _context;
        private readonly IRunTimeSchemaReader _schemaReader;

        public SchemaHelper(IContext context, IRunTimeSchemaReader schemaReader) {
            _context = context;
            _schemaReader = schemaReader;
        }

        /// <summary>
        /// If any entity schema's are helped, this should return true so the consumer knows he/she ought to check process.Errors().
        /// </summary>
        /// <returns>true if any schema has been helped, false if no help provided</returns>
        public bool Help(Process process) {

            var helped = false;
            foreach (var entity in process.Entities) {

                if (entity.Fields.Any(f => f.Input)) {
                    continue;
                }

                _schemaReader.Process = process;
                var schema = _schemaReader.Read(entity);
                if (!schema.Entities.Any()) {
                    _context.Warn($"Could not detect {entity.Alias} fields.");
                    continue;
                }

                var e = schema.Entities.First();
                var fields = e.Fields;
                foreach (var field in fields.Where(field => Constants.InvalidFieldNames.Contains(field.Name) && Constants.InvalidFieldNames.Contains(field.Alias))) {
                    field.Alias = e.Alias + field.Name;
                }
                var add = e.Fields.Where(f => !f.System).ToList();

                var processKeys = new HashSet<string>(process.GetAllFields().Select(f => f.Alias), System.StringComparer.OrdinalIgnoreCase);

                if (add.Any()) {
                    _context.Debug(() => $"Detected {add.Count} field{add.Count.Plural()} in {entity.Alias}.");

                    var entityKeys = new HashSet<string>(entity.GetAllFields().Select(f => f.Alias), System.StringComparer.OrdinalIgnoreCase);
                    

                    foreach (var field in add) {
                        if (!entityKeys.Contains(field.Alias)) {
                            if (!field.PrimaryKey && processKeys.Contains(field.Alias)) {
                                field.Alias = entity.Alias + field.Alias;
                            }
                            entity.Fields.Add(field);
                        }
                    }
                    process.Connections.First(c => c.Name == e.Input).Delimiter = schema.Connection.Delimiter;
                    helped = true;
                } else {
                    _context.Warn($"Could not detect {entity.Alias} fields.");
                }
            }

            return helped;

        }
    }
}