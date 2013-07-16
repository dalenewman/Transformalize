/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Transforms;
using System.Linq;

namespace Transformalize.Readers {

    public class ProcessReader : WithLoggingMixin, IConfigurationReader<Process> {
        private readonly string _name;
        private readonly IConnectionChecker _connectionChecker;
        private Process _process;
        private readonly ProcessConfigurationElement _config;
        private int _connectionCount;
        private int _mapCount;
        private int _entityCount;
        private int _relationshipCount;
        private int _transformCount;
        public int Count { get { return 1; } }

        public ProcessReader(string name, IConnectionChecker connectionChecker = null) {
            _name = name;
            _connectionChecker = connectionChecker ?? new SqlServerConnectionChecker(name);
            _config = ((TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize")).Processes.Get(_name);
        }

        public Process Read() {
            ReadProcess();
            _connectionCount = ReadConnections(_config);
            _mapCount = ReadMaps(_config);
            _entityCount = ReadEntities(_config);
            _relationshipCount = ReadRelationships(_config);
            _transformCount = ReadTransforms(_config);
            _process.RelatedKeys = ReadRelatedKeys();
            _process.View = _process.MasterEntity.OutputName() + "Star";

            foreach (var entity in _process.Entities) {
                entity.Value.RelationshipToMaster = ReadRelationshipToMaster(entity.Value);
            }

            LogProcessConfiguration();

            return _process;
        }

        private IEnumerable<Relationship> ReadRelationshipToMaster(Entity rightEntity) {
            var relationships = _process.Relationships.Where(r => r.RightEntity.Equals(rightEntity)).ToList();

            if (relationships.Any() && !relationships.Any(r => r.LeftEntity.IsMaster())) {
                var leftEntity = relationships.Last().LeftEntity;
                relationships.AddRange(ReadRelationshipToMaster(leftEntity));
            }
            return relationships;
        }

        private void LogProcessConfiguration() {
            Info("{0} | Process Loaded.", _process.Name);
            Info("{0} | {1} Connection{2}.", _process.Name, _connectionCount, _connectionCount == 1 ? string.Empty : "s");
            Info("{0} | {1} Map{2}.", _process.Name, _mapCount, _mapCount == 1 ? string.Empty : "s");
            Info("{0} | {1} Entit{2}.", _process.Name, _entityCount, _entityCount == 1 ? "y" : "ies");
            Info("{0} | {1} Relationship{2}.", _process.Name, _relationshipCount, _relationshipCount == 1 ? string.Empty : "s");
            Info("{0} | {1} Transform{2}.", _process.Name, _transformCount, _transformCount == 1 ? string.Empty : "s");
        }

        private void ReadProcess() {
            _process = new Process { Name = _config.Name };
        }

        private int ReadTransforms(ProcessConfigurationElement config) {
            _process.Transforms = GetTransforms(config.Transforms);
            foreach (var p in _process.Transforms.SelectMany(t => t.Parameters)) {
                _process.Parameters[p.Key] = p.Value;
            }
            foreach (var r in _process.Transforms.SelectMany(t => t.Results)) {
                _process.Results[r.Key] = r.Value;
            }
            return _process.Transforms.Count();
        }

        private int ReadRelationships(ProcessConfigurationElement config) {
            var count = 0;
            foreach (RelationshipConfigurationElement r in config.Relationships) {
                _process.Relationships.Add(new Relationship {
                    LeftEntity = _process.Entities[r.LeftEntity],
                    RightEntity = _process.Entities[r.RightEntity],
                    Join = GetJoins(r)
                });
                count++;
            }
            return count;
        }

        private List<Join> GetJoins(RelationshipConfigurationElement relationshipElement) {
            var join = new List<Join>();
            foreach (JoinConfigurationElement joinElement in relationshipElement.Join) {
                var j = new Join {
                    LeftField = _process.Entities[relationshipElement.LeftEntity].All[joinElement.LeftField],
                    RightField = _process.Entities[relationshipElement.RightEntity].All[joinElement.RightField]
                };
                j.LeftField.FieldType = FieldType.ForeignKey;
                join.Add(j);
            }
            return join;
        }

        private int ReadEntities(ProcessConfigurationElement config) {
            var count = 0;
            foreach (EntityConfigurationElement e in config.Entities) {
                var entity = GetEntity(e, count);
                _process.Entities.Add(e.Name, entity);
                if (entity.IsMaster())
                    _process.MasterEntity = entity;
                count++;
            }
            return count;
        }

        private IEnumerable<Field> ReadRelatedKeys() {
            var entity = _process.Entities.Select(e => e.Value).First(e => e.IsMaster());
            return GetRelatedKeys(entity);
        }

        private IEnumerable<Field> GetRelatedKeys(Entity entity) {
            var foreignKeys = entity.All.Where(f => f.Value.FieldType.Equals(FieldType.ForeignKey)).Select(f => f.Value).ToList();
            if (foreignKeys.Any()) {
                foreach (var alias in foreignKeys.Select(fk => fk.Alias).ToArray()) {
                    var nextEntity = _process.Relationships.Where(r => r.LeftEntity.Name.Equals(entity.Name) && r.Join.Any(j => j.LeftField.Alias.Equals(alias))).Select(r => r.RightEntity).First();
                    foreignKeys.AddRange(GetRelatedKeys(nextEntity));
                }
            }
            return foreignKeys;
        }

        private int ReadMaps(ProcessConfigurationElement config) {
            var count = 0;
            foreach (MapConfigurationElement m in config.Maps) {
                _process.MapEquals[m.Name] = GetMapItems(m.Items, "equals");
                _process.MapStartsWith[m.Name] = GetMapItems(m.Items, "startswith");
                _process.MapEndsWith[m.Name] = GetMapItems(m.Items, "endswith");
                count++;
            }
            return count;
        }

        private int ReadConnections(ProcessConfigurationElement config) {
            var count = 0;
            foreach (ConnectionConfigurationElement element in config.Connections) {
                var connection = new Connection(_connectionChecker) {
                    ConnectionString = element.Value,
                    Provider = element.Provider,
                    Year = element.Year,
                    OutputBatchSize = element.OutputBatchSize,
                    InputBatchSize = element.InputBatchSize,
                };
                _process.Connections.Add(element.Name, connection);
                count++;
            }
            return count;
        }

        private Entity GetEntity(EntityConfigurationElement e, int entityCount) {

            var entity = new Entity {
                ProcessName = _process.Name,
                Schema = e.Schema,
                Name = e.Name,
                InputConnection = _process.Connections[e.Connection],
                OutputConnection = _process.Connections["output"]
            };

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in e.PrimaryKey) {
                var fieldType = entityCount == 0 ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = GetField(entity, pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey.Add(pk.Alias, keyField);
                entity.All.Add(pk.Alias, keyField);

                if (e.Version.Equals(pk.Name)) {
                    entity.Version = keyField;
                }

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in e.Fields) {
                var field = GetField(entity, f);
                field.Index = fieldIndex;

                foreach (XmlConfigurationElement x in f.Xml) {
                    field.InnerXml.Add(x.Alias, new Field(x.Type, x.Length, FieldType.Xml, x.Output, x.Default) {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Parent = f.Name,
                        XPath = f.Xml.XPath + x.XPath,
                        Name = x.XPath,
                        Alias = x.Alias,
                        Index = x.Index,
                        Precision = x.Precision,
                        Scale = x.Scale,
                        Transforms = GetTransforms(x.Transforms)
                    });

                }

                entity.Fields.Add(f.Alias, field);
                entity.All.Add(f.Alias, field);

                if (e.Version.Equals(f.Name)) {
                    entity.Version = field;
                }

                fieldIndex++;
            }

            if (entity.Version == null) {
                var message = string.Format("{0} | version field reference '{1}' is undefined in {2}." , _process.Name, e.Version, e.Name);
                Error(message);
                throw new TransformalizeException(message);
            }

            return entity;
        }

        private static Dictionary<string, object> GetMapItems(IEnumerable items, string @operator) {
            var mapItems = new Dictionary<string, object>();
            foreach (var i in items.Cast<ItemConfigurationElement>().Where(i => i.Operator.Equals(@operator, StringComparison.OrdinalIgnoreCase))) {
                mapItems[i.From] = i.To;
            }
            return mapItems;
        }

        private Field GetField(Entity entity, FieldConfigurationElement field, FieldType fieldType = FieldType.Field) {
            return new Field(field.Type, field.Length, fieldType, field.Output, field.Default) {
                Entity = entity.Name,
                Schema = entity.Schema,
                Name = field.Name,
                Alias = field.Alias,
                Precision = field.Precision,
                Scale = field.Scale,
                Input = field.Input,
                Unicode = field.Unicode,
                VariableLength = field.VariableLength,
                Transforms = GetTransforms(field.Transforms)
            };
        }

        private Transformer[] GetTransforms(IEnumerable transforms) {
            var result = new List<Transformer>();

            foreach (TransformConfigurationElement t in transforms) {
                var parameters = t.Parameters.Cast<ParameterConfigurationElement>().Select(p => _process.Entities[p.Entity].All[p.Field]).ToDictionary(v => v.Alias, v => v);
                var results = new Dictionary<string, Field>();
                foreach (FieldConfigurationElement r in t.Results) {
                    var field = GetField(new Entity(), r);
                    results[field.Alias] = field;
                }

                switch (t.Method.ToLower()) {
                    case "replace":
                        result.Add(new ReplaceTransform(t.OldValue, t.NewValue, parameters, results));
                        break;
                    case "insert":
                        result.Add(new InsertTransform(t.Index, t.Value, parameters, results));
                        break;
                    case "remove":
                        result.Add(new RemoveTransform(t.StartIndex, t.Length, parameters, results));
                        break;
                    case "trimstart":
                        result.Add(new TrimStartTransform(t.TrimChars, parameters, results));
                        break;
                    case "trimend":
                        result.Add(new TrimEndTransform(t.TrimChars, parameters, results));
                        break;
                    case "trim":
                        result.Add(new TrimTransform(t.TrimChars, parameters, results));
                        break;
                    case "substring":
                        result.Add(new SubstringTransform(t.StartIndex, t.Length, parameters, results));
                        break;
                    case "left":
                        result.Add(new LeftTransform(t.Length, parameters, results));
                        break;
                    case "right":
                        result.Add(new RightTransform(t.Length, parameters, results));
                        break;
                    case "map":
                        var equals = _process.MapEquals[t.Map];
                        var startsWith = _process.MapStartsWith[t.Map];
                        var endsWith = _process.MapEndsWith[t.Map];
                        result.Add(new MapTransform(new[] { @equals, startsWith, endsWith }, parameters, results));
                        break;
                    case "javascript":
                        result.Add(new JavascriptTransform(t.Script, parameters, results));
                        break;
                    case "padleft":
                        result.Add(new PadLeftTransform(t.TotalWidth, t.PaddingChar, parameters, results));
                        break;
                    case "padright":
                        result.Add(new PadRightTransform(t.TotalWidth, t.PaddingChar, parameters, results));
                        break;
                    case "format":
                        result.Add(new FormatTransform(t.Format, parameters, results));
                        break;
                    case "toupper":
                        result.Add(new ToUpperTransform());
                        break;
                    case "tolower":
                        result.Add(new ToLowerTransform());
                        break;
                    case "concat":
                        result.Add(new ConcatTransform(parameters, results));
                        break;
                }
            }

            return result.ToArray();
        }

    }
}