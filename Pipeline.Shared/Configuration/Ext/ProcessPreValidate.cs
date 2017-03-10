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
using System;
using System.Linq;
using Cfg.Net.Ext;

namespace Transformalize.Configuration.Ext {
    public static class ProcessPreValidate {

        const string All = "*";

        public static void PreValidate(this Process p, Action<string> error, Action<string> warn) {
            if (string.IsNullOrEmpty(p.Star)) {
                p.Star = p.Name + "Star";
            }

            if (string.IsNullOrEmpty(p.Flat)) {
                p.Flat = p.Name + "Flat";
            }

            // process-level calculated fields are not input
            foreach (var calculatedField in p.CalculatedFields) {
                calculatedField.Input = false;
                calculatedField.IsCalculated = true;
            }

            // Convenience, User can use Parameters collection at root process level instead of creating Environments collection with sets of parameters
            if (p.Parameters.Any() && p.Environments.Any()) {
                error("You can not have parameters and environments.  Choose one.");
            } else {
                if (p.Parameters.Any() && !p.Environments.Any()) {
                    p.Environments.Add(new Environment { Name = "One", Parameters = p.Parameters.Select(x => x.Clone()).ToList() });
                    p.Parameters.Clear();
                }
            }

            AddDefaultDelimiters(p);
            DefaultConnection(p, "input");
            DefaultConnection(p, "output");
            DefaultEntityConnections(p);
            DefaultOutput(p);
            DefaultSearchTypes(p);
            DefaultFileInspection(p);

            foreach (var entity in p.Entities) {
                try {
                    entity.AdaptFieldsCreatedFromTransforms();
                } catch (Exception ex) {
                    error($"Trouble adapting fields created from transforms. {ex.Message}");
                }

                if (!p.ReadOnly) {
                    entity.AddSystemFields();
                    entity.ModifyMissingPrimaryKey();
                }

                entity.ModifyIndexes();
            }

            try {
                ExpandShortHandTransforms(p);
            } catch (Exception ex) {
                error($"Error expanding short-hand transforms: {ex.Message}.");
            }

            // possible candidates for PostValidate
            MergeParameters(p);
            SetPrimaryKeys(p);

            // force primary key to output if not internal
            if (p.Output().IsNotInternal()) {
                foreach (var field in p.Entities.SelectMany(entity => p.GetAllFields().Where(field => field.PrimaryKey && !field.Output))) {
                    warn($"Primary Keys must be output. Overriding output to true for {field.Alias}.");
                    field.Output = true;
                }
            }

            // verify entities have level and message field for log output
            if (p.Output().Provider == "log") {
                foreach (var fields in p.Entities.Select(entity => entity.GetAllFields().ToArray())) {
                    if (!fields.Any(f => f.Alias.Equals("message", StringComparison.OrdinalIgnoreCase))) {
                        error("Log output requires a message field");
                    }
                    if (!fields.Any(f => f.Alias.Equals("level", StringComparison.OrdinalIgnoreCase))) {
                        error("Log output requires a level field");
                    }
                }
            }
        }

        /// <summary>
        /// If in meta mode with file connections, setup delimiters
        /// If no entity is available, create one for the file connection
        /// </summary>
        /// <param name="p"></param>
        private static void DefaultFileInspection(Process p) {
            if (p.Connections.All(c => c.Provider != "file"))
                return;

            if (p.Entities.Any())
                return;

            var connection = (p.Connections.FirstOrDefault(cn => cn.Provider == "file" && cn.Name == "input") ?? p.Connections.First(cn => cn.Provider == "file"));
            p.Entities.Add(
                new Entity {
                    Name = connection.Name,
                    Alias = connection.Name,
                    Connection = connection.Name,
                }
            );
        }

        static void AddDefaultDelimiters(Process p) {
            foreach (var connection in p.Connections.Where(c => c.Provider == "file" && c.Delimiter == string.Empty && !c.Delimiters.Any())) {
                connection.Delimiters.Add(new Delimiter { Name = "comma", Character = ',' });
                connection.Delimiters.Add(new Delimiter { Name = "tab", Character = '\t' });
                connection.Delimiters.Add(new Delimiter { Name = "pipe", Character = '|' });
                connection.Delimiters.Add(new Delimiter { Name = "semicolon", Character = ';' });
                //Delimiters.Add(new Delimiter { Name = "unit", Character = Convert.ToChar(31) });
            }
        }


        static void DefaultConnection(Process p, string name) {
            if (p.Connections.All(c => c.Name != name)) {
                p.Connections.Add(new Connection { Name = name });
            }
        }

        static void DefaultEntityConnections(Process p) {
            foreach (var entity in p.Entities.Where(entity => !entity.HasConnection())) {
                entity.Connection = p.Connections.Any(c => c.Name == "input") ? "input" : p.Connections.First().Name;
            }
        }

        static void DefaultOutput(Process p) {
            if (p.Connections.All(c => c.Name != "output"))
                p.Connections.Add(new Connection {
                    Name = "output",
                    Provider = "internal"
                });
        }

        static void DefaultSearchTypes(Process p) {

            var searchFields = p.GetSearchFields().ToArray();

            if (searchFields.Any()) {
                if (p.SearchTypes.All(st => st.Name != "none")) {
                    p.SearchTypes.Add(new SearchType {
                        Name = "none",
                        MultiValued = false,
                        Store = false,
                        Index = false
                    });
                }

                if (p.SearchTypes.All(st => st.Name != "default")) {
                    p.SearchTypes.Add(new SearchType {
                        Name = "default",
                        MultiValued = false,
                        Store = true,
                        Index = true
                    });
                }

            }

        }

        static void MergeParameters(Process p) {
            foreach (var entity in p.Entities) {
                entity.MergeParameters();
            }
            var index = 0;
            foreach (var field in p.CalculatedFields) {
                foreach (var transform in field.Transforms.Where(t => !Transform.ProducerSet().Contains(t.Method))) {
                    if (!string.IsNullOrEmpty(transform.Parameter)) {
                        if (transform.Parameter == All) {
                            foreach (var entity in p.Entities) {
                                foreach (var entityField in entity.GetAllFields().Where(f => f.Output && !f.System)) {
                                    transform.Parameters.Add(GetParameter(entity.Alias, entityField.Alias, entityField.Type));
                                }
                            }
                            var thisField = field;
                            foreach (var cf in p.CalculatedFields.Take(index).Where(cf => cf.Name != thisField.Name)) {
                                transform.Parameters.Add(GetParameter(string.Empty, cf.Alias, cf.Type));
                            }
                        } else {
                            if (transform.Parameter.IndexOf('.') > 0) {
                                var split = transform.Parameter.Split(new[] { '.' });
                                transform.Parameters.Add(GetParameter(split[0], split[1]));
                            } else {
                                transform.Parameters.Add(GetParameter(transform.Parameter));
                            }
                        }
                        transform.Parameter = string.Empty;
                    }

                }
                index++;
            }
        }

        /// <summary>
        /// Converts custom shorthand transforms
        /// </summary>
        static void ExpandShortHandTransforms(Process p) {
            foreach (var entity in p.Entities) {
                foreach (var field in entity.Fields) {
                    field.TransformCopyIntoParameters(entity);
                }
                foreach (var field in entity.CalculatedFields) {
                    field.TransformCopyIntoParameters(entity);
                }
            }
            foreach (var field in p.CalculatedFields) {
                field.TransformCopyIntoParameters();
            }
        }

        static void SetPrimaryKeys(Process p) {
            foreach (var field in p.Entities.SelectMany(entity => entity.GetAllFields().Where(field => field.PrimaryKey))) {
                field.KeyType = KeyType.Primary;
            }
        }

        static Parameter GetParameter(string field) {
            return new Parameter { Field = field };
        }

        static Parameter GetParameter(string entity, string field) {
            return new Parameter { Entity = entity, Field = field };
        }

        static Parameter GetParameter(string entity, string field, string type) {
            return new Parameter { Entity = entity, Field = field, Type = type };
        }
    }
}
