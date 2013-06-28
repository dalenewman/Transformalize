using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Transforms;
using System.Text;
using System.Linq;

namespace Transformalize.Readers {

    public class ProcessReader : WithLoggingMixin, IProcessReader {
        private readonly string _name;
        private Process _process;

        public ProcessReader(string name) {
            _name = name;
        }

        public Process GetProcess() {

            var configCollection = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var config = configCollection.Processes.Get(_name);
            var entityCount = 0;

            _process = new Process { Name = config.Name, Output = config.Output, Time = config.Time };

            //shared connections
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

            //shared maps
            foreach (MapConfigurationElement m in config.Maps) {
                _process.MapEquals[m.Name] = GetMapItems(m.Items, "equals");
                _process.MapStartsWith[m.Name] = GetMapItems(m.Items, "startswith");
                _process.MapEndsWith[m.Name] = GetMapItems(m.Items, "endswith");
            }

            foreach (EntityConfigurationElement e in config.Entities) {
                entityCount++;
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
                    var keyField = new Field(pk.Type, pk.Length, fieldType, pk.Output) {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = pk.Name,
                        Alias = pk.Alias,
                        Precision = pk.Precision,
                        Scale = pk.Scale,
                        Input = pk.Input,
                        Default = pk.Default,
                        Transforms = GetTransforms(pk.Transforms)
                    };

                    entity.PrimaryKey.Add(pk.Alias, keyField);
                    entity.All.Add(pk.Alias, keyField);

                    if (e.Version.Equals(pk.Name)) {
                        entity.Version = keyField;
                    }
                }

                foreach (FieldConfigurationElement f in e.Fields) {
                    var field = new Field(f.Type, f.Length, FieldType.Field, f.Output) {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = f.Name,
                        Alias = f.Alias,
                        Precision = f.Precision,
                        Scale = f.Scale,
                        Input = f.Input,
                        Default = f.Default,
                        Transforms = GetTransforms(f.Transforms)
                    };

                    foreach (XmlConfigurationElement x in f.Xml) {
                        field.InnerXml.Add(x.Alias, new Xml(x.Type, x.Length, x.Output) {
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
                            Default = x.Default,
                            Transforms = GetTransforms(x.Transforms)
                        });

                    }

                    entity.Fields.Add(f.Alias, field);
                    entity.All.Add(f.Alias, field);

                    if (e.Version.Equals(f.Name)) {
                        entity.Version = field;
                    }
                }

                _process.Entities.Add(e.Name, entity);
            }

            foreach (RelationshipConfigurationElement joinElement in config.Relationships) {
                var join = new Relationship();
                join.LeftEntity = _process.Entities[joinElement.LeftEntity];
                join.LeftField = join.LeftEntity.All[joinElement.LeftField];
                join.LeftField.FieldType = FieldType.ForeignKey;
                join.RightEntity = _process.Entities[joinElement.RightEntity];
                join.RightField = join.RightEntity.All[joinElement.RightField];
                _process.Joins.Add(join);
            }

            Info("{0} | Process Loaded.", _process.Name);
            return _process;
        }

        private Dictionary<string, object> GetMapItems(ItemElementCollection items, string @operator) {
            var mapItems = new Dictionary<string, object>();
            foreach (var i in items.Cast<ItemConfigurationElement>().Where(i => i.Operator.Equals(@operator, StringComparison.OrdinalIgnoreCase))) {
                mapItems[i.From] = i.To;
            }
            return mapItems;
        }

        private ITransform[] GetTransforms(IEnumerable transforms) {
            var result = new List<ITransform>();

            foreach (TransformConfigurationElement t in transforms) {
                switch (t.Method.ToLower()) {
                    case "replace":
                        result.Add(new ReplaceTransform(t.OldValue, t.NewValue));
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
                        result.Add(new JavascriptTransform(t.Script));
                        break;
                }
            }

            return result.ToArray();
        }
    }
}