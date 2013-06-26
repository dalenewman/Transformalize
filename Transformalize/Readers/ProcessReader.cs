using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Transforms;
using System.Text;

namespace Transformalize.Readers {

    public class ProcessReader : IProcessReader {
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

                _process.MapStartsWith[m.Name] = new Dictionary<string, object>();
                _process.MapEndsWith[m.Name] = new Dictionary<string, object>();
                _process.MapEquals[m.Name] = new Dictionary<string, object>();

                foreach (ItemConfigurationElement i in m.Items) {

                    switch (i.Operator.ToLower()) {
                        case "startswith":
                            _process.MapStartsWith[m.Name][i.From] = i.To;
                            break;
                        case "endswith":
                            _process.MapEndsWith[m.Name][i.From] = i.To;
                            break;
                        default:
                            _process.MapEquals[m.Name][i.From] = i.To;
                            break;
                    }
                }
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
                    var keyField = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = pk.Name,
                        Type = pk.Type,
                        Alias = pk.Alias,
                        Length = pk.Length,
                        Precision = pk.Precision,
                        Scale = pk.Scale,
                        Output = pk.Output,
                        Input = pk.Input,
                        FieldType = entityCount == 1 ? FieldType.MasterKey : FieldType.PrimaryKey,
                        Default = pk.Default,
                        StringBuilder = new StringBuilder(pk.Length, 4000),
                        Transforms = GetTransforms(pk.Transforms)
                    };

                    entity.PrimaryKey.Add(pk.Alias, keyField);
                    entity.All.Add(pk.Alias, keyField);

                    if (e.Version.Equals(pk.Name)) {
                        entity.Version = keyField;
                    }
                }

                foreach (FieldConfigurationElement f in e.Fields) {
                    var field = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = f.Name,
                        Type = f.Type,
                        Alias = f.Alias,
                        Length = f.Length,
                        Precision = f.Precision,
                        Scale = f.Scale,
                        Output = f.Output,
                        Input = f.Input,
                        FieldType = FieldType.Field,
                        Default = f.Default,
                        StringBuilder = new StringBuilder(f.Length, 4000),
                        Transforms = GetTransforms(f.Transforms)
                    };

                    foreach (XmlConfigurationElement x in f.Xml) {
                        field.InnerXml.Add(x.Alias, new Xml {
                            Entity = entity.Name,
                            Schema = entity.Schema,
                            Parent = f.Name,
                            XPath = f.Xml.XPath + x.XPath,
                            Name = x.XPath,
                            Alias = x.Alias,
                            Index = x.Index,
                            Type = x.Type,
                            Length = x.Length,
                            Precision = x.Precision,
                            Scale = x.Scale,
                            Output = x.Output,
                            Input = true,
                            Default = x.Default,
                            FieldType = FieldType.Xml,
                            StringBuilder = new StringBuilder(x.Length, 4000),
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

            foreach (JoinConfigurationElement joinElement in config.Joins) {
                var join = new Join();
                join.LeftEntity = _process.Entities[joinElement.LeftEntity];
                join.LeftField = join.LeftEntity.All[joinElement.LeftField];
                join.LeftField.FieldType = FieldType.ForeignKey;
                join.RightEntity = _process.Entities[joinElement.RightEntity];
                join.RightField = join.RightEntity.All[joinElement.RightField];
                _process.Joins.Add(join);
            }

            return _process;
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
                }
            }

            return result.ToArray();
        }
    }
}