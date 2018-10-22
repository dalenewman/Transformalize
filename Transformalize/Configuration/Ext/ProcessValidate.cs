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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Extensions;

namespace Transformalize.Configuration.Ext {
    public static class ProcessValidate {

        public static void Validate(this Process p, Action<string> error, Action<string> warn) {

            if (p.ReadOnly && p.Buffer) {
                error("A process can not be read-only and buffer at the same time.");
            }

            if (p.Environment != string.Empty && p.Environments.All(e => e.Name != p.Environment)) {
                warn($"This process refers to an undefined environment: {p.Environment}");
                if (p.Environments.Any()) {
                    p.Environment = p.Environments.First().Name;
                } else {
                    error("Please remove the environment attribute or add a matching environment.");
                }
            }

            ValidateDuplicateEntities(p, error);
            ValidateDuplicateFields(p, error);
            ValidateRelationships(p, error, warn);
            ValidateEntityConnections(p, error);
            ValidateEntityInvalidFields(p, error);
            ValidateEntityMeetsProviderExpectations(p, error);
            ValidateEntityFilterMaps(p, error);
            ValidateEntityFieldMaps(p, error);
            ValidateActionConnections(p, error);
            ValidateTemplateActionConnections(p, error);
            ValidateTransformConnections(p, error);
            ValidateMapConnections(p, error);
            ValidateMapTransforms(p, error);
            ValidateSearchTypes(p, error);
            ValidateShouldRuns(p, error, warn);
            ValidateScripts(p, error);
            ValidateParameterMaps(p, error);
            ValidateDirectoryReaderHasAtLeastOneValidField(p, error);
            ValidateFlatten(p, error, warn);
            ValidateTransformParameters(p, error, warn);
        }

        private static void ValidateTransformParameters(Process process, Action<string> error, Action<string> warn) {
            var fields = new HashSet<string>(process.GetAllFields().Select(f => f.Alias));
            foreach (var transform in process.GetAllTransforms().Where(t => !Operation.ProducerSet().Contains(t.Method))) {
                foreach (var parameter in transform.Parameters) {
                    if (parameter.Name != string.Empty) {
                        if (fields.Contains(parameter.Name)) {
                            warn($"A parameter name attribute is the same as a field name: {parameter.Name}.  Perhaps you meant to use use the field attribute.");
                        }
                    }

                    if (parameter.Field != string.Empty) {
                        if (!fields.Contains(parameter.Field)) {
                            error($"A {transform.Method} operation refers to an invalid field: {parameter.Field}!  Note: if a field name is aliased, use the alias.");
                        }
                    }

                }
            }
        }

        private static void ValidateEntityInvalidFields(Process p, Action<string> error) {
            if (p.ReadOnly)
                return;

            foreach (var entity in p.Entities) {
                foreach (var field in entity.GetAllFields().Where(f => !f.System)) {
                    if (Constants.InvalidFieldNames.Contains(field.Alias)) {
                        error($"{field.Alias} is a reserved word in {entity.Alias}.  Please alias it (<a name='{field.Alias}' alias='{entity.Alias}{field.Alias.Remove(0, 3)}' />).");
                    }
                }
            }
        }

        private static void ValidateFlatten(Process p, Action<string> error, Action<string> warn) {
            if (!p.Flatten)
                return;

            if (p.Entities.Count < 2) {
                warn("To flatten, you must have at least 2 entities.");
            }
            //if (p.Output() == null || !Constants.AdoProviderSet().Contains(p.Output().Provider)) {
            //    error($"To flatten, you must use an ADO based output provider, e.g. ({Constants.AdoProviderDomain})");
            //}
        }

        private static void ValidateDirectoryReaderHasAtLeastOneValidField(Process process, Action<string> error) {

            var fieldNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"CreationTimeUtc","date"},
                {"DirectoryName","string"},
                {"Extension","string"},
                {"FullName","string"},
                {"LastWriteTimeUtc","datetime"},
                {"Length","long"},
                {"Name","string"}
            };

            if (process.Connections.All(c => c.Provider != "filesystem"))
                return;

            foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == "filesystem").Where(entity => !entity.Fields.Where(f => f.Input).Any(f => fieldNames.ContainsKey(f.Name)))) {
                error($"The {entity.Alias} entity reads a directory listing. It needs at least one of these valid fields: {(string.Join(", ", fieldNames).Replace(", Name", ", or Name"))}.");
            }

            foreach (
                var field in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == "filesystem").SelectMany(e => e.Fields.Where(f => f.Input && fieldNames.ContainsKey(f.Name)))) {
                var type = fieldNames[field.Name];
                if (field.Type.StartsWith(type, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }
                if (type == "long" && field.Type.Equals("Int64", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }
                error($"The field named {field.Name} has an incompatible type of {field.Type}. The type should be {type}.");
            }


        }

        private static void ValidateParameterMaps(Process process, Action<string> error) {
            foreach (var parameter in process.Environments.SelectMany(e => e.Parameters.Where(p => p.Map != string.Empty)).Where(parameter => process.Maps.All(m => m.Name != parameter.Map))) {
                error($"A parameter refers to invalid map: {parameter.Map}.");
            }
        }

        private static void ValidateScripts(Process process, Action<string> error) {
            var scriptsRegistered = process.Scripts.Select(s => s.Name);

            var transformRefs = process.GetAllTransforms().Where(t => t.Scripts.Any()).SelectMany(t => t.Scripts).Select(s => s.Name).Distinct();
            var entityRefs = process.Entities.Where(e => e.Script != string.Empty).Select(e => e.Script).Distinct();
            var references = transformRefs.Union(entityRefs);

            var problems = references.Except(scriptsRegistered).ToArray();
            if (problems.Length <= 0)
                return;
            foreach (var problem in problems) {
                error($"The {problem} script is not registered in the scripts collection.");
            }
        }

        private static void ValidateMapTransforms(Process p, Action<string> error) {
            foreach (var transform in p.GetAllTransforms().Where(t => t.Method == "map")) {
                if (p.Maps.All(m => m.Name != transform.Map)) {
                    error($"A map transform references an invalid map: {transform.Map}.");
                }
                if (p.Maps.All(m => m.Name != transform.Map)) {
                    error($"The map {transform.Map} is invalid.");
                } else {
                    var map = p.Maps.First(m => m.Name == transform.Map);
                    foreach (var item in map.Items.Where(i => i.Parameter != string.Empty)) {
                        Field field;
                        if (!p.TryGetField(item.Parameter, out field)) {
                            error($"A map transform references an invalid field: {item.Parameter}.");
                        }
                    }
                }
            }
        }


        private static void ValidateMapConnections(Process p, Action<string> error) {
            foreach (var map in p.Maps.Where(m => m.Query != string.Empty).Where(map => p.Connections.All(c => c.Name != map.Connection))) {
                error($"The {map.Name} map references an invalid connection: {map.Connection}.");
            }
        }

        private static void ValidateTransformConnections(Process p, Action<string> error) {

            var methodsWithConnections = new[] { "mail", "run" };

            foreach (var transform in p.GetAllTransforms().Where(t => methodsWithConnections.Any(nc => nc == t.Method))) {
                var connection = p.Connections.FirstOrDefault(c => c.Name == transform.Connection);
                if (connection == null) {
                    error($"The {transform.Method} transform references an invalid connection: {transform.Connection}.");
                    continue;
                }

                switch (transform.Method) {
                    case "mail":
                        if (connection.Provider != "mail") {
                            error($"The {transform.Method} transform references the wrong type of connection: {connection.Provider}.");
                        }
                        break;
                }
            }
        }

        private static void ValidateTemplateActionConnections(Process p, Action<string> error) {
            foreach (var action in p.Templates.SelectMany(template => template.Actions.Where(a => a.Connection != string.Empty).Where(action => p.Connections.All(c => c.Name != action.Connection)))) {
                error($"The {action.Type} template action references an invalid connection: {action.Connection}.");
            }
        }

        private static void ValidateActionConnections(Process p, Action<string> error) {
            foreach (var action in p.Actions.Where(action => action.Connection != string.Empty).Where(action => p.Connections.All(c => c.Name != action.Connection))) {
                error($"The {action.Type} action references an invalid connection: {action.Connection}.");
            }
        }

        private static void ValidateEntityConnections(Process p, Action<string> error) {
            foreach (var entity in p.Entities.Where(entity => p.Connections.All(c => c.Name != entity.Connection))) {
                error($"The {entity.Name} entity references an invalid connection: {entity.Connection}.");
            }
        }

        private static void ValidateEntityMeetsProviderExpectations(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                var connection = p.Output();
                if (connection != null) {
                    switch (connection.Provider) {
                        case "kml":
                        case "geojson":
                            var fields = entity.GetAllFields().ToArray();
                            var lat = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
                            if (lat == null) {
                                error($"The {entity.Alias} entity needs a latitude (or lat) output field in order to output {connection.Provider}.");
                            } else {
                                var lon = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
                                if (lon == null) {
                                    error($"The {entity.Alias} entity needs a longitude (or lon) output field in order to output {connection.Provider}.");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void ValidateEntityFilterMaps(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                foreach (var filter in entity.Filter.Where(filter => !string.IsNullOrEmpty(filter.Map)).Where(filter => p.Maps.All(m => m.Name != filter.Map))) {
                    error($"The filter on field {filter.Field} in entity {entity.Alias} refers to invalid map: {filter.Map}");
                }
                foreach (var filter in entity.Filter.Where(f => f.Type == "facet")) {
                    if (filter.Field == string.Empty) {
                        error("Facet filters need a field.");
                    }
                    if (filter.Map == string.Empty) {
                        error("Facet filters need a map to load facets into.");
                    }
                }
            }

        }

        private static void ValidateEntityFieldMaps(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                foreach (var field in entity.Fields.Where(f => !string.IsNullOrEmpty(f.Map)).Where(f => p.Maps.All(m => m.Name != f.Map))) {
                    error($"The field {field.Alias} in entity {entity.Alias} refers to invalid map: {field.Map}");
                }
            }
        }

        private static void ValidateRelationships(Process p, Action<string> error, Action<string> warn) {

            // count check
            if (p.Entities.Count > 1 && p.Relationships.Count + 1 < p.Entities.Count) {
                var message = $"You have {p.Entities.Count} entities so you need {p.Entities.Count - 1} relationships. You have {p.Relationships.Count} relationships.";
                if (p.Mode == "check") {
                    warn(message);
                } else {
                    error(message);
                }
            }

            //entity alias, name check, and if that passes, do field alias, name check
            foreach (var relationship in p.Relationships) {
                var problem = false;

                // validate (and modify) left side
                Entity leftEntity;
                if (p.TryGetEntity(relationship.LeftEntity, out leftEntity)) {
                    relationship.Summary.LeftEntity = leftEntity;
                    foreach (var leftField in relationship.GetLeftJoinFields()) {
                        Field field;
                        if (leftEntity.TryGetField(leftField, out field)) {
                            relationship.Summary.LeftFields.Add(field);
                        } else {
                            var message = $"A relationship references a left-field that doesn't exist: {leftField}";
                            if (p.Mode == "check") {
                                warn(message);
                            } else {
                                error(message);
                            }
                            problem = true;
                        }
                    }
                } else {
                    var message = $"A relationship references a left-entity that doesn't exist: {relationship.LeftEntity}";
                    if (p.Mode == "check") {
                        error(message);
                    } else {
                        warn(message);
                    }
                    problem = true;
                }

                //validate (and modify) right side
                Entity rightEntity;
                if (p.TryGetEntity(relationship.RightEntity, out rightEntity)) {
                    relationship.Summary.RightEntity = rightEntity;
                    foreach (var rightField in relationship.GetRightJoinFields()) {
                        Field field;
                        if (rightEntity.TryGetField(rightField, out field)) {
                            relationship.Summary.RightFields.Add(field);
                        } else {
                            var message = $"A relationship references a right-field that doesn't exist: {rightField}";
                            if (p.Mode == "check") {
                                warn(message);
                            } else {
                                error(message);
                            }
                            error(message);
                            problem = true;
                        }
                    }
                } else {
                    if (p.Mode == "check") {
                        warn($"A relationship references a right-entity that doesn't exist: {relationship.RightEntity}");
                    } else {
                        error($"A relationship references a right-entity that doesn't exist: {relationship.RightEntity}");
                    }
                    problem = true;
                }

                //if everything is cool, set the foreign key flags
                if (!problem && relationship.Summary.IsAligned()) {
                    for (var i = 0; i < relationship.Summary.LeftFields.Count; i++) {
                        var leftField = relationship.Summary.LeftFields[i];
                        var rightField = relationship.Summary.RightFields[i];

                        leftField.KeyType |= KeyType.Foreign;
                        if (!leftField.Output) {
                            warn($"Foreign key {leftField.Alias} on left side must be output. Overriding output to true.");
                            leftField.Output = true;
                        }

                        if (leftField.Type != rightField.Type) {
                            warn($"The {leftField.Alias} and {rightField.Alias} relationship fields do not have the same type.");
                        }
                    }
                }

            }

        }


        private static void ValidateDuplicateFields(Process p, Action<string> error) {
            var fieldDuplicates = p.Entities
                .SelectMany(e => e.GetAllFields())
                .Where(f => f.Output && !f.PrimaryKey && !f.System && f.Name != null)
                .Concat(p.CalculatedFields.Where(f => f.Name != null))
                .GroupBy(f => f.Alias.ToLower())
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in fieldDuplicates) {
                error($"The entity field '{duplicate}' occurs more than once. Remove, alias, or prefix one.");
            }
        }


        private static void ValidateDuplicateEntities(Process p, Action<string> error) {
            var entityDuplicates = p.Entities
                .GroupBy(e => e.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in entityDuplicates) {
                error($"The '{duplicate}' entity occurs more than once. Remove or alias one.");
            }
        }


        private static void ValidateShouldRuns(Process p, Action<string> error, Action<string> warn) {

            // transform run-* fields inherit from field run-* fields if necessary
            foreach (var entity in p.Entities) {
                foreach (var field in entity.GetAllFields().Where(f => f.RunField != string.Empty)) {
                    foreach (var transform in field.Transforms.Where(t => t.RunField == string.Empty)) {
                        transform.RunField = field.RunField;
                        transform.RunOperator = field.RunOperator;
                        transform.RunValue = field.RunValue;
                    }
                }
            }

            foreach (var entity in p.Entities) {
                EvaluateRunFields(entity, f => f.Transforms, error, warn);
                EvaluateRunFields(entity, f => f.Validators, error, warn);
            }
        }

        private static void EvaluateRunFields(Entity entity, Func<Field, List<Operation>> operations, Action<string> error, Action<string> warn) {
            foreach (var field in entity.GetAllFields().Where(f => operations(f).Any(t => t.RunField != string.Empty))) {
                foreach (var operation in operations(field).Where(t => t.RunField != string.Empty)) {
                    if (entity.TryGetField(operation.RunField, out var runField)) {
                        var runValue = runField.Type == "bool" && operation.RunValue == Constants.DefaultSetting ? "true" : operation.RunValue;
                        try {
                            var value = Constants.ConversionMap[runField.Type](runValue);
                            operation.ShouldRun = row => Utility.Evaluate(row[runField], operation.RunOperator, value);
                        } catch (Exception ex) {
                            error($"Trouble converting {runValue} to {runField.Type}. {ex.Message}");
                        }
                    } else {
                        warn($"Run Field {operation.RunField} does not exist in {entity.Alias}, so it will not be evaluated.");
                    }
                }
            }
        }

        private static void ValidateSearchTypes(Process p, Action<string> error) {
            foreach (var name in p.GetSearchFields().Where(f => !f.System).Select(f => f.SearchType).Distinct()) {
                if (name != "none" && p.SearchTypes.All(st => st.Name != name)) {
                    error($"Search type {name} is invalid. Add it to search types, or remove it from the field using it.");
                }
            }
        }
    }
}