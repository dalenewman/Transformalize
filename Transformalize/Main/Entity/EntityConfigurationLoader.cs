#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main {
    public class EntityConfigurationLoader {
        private const string DEFAULT = "[default]";
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;

        public EntityConfigurationLoader(Process process) {
            _process = process;
        }

        public Entity Read(int batchId, EntityConfigurationElement element, bool isMaster) {

            Validate(element);

            GlobalDiagnosticsContext.Set("entity", Common.LogLength(element.Alias));

            var threading = _process.PipelineThreading;

            if (!string.IsNullOrEmpty(element.PipelineThreading)) {
                PipelineThreading elementThreading;
                if (Enum.TryParse(element.PipelineThreading, true, out elementThreading)) {
                    threading = elementThreading;
                }
            }

            var entity = new Entity(batchId) {
                ProcessName = _process.Name,
                Schema = element.Schema,
                PipelineThreading = threading,
                Name = element.Name,
                InputConnection = _process.Connections[element.Connection],
                Prefix = element.Prefix,
                Group = element.Group,
                IndexOptimizations = element.IndexOptimizations,
                Delete = element.Delete,
                PrependProcessNameToOutputName = element.PrependProcessNameToOutputName,
                Sample = element.Sample,
                SqlOverride = element.SqlOverride,
                Alias = string.IsNullOrEmpty(element.Alias) ? element.Name : element.Alias,
                InternalOutput = element.Output.Cast<OutputConfigurationElement>().ToDictionary(o => o.Name, o => Enumerable.Repeat(new Row(), 0)),
                UseBcp = element.UseBcp,
                InputOperation = element.InputOperation,
            };

            GuardAgainstInvalidGrouping(element, entity);
            GuardAgainstMissingPrimaryKey(element);

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in element.Fields) {
                var fieldType = GetFieldType(f, isMaster);

                var field = new FieldReader(_process, entity).Read(f, fieldType);

                if (field.Index == 0) {
                    field.Index = fieldIndex;
                }

                entity.Fields[field.Alias] = field;

                if (f.PrimaryKey) {
                    entity.PrimaryKey[field.Alias] = field;
                }

                fieldIndex++;
            }

            foreach (FieldConfigurationElement cf in element.CalculatedFields) {
                var fieldReader = new FieldReader(_process, entity, usePrefix: false);
                var fieldType = GetFieldType(cf, isMaster);
                var field = fieldReader.Read(cf, fieldType);

                if (field.Index == 0) {
                    field.Index = fieldIndex;
                }

                field.Input = false;
                entity.CalculatedFields.Add(cf.Alias, field);
                if (cf.PrimaryKey) {
                    entity.PrimaryKey[cf.Alias] = field;
                }
                fieldIndex++;
            }

            LoadVersion(element, entity);

            // output
            var outputValidator = ValidationFactory.CreateValidator<OutputConfigurationElement>();
            foreach (OutputConfigurationElement output in element.Output) {

                var results = outputValidator.Validate(output);

                if (results.IsValid) {
                    if (_process.Connections.ContainsKey(output.Connection)) {

                        Func<Row, bool> shouldRun = row => true;

                        if (!output.RunField.Equals(string.Empty)) {
                            var f = output.RunField;
                            var match = entity.Fields.Where(p => p.Value.Alias.Equals(f) || p.Value.Name.Equals(f)).Select(p => p.Value).ToArray();
                            if (match.Length > 0) {
                                var field = match[0];
                                if (output.RunType.Equals(DEFAULT)) {
                                    output.RunType = field.SimpleType;
                                }
                                var op = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), output.RunOperator, true);
                                var simpleType = Common.ToSimpleType(output.RunType);
                                var value = Common.ConversionMap[simpleType](output.RunValue);
                                shouldRun = row => Common.CompareMap[op](row[field.Alias], value);
                            } else {
                                _log.Warn("Field {0} specified in {1} output doesn't exist.  It will not affect the output.", output.RunField, output.Name);
                            }
                        }

                        entity.Output.Add(new Output {
                            Name = output.Name,
                            Connection = _process.Connections[output.Connection],
                            ShouldRun = shouldRun
                        });
                    } else {
                        _log.Warn("Can't add output {0} because connection {1} doesn't exist.", output.Name, output.Connection);
                    }
                } else {
                    _log.Warn("Output {0} is invalid.", output.Name);
                    foreach (var reason in results) {
                        _log.Warn(reason.Message);
                    }
                }
            }

            return entity;
        }

        private void Validate(EntityConfigurationElement element) {
            var validator = ValidationFactory.CreateValidator<EntityConfigurationElement>();
            var results = validator.Validate(element);
            if (!results.IsValid) {
                foreach (var result in results) {
                    _process.ValidationResults.AddResult(result);
                    _log.Error(result.Message);
                }
                LogManager.Flush();
                Environment.Exit(1);
            }
        }

        private void GuardAgainstMissingPrimaryKey(EntityConfigurationElement element) {
            if (element.Fields.Cast<FieldConfigurationElement>().Any(f => f.PrimaryKey))
                return;

            if (element.CalculatedFields.Cast<FieldConfigurationElement>().Any(cf => cf.PrimaryKey))
                return;

            _log.Info("Adding TflHashCode primary key for {0}.", element.Name);
            var pk = new FieldConfigurationElement {
                Name = "TflHashCode",
                Type = "System.Int32",
                PrimaryKey = true,
                Transforms = new TransformElementCollection {
                    new TransformConfigurationElement {Method = "concat", Parameter = "*"},
                    new TransformConfigurationElement {Method = "gethashcode"}
                }
            };
            element.CalculatedFields.Insert(pk);
            if (string.IsNullOrEmpty(element.Version)) {
                element.Version = "TflHashCode";
            }
        }

        private void LoadVersion(EntityConfigurationElement element, Entity entity) {
            if (String.IsNullOrEmpty(element.Version))
                return;

            if (entity.Fields.ContainsKey(element.Version)) {
                entity.Version = entity.Fields[element.Version];
            } else {
                if (entity.Fields.Any(kv => kv.Value.Name.Equals(element.Version, IC))) {
                    entity.Version = entity.Fields.ToEnumerable().First(v => v.Name.Equals(element.Version, IC));
                } else {
                    if (entity.CalculatedFields.ContainsKey(element.Version)) {
                        entity.Version = entity.CalculatedFields[element.Version];
                    } else {
                        _log.Error("version field reference '{0}' is undefined in {1}.", element.Version, element.Name);
                        LogManager.Flush();
                        Environment.Exit(1);
                    }
                }
            }
            entity.Version.Output = true;
        }

        private void GuardAgainstInvalidGrouping(EntityConfigurationElement element, Entity entity) {
            if (!entity.Group)
                return;

            if (!element.Fields.Cast<FieldConfigurationElement>().Any(f => f.Output && string.IsNullOrEmpty(f.Aggregate)))
                return;

            _log.Error("Entity {0} is set to group, but not all your output fields have aggregate defined.", entity.Alias);
            LogManager.Flush();
            Environment.Exit(1);
        }

        private static FieldType GetFieldType(FieldConfigurationElement element, bool isMaster) {
            FieldType fieldType;
            if (element.PrimaryKey) {
                fieldType = isMaster ? FieldType.MasterKey : FieldType.PrimaryKey;
            } else {
                fieldType = FieldType.Field;
            }
            return fieldType;
        }
    }
}