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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Model;
using Transformalize.Transforms;

namespace Transformalize.Data {

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

        public ProcessReader(string name, IConnectionChecker connectionChecker = null, IEntityAutoFieldReader autoFieldReader = null) {
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
                entity.RelationshipToMaster = ReadRelationshipToMaster(entity);
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
            Debug("{0} | Process Loaded.", _process.Name);
            Debug("{0} | {1} Connection{2}.", _process.Name, _connectionCount, _connectionCount == 1 ? string.Empty : "s");
            Debug("{0} | {1} Map{2}.", _process.Name, _mapCount, _mapCount == 1 ? string.Empty : "s");
            Debug("{0} | {1} Entit{2}.", _process.Name, _entityCount, _entityCount == 1 ? "y" : "ies");
            Debug("{0} | {1} Relationship{2}.", _process.Name, _relationshipCount, _relationshipCount == 1 ? string.Empty : "s");

            _transformCount += _process.Entities.SelectMany(e => e.All).SelectMany(f => f.Value.Transforms).Count();
            Debug("{0} | {1} Transform{2}.", _process.Name, _transformCount, _transformCount == 1 ? string.Empty : "s");
        }

        private void ReadProcess() {
            _process = new Process { Name = _config.Name };
        }

        private int ReadTransforms(ProcessConfigurationElement config) {
            var alternates = new Dictionary<string, Field>();
            foreach (var entity in _process.Entities) {
                foreach (var field in entity.All) {
                    alternates[field.Value.Alias] = field.Value;
                }
            }

            _process.Transforms = GetTransforms(config.Transforms, new FieldSqlWriter(alternates).ExpandXml().Input().Context());
            foreach (var transform in _process.Transforms) {
                foreach (var pair in transform.Parameters) {
                    _process.Parameters.Add(pair.Key, pair.Value);
                }
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
                    LeftEntity = _process.Entities.First(e => e.Name.Equals(r.LeftEntity, StringComparison.OrdinalIgnoreCase)),
                    RightEntity = _process.Entities.First(e => e.Name.Equals(r.RightEntity, StringComparison.OrdinalIgnoreCase)),
                    Join = GetJoins(r)
                });
                count++;
            }
            return count;
        }

        private List<Join> GetJoins(RelationshipConfigurationElement relationshipElement) {
            var join = new List<Join>();
            foreach (JoinConfigurationElement joinElement in relationshipElement.Join) {
                var leftEntity = _process.Entities.First(e => e.Name.Equals(relationshipElement.LeftEntity, StringComparison.OrdinalIgnoreCase));
                var rightEntity = _process.Entities.First(e => e.Name.Equals(relationshipElement.RightEntity, StringComparison.OrdinalIgnoreCase));
                var j = new Join {
                    LeftField = leftEntity.All.ContainsKey(joinElement.LeftField) ? leftEntity.All[joinElement.LeftField] : leftEntity.All.Select(kv=>kv.Value).First(v=>v.Name.Equals(joinElement.LeftField, StringComparison.OrdinalIgnoreCase)),
                    RightField = rightEntity.All.ContainsKey(joinElement.RightField) ? rightEntity.All[joinElement.RightField] : rightEntity.All.Select(kv=>kv.Value).First(v=>v.Name.Equals(joinElement.RightField, StringComparison.OrdinalIgnoreCase))
                };

                if (j.LeftField.FieldType.HasFlag(FieldType.MasterKey) ||
                    j.LeftField.FieldType.HasFlag(FieldType.PrimaryKey)) {
                    j.LeftField.FieldType |= FieldType.ForeignKey;
                }
                else {
                    j.LeftField.FieldType = FieldType.ForeignKey;
                }

                join.Add(j);
            }
            return join;
        }

        private int ReadEntities(ProcessConfigurationElement config) {
            var count = 0;
            foreach (EntityConfigurationElement e in config.Entities) {
                var entity = GetEntity(e, count);
                _process.Entities.Add(entity);
                if (entity.IsMaster())
                    _process.MasterEntity = entity;
                count++;
            }
            return count;
        }

        private IEnumerable<Field> ReadRelatedKeys() {
            var entity = _process.Entities.First(e => e.IsMaster());
            return GetRelatedKeys(entity);
        }

        private IEnumerable<Field> GetRelatedKeys(Entity entity) {
            var foreignKeys = entity.All.Where(f => f.Value.FieldType.HasFlag(FieldType.ForeignKey)).Select(f => f.Value).ToList();
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
                var connection = new SqlServerConnection(element.Value, _process.Name) {
                    Provider = element.Provider,
                    CompatibilityLevel = element.CompatabilityLevel,
                    BatchSize = element.BatchSize
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
                OutputConnection = _process.Connections["output"],
                Prefix = e.Prefix == "Default" ? e.Name.Replace(" ", string.Empty) : e.Prefix,
                Group = e.Group,
                Auto = e.Auto
            };

            if (entity.Auto) {
                var autoReader = new SqlServerEntityAutoFieldReader(entity, entityCount);
                entity.All = autoReader.ReadAll();
                entity.Fields = autoReader.ReadFields();
                entity.PrimaryKey = autoReader.ReadPrimaryKey();
            }

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in e.PrimaryKey) {
                var fieldType = entityCount == 0 ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = GetField(entity, pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey[pk.Alias] = keyField;
                entity.All[pk.Alias] = keyField;

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in e.Fields) {
                var field = GetField(entity, f);
                field.Index = fieldIndex;

                foreach (XmlConfigurationElement x in f.Xml) {
                    var xmlField = new Field(x.Type, x.Length, FieldType.Xml, x.Output, x.Default) {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Parent = f.Name,
                        XPath = f.Xml.XPath + x.XPath,
                        Name = x.XPath,
                        Alias = x.Alias,
                        Index = x.Index,
                        Precision = x.Precision,
                        Scale = x.Scale,
                        Aggregate = x.Aggregate.ToLower()
                    };
                    xmlField.Transforms = GetTransforms(x.Transforms, new FieldSqlWriter(xmlField).Input().Context());
                    field.InnerXml.Add(x.Alias, xmlField);
                }

                if (entity.Auto && entity.Fields.ContainsKey(field.Name)) {
                    entity.Fields.Remove(field.Name);
                    entity.All.Remove(field.Name);
                }

                entity.Fields[f.Alias] = field;
                entity.All[f.Alias] = field;

                fieldIndex++;
            }

            if (entity.All.ContainsKey(e.Version)) {
                entity.Version = entity.All[e.Version];
            }
            else {
                if (entity.All.Any(kv => kv.Value.Name.Equals(e.Version, StringComparison.OrdinalIgnoreCase))) {
                    entity.Version = entity.All.Select(kv => kv.Value).First(v => v.Name.Equals(e.Version, StringComparison.OrdinalIgnoreCase));
                } else {
                    var message = string.Format("{0} | version field reference '{1}' is undefined in {2}.", _process.Name, e.Version, e.Name);
                    Error(message);
                    throw new TransformalizeException(message);
                }
            }

            var entityKeys = new HashSet<string>(entity.Fields.Where(kv => kv.Value.Output).Select(kv => kv.Key));
            var processKeys = new HashSet<string>(_process.Entities.SelectMany(pe => pe.Fields).Where(kv => kv.Value.Output).Select(kv => kv.Key));
            entityKeys.IntersectWith(processKeys);
            if (entityKeys.Any()) {
                var count = entityKeys.Count;
                var message = String.Format("{0} | field overlap error.  The field{2}: {1} {3} already defined in previous entities.  You must alias (rename) these.", _process.Name, string.Join(", ", entityKeys), count == 1 ? string.Empty : "s", count == 1 ? "is" : "are");
                throw new TransformalizeException(message);
            }

            entity.Transforms = GetTransforms(e.Transforms, new FieldSqlWriter(entity.All).ExpandXml().Input().Context());

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
            var newField = new Field(field.Type, field.Length, fieldType, field.Output, field.Default) {
                Entity = entity.Name,
                Schema = entity.Schema,
                Name = field.Name,
                Alias = field.Alias,
                Precision = field.Precision,
                Scale = field.Scale,
                Input = field.Input,
                Unicode = field.Unicode,
                VariableLength = field.VariableLength,
                Aggregate = field.Aggregate.ToLower()
            };
            newField.Transforms = GetTransforms(field.Transforms, new FieldSqlWriter(newField).ExpandXml().Input().Context());
            return newField;
        }

        private AbstractTransform[] GetTransforms(IEnumerable transforms, Dictionary<string, Field> defaultParameters) {
            var result = new List<AbstractTransform>();

            foreach (TransformConfigurationElement t in transforms) {
                var parameters = GetParameters(t);

                var results = new Dictionary<string, Field>();
                foreach (FieldConfigurationElement r in t.Results) {
                    var field = GetField(new Entity(), r);
                    results[field.Alias] = field;
                }
                if (!parameters.Any()) {
                    foreach (var p in defaultParameters) {
                        parameters.Add(p.Key, p.Key, null, "System.Object");
                    }
                }

                switch (t.Method.ToLower()) {
                    case "replace":
                        result.Add(new ReplaceTransform(t.OldValue, t.NewValue));
                        break;
                    case "regexreplace":
                        result.Add(new RegexReplaceTransform(t.Pattern, t.Replacement, t.Count));
                        break;
                    case "insert":
                        result.Add(new InsertTransform(t.Index, t.Value));
                        break;
                    case "remove":
                        result.Add(new RemoveTransform(t.StartIndex, t.Length));
                        break;
                    case "trimstart":
                        result.Add(new TrimStartTransform(t.TrimChars));
                        break;
                    case "trimend":
                        result.Add(new TrimEndTransform(t.TrimChars));
                        break;
                    case "trim":
                        result.Add(new TrimTransform(t.TrimChars));
                        break;
                    case "substring":
                        result.Add(new SubstringTransform(t.StartIndex, t.Length));
                        break;
                    case "left":
                        result.Add(new LeftTransform(t.Length));
                        break;
                    case "right":
                        result.Add(new RightTransform(t.Length));
                        break;
                    case "map":
                        var equals = _process.MapEquals[t.Map];
                        var startsWith = _process.MapStartsWith[t.Map];
                        var endsWith = _process.MapEndsWith[t.Map];
                        result.Add(new MapTransform(new[] { @equals, startsWith, endsWith }));
                        break;
                    case "javascript":
                        result.Add(new JavascriptTransform(t.Script, parameters, results));
                        break;
                    case "template":
                        result.Add(new TemplateTransform(t.Template, parameters, results));
                        break;
                    case "padleft":
                        result.Add(new PadLeftTransform(t.TotalWidth, t.PaddingChar));
                        break;
                    case "padright":
                        result.Add(new PadRightTransform(t.TotalWidth, t.PaddingChar));
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
                    case "join":
                        result.Add(new JoinTransform(t.Separator, parameters, results));
                        break;
                    case "split":
                        result.Add(new SplitTransform(t.Separator, parameters, results));
                        break;
                    case "copy":
                        result.Add(new CopyTransform(parameters, results));
                        break;
                }
            }

            return result.ToArray();
        }

        private IParameters GetParameters(TransformConfigurationElement t) {

            var parameters = new Parameters();

            foreach (ParameterConfigurationElement p in t.Parameters) {
                if (!string.IsNullOrEmpty(p.Entity) && !string.IsNullOrEmpty(p.Field)) {
                    if (_process.Entities.Any(e => e.Name.Equals(p.Entity))) {
                        var entity = _process.Entities.Find(e => e.Name.Equals(p.Entity));
                        if (entity.All.ContainsKey(p.Field)) {
                            var field = entity.All[p.Field];
                            var key = String.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                            parameters.Add(field.Alias, key, null, "System.Object");
                        }
                        else {
                            var message = string.Format("A {0} parameter references the field {1} in entity {2}.  The field {1} does not exist.", t.Method, p.Field, p.Entity);
                            throw new TransformalizeException(message);
                        }
                    }
                    else {
                        var message = string.Format("A {0} parameter references the entity {1}.  The entity {1} does not exist.", t.Method, p.Entity);
                        throw new TransformalizeException(message);
                    }
                }
                else {
                    if (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)) {
                        var message = string.Format("A {0} parameter does not reference an entity and field, nor does it have a name and value.  It must do have one or the other.", t.Method);
                        throw new TransformalizeException(message);
                    }
                    parameters.Add(p.Name, p.Name, p.Value, p.Type);
                }
            }
            return parameters;
        }
    }
}