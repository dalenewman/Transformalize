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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class EntityConfigurationLoader {

        private const string DEFAULT = "[default]";
        private readonly Process _process;

        public EntityConfigurationLoader(Process process) {
            _process = process;
        }

        public Entity Read(TflEntity element, short entityIndex) {

            var entity = new Entity() {
                ProcessName = _process.Name,
                Schema = element.Schema,
                PipelineThreading = DetermineThreading(element),
                Name = element.Name,
                Prefix = element.Prefix,
                Group = element.Group,
                Delete = element.Delete,
                PrependProcessNameToOutputName = element.PrependProcessNameToOutputName,
                Sample = element.Sample,
                DetectChanges = element.DetectChanges,
                TrimAll = element.TrimAll,
                NoLock = element.NoLock,
                Unicode = element.Unicode,
                VariableLength = element.VariableLength,
                SqlOverride = element.Query,
                SqlScriptOverride = element.Script,
                SqlKeysOverride = element.QueryKeys,
                Alias = string.IsNullOrEmpty(element.Alias) ? element.Name : element.Alias,
                InputOperation = element.InputOperation,
                Index = entityIndex
            };

            GuardAgainstMissingPrimaryKey(element);

            // wire up connections
            if (_process.Connections.Contains(element.Connection)) {
                entity.Input.Add(_process.Connections.GetConnectionByName(element.Connection).NamedConnection());
            }

            //needs an input connection
            GuardAgainstMissingFields(element, entity, entityIndex);

            //fields
            short autoIndex = 0;

            foreach (TflField f in element.Fields) {
                var fieldType = GetFieldType(f, entityIndex == 0);

                var field = new FieldReader(_process, entity).Read(f, fieldType);
                if (field.Index.Equals(short.MaxValue)) {
                    field.Index = autoIndex;
                }

                entity.Fields.Add(field);

                if (f.PrimaryKey) {
                    entity.PrimaryKey.Add(field);
                }

                autoIndex++;
            }

            foreach (var cf in element.CalculatedFields) {
                var fieldReader = new FieldReader(_process, entity, usePrefix: false);
                var fieldType = GetFieldType(cf, entityIndex == 0);
                var field = fieldReader.Read(cf, fieldType);

                if (field.Index.Equals(short.MaxValue)) {
                    field.Index = autoIndex;
                }

                field.IsCalculated = true;
                field.Input = false;
                entity.CalculatedFields.Add(field);
                if (cf.PrimaryKey) {
                    entity.PrimaryKey.Add(field);
                }
                autoIndex++;
            }

            //depend on fields
            LoadVersion(element, entity);
            LoadFilter(element.Filter, entity, _process.Logger);

            entity.Input.AddRange(PrepareIo(element.Input, entity.Fields));
            entity.Output = PrepareIo(element.Output, entity.Fields);

            return entity;
        }

        private static void LoadFilter(IEnumerable<TflFilter> filter, Entity entity, ILogger logger) {

            foreach (var element in filter) {

                var item = new Filter(logger);

                if (!string.IsNullOrEmpty(element.Expression)) {
                    item.Expression = element.Expression;
                    entity.Filters.Add(item);
                    continue;
                }

                if (!string.IsNullOrEmpty(element.Left)) {
                    if (entity.HasField(element.Left)) {
                        item.LeftField = entity.FindField(element.Left).First();
                    } else {
                        item.LeftLiteral = element.Left;
                    }
                }

                if (!string.IsNullOrEmpty(element.Right)) {
                    if (entity.HasField(element.Right)) {
                        item.RightField = entity.FindField(element.Right).First();
                    } else {
                        item.RightLiteral = element.Right;
                    }
                }

                item.Operator = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true);
                item.Continuation = (Continuation)Enum.Parse(typeof(Continuation), element.Continuation, true);
                entity.Filters.Add(item);
            }

        }

        private PipelineThreading DetermineThreading(TflEntity element) {
            var threading = PipelineThreading.Default;
            if (_process.PipelineThreading != PipelineThreading.Default) {
                threading = _process.PipelineThreading;
            }

            if (!element.PipelineThreading.Equals("Default")) {
                PipelineThreading entityThreading;
                if (Enum.TryParse(element.PipelineThreading, true, out entityThreading)) {
                    threading = entityThreading;
                }
            }
            return threading;
        }

        private void GuardAgainstMissingFields(TflEntity element, Entity entity, short entityIndex) {

            if (_process.Mode != "metadata" && element.Fields.Count == 0) {
                try {

                    _process.Logger.EntityInfo(entity.Name, "Detecting fields.");
                    var connection = entity.Input.First().Connection;
                    var fields = connection.GetEntitySchema(_process, entity, entityIndex == 0);
                    if (!fields.Any())
                        return;

                    foreach (var field in fields) {
                        if (String.IsNullOrEmpty(field.Label) || field.Label.Equals(field.Alias)) {
                            field.Label = MetaDataWriter.AddSpacesToSentence(field.Alias, true).Replace("_", " ");
                        }
                        var name = field.Name;
                        var f = element.GetDefaultOf<TflField>(x => x.Name = name);
                        f.Type = field.Type;
                        f.Length = field.Length;
                        f.PrimaryKey = field.FieldType.Equals(FieldType.PrimaryKey) || field.FieldType.Equals(FieldType.MasterKey);
                        f.Output = true;
                        f.Default = string.Empty;
                        f.Input = true;
                        f.Precision = field.Precision;
                        f.Scale = field.Scale;
                        f.Label = field.Label;
                        element.Fields.Add(f);
                    }
                    _process.Logger.EntityInfo(entity.Name, "Detected {0} fields.", fields.Count);
                } catch (Exception ex) {
                    throw new TransformalizeException(_process.Logger, "No fields defined.  Unable to detect them for {0}. {1}", entity.Name, ex.Message);
                } finally {
                    if (element.Fields.Count == 0) {
                        throw new TransformalizeException(_process.Logger, entity.Name, "No fields defined.  Unable to detect them for {0}.", entity.Name);
                    }
                }
            }
        }

        private List<NamedConnection> PrepareIo(IEnumerable<TflIo> collection, Fields fields) {
            var namedConnections = new List<NamedConnection>();
            var entity = fields.Any() ? fields[0].Entity : "None";

            foreach (var io in collection) {
                Func<Row, bool> shouldRun = row => true;
                if (!io.RunField.Equals(string.Empty)) {
                    var f = io.RunField;
                    var match = fields.Find(f).ToArray();
                    if (match.Length > 0) {
                        var field = match[0];
                        if (io.RunType.Equals(DEFAULT)) {
                            io.RunType = field.SimpleType;
                        }
                        var op = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), io.RunOperator, true);
                        var simpleType = Common.ToSimpleType(io.RunType);
                        var value = Common.ConversionMap[simpleType](io.RunValue);
                        shouldRun = row => Common.CompareMap[op](row[field.Alias], value);
                    } else {
                        _process.Logger.EntityWarn(entity, "Field {0} specified in {1} output doesn't exist.  It will not affect the output.", io.RunField, io.Name);
                    }
                }

                namedConnections.Add(new NamedConnection {
                    Name = io.Name,
                    Connection = _process.Connections.GetConnectionByName(io.Connection).Connection,
                    ShouldRun = shouldRun
                });

            }
            return namedConnections;
        }

        private void GuardAgainstMissingPrimaryKey(TflEntity element) {

            if (element.Fields.Any(f => f.PrimaryKey))
                return;

            if (element.CalculatedFields.Any(cf => cf.PrimaryKey))
                return;

            if (!element.CalculatedFields.Any(cf => cf.Name.Equals("TflHashCode", StringComparison.OrdinalIgnoreCase))) {
                _process.Logger.EntityWarn(element.Name, "Adding TflHashCode primary key for {0}.", element.Name);
                var pk = element.GetDefaultOf<TflField>(f => {
                    f.Name = "TflHashCode";
                    f.Type = "int";
                    f.PrimaryKey = true;
                    f.Transforms = new List<TflTransform> {
                        element.GetDefaultOf<TflTransform>(t => {
                            t.Method = "concat";
                            t.Parameter = "*";
                        }), 
                        element.GetDefaultOf<TflTransform>(t => t.Method = "gethashcode")
                    };
                });

                element.CalculatedFields.Insert(0, pk);
            }

            if (string.IsNullOrEmpty(element.Version)) {
                element.Version = "TflHashCode";
            }
        }

        private void LoadVersion(TflEntity element, Entity entity) {
            if (String.IsNullOrEmpty(element.Version))
                return;

            if (entity.Fields.HaveField(entity.Alias, element.Version)) {
                entity.Version = entity.Fields.Find(entity.Alias, element.Version).First();
            } else {
                if (entity.CalculatedFields.HaveField(entity.Alias, element.Version)) {
                    entity.Version = entity.CalculatedFields.Find(entity.Alias, element.Version).First();
                } else {
                    throw new TransformalizeException(_process.Logger, "version field reference '{0}' is undefined in {1}.", element.Version, element.Name);
                }
            }
            entity.Version.Output = true;
        }

        private static FieldType GetFieldType(TflField element, bool isMaster) {
            FieldType fieldType;
            if (element.PrimaryKey) {
                fieldType = isMaster ? FieldType.MasterKey : FieldType.PrimaryKey;
            } else {
                fieldType = FieldType.NonKey;
            }
            return fieldType;
        }

    }
}