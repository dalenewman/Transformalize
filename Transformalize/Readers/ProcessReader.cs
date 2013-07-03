using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Transforms;
using System.Linq;

namespace Transformalize.Readers {

    public class ProcessReader : WithLoggingMixin, IProcessReader {
        private readonly string _name;
        private Process _process;
        private readonly ProcessConfigurationElement _config;

        public ProcessReader(string name) {
            _name = name;
            _config = ((TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize")).Processes.Get(_name);
        }

        public Process GetProcess() {
            ReadProcess();
            ReadConnections(_config);
            ReadMaps(_config);
            ReadEntities(_config);
            ReadRelationships(_config);
            ReadTransforms(_config);
            return ReturnProcess();
        }

        public Process GetSingleEntityProcess(string name) {
            ReadProcess();
            ReadConnections(_config);
            ReadMaps(_config);
            ReadEntity(_config, name);

            var output = string.Format("Tfl{0}", name);
            _process.Entities[name].Output = output;
            _process.Output = output;

            return ReturnProcess();
        }

        private Process ReturnProcess() {
            Info("{0} | Process Loaded.", _process.Name);
            return _process;
        }

        private void ReadProcess() {
            _process = new Process { Name = _config.Name, Output = _config.Output, Time = _config.Time };
        }

        private void ReadTransforms(ProcessConfigurationElement config) {
            _process.Transforms = GetTransforms(config.Transforms);
            foreach (var p in _process.Transforms.SelectMany(t => t.Parameters)) {
                _process.Parameters[p.Key] = p.Value;
            }
            foreach (var r in _process.Transforms.SelectMany(t => t.Results)) {
                _process.Results[r.Key] = r.Value;
            }
        }

        private void ReadRelationships(ProcessConfigurationElement config) {
            foreach (RelationshipConfigurationElement joinElement in config.Relationships) {
                var j = new Relationship { LeftEntity = _process.Entities[joinElement.LeftEntity] };
                j.LeftField = j.LeftEntity.All[joinElement.LeftField];
                j.LeftField.FieldType = FieldType.ForeignKey;
                j.RightEntity = _process.Entities[joinElement.RightEntity];
                j.RightField = j.RightEntity.All[joinElement.RightField];
                _process.Joins.Add(j);
            }
        }

        private void ReadEntities(ProcessConfigurationElement config) {
            var entityCount = 1;
            foreach (EntityConfigurationElement e in config.Entities) {
                _process.Entities.Add(e.Name, GetEntity(e, entityCount));
                entityCount++;
            }
        }

        private void ReadEntity(ProcessConfigurationElement config, string name) {
            var entity = GetEntity(config.Entities.Cast<EntityConfigurationElement>().First(e => e.Name == name), 1);
            _process.Entities.Add(entity.Name, entity);
        }

        private void ReadMaps(ProcessConfigurationElement config) {
            foreach (MapConfigurationElement m in config.Maps) {
                _process.MapEquals[m.Name] = GetMapItems(m.Items, "equals");
                _process.MapStartsWith[m.Name] = GetMapItems(m.Items, "startswith");
                _process.MapEndsWith[m.Name] = GetMapItems(m.Items, "endswith");
            }
        }

        private void ReadConnections(ProcessConfigurationElement config) {
            foreach (ConnectionConfigurationElement element in config.Connections) {
                var connection = new Connection {
                    ConnectionString = element.Value,
                    Provider = element.Provider,
                    Year = element.Year,
                    OutputBatchSize = element.OutputBatchSize,
                    InputBatchSize = element.InputBatchSize,
                };
                _process.Connections.Add(element.Name, connection);
                if (element.Name.Equals("output", StringComparison.OrdinalIgnoreCase)) {
                    _process.OutputConnection = connection;
                }
            }
        }

        private Entity GetEntity(EntityConfigurationElement e, int entityCount) {

            var entity = new Entity {
                ProcessName = _process.Name,
                Schema = e.Schema,
                Name = e.Name,
                InputConnection = _process.Connections[e.Connection],
                OutputConnection = _process.OutputConnection,
                Output = _process.Output
            };

            foreach (FieldConfigurationElement pk in e.PrimaryKey) {
                var fieldType = entityCount == 1 ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = GetField(entity, pk, fieldType);

                entity.PrimaryKey.Add(pk.Alias, keyField);
                entity.All.Add(pk.Alias, keyField);

                if (e.Version.Equals(pk.Name)) {
                    entity.Version = keyField;
                }
            }

            foreach (FieldConfigurationElement f in e.Fields) {
                var field = GetField(entity, f);

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
                        Input = true,
                        Transforms = GetTransforms(x.Transforms)
                    });

                }

                entity.Fields.Add(f.Alias, field);
                entity.All.Add(f.Alias, field);

                if (e.Version.Equals(f.Name)) {
                    entity.Version = field;
                }
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
                Transforms = GetTransforms(field.Transforms)
            };
        }

        private ITransform[] GetTransforms(IEnumerable transforms) {
            var result = new List<ITransform>();

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
                }
            }

            return result.ToArray();
        }
    }
}