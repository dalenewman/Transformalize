using System;
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;

namespace Transformalize.Readers {

    public class ProcessReader : IProcessReader {
        private readonly string _name;

        public ProcessReader(string name) {
            _name = name;
        }

        public Process GetProcess() {

            var configCollection = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var config = configCollection.Processes.Get(_name);
            var entityCount = 0;

            var process = new Process { Name = config.Name, Output = config.Output, Time = config.Time };

            foreach (ConnectionConfigurationElement element in config.Connections) {
                var connection = new Connection {
                    ConnectionString = element.Value,
                    Provider = element.Provider,
                    Year = element.Year,
                    BatchInsertSize = element.BatchInsertSize,
                    BulkInsertSize = element.BulkInsertSize,
                    BatchUpdateSize = element.BatchUpdateSize,
                    BatchSelectSize = element.BatchSelectSize,
                };
                process.Connections.Add(element.Name, connection);
                if (element.Name.Equals("output", StringComparison.OrdinalIgnoreCase)) {
                    process.OutputConnection = connection;
                }
            }

            foreach (EntityConfigurationElement entityElement in config.Entities) {
                entityCount++;
                var entity = new Entity {
                    ProcessName = process.Name,
                    Schema = entityElement.Schema,
                    Name = entityElement.Name,
                    InputConnection = process.Connections[entityElement.Connection],
                    OutputConnection = process.OutputConnection,
                    Output = process.Output
                };

                foreach (FieldConfigurationElement fieldElement in entityElement.PrimaryKey) {
                    var keyField = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = fieldElement.Name,
                        Type = fieldElement.Type,
                        Alias = fieldElement.Alias,
                        Length = fieldElement.Length,
                        Precision = fieldElement.Precision,
                        Scale = fieldElement.Scale,
                        Output = fieldElement.Output,
                        Input = fieldElement.Input,
                        FieldType = entityCount == 1 ? FieldType.MasterKey : FieldType.PrimaryKey,
                        Default = fieldElement.Default
                    };
                    entity.PrimaryKey.Add(fieldElement.Alias, keyField);
                    entity.All.Add(fieldElement.Alias, keyField);

                    if (entityElement.Version.Equals(fieldElement.Name)) {
                        entity.Version = keyField;
                    }
                }

                foreach (FieldConfigurationElement fieldElement in entityElement.Fields) {
                    var field = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = fieldElement.Name,
                        Type = fieldElement.Type,
                        Alias = fieldElement.Alias,
                        Length = fieldElement.Length,
                        Precision = fieldElement.Precision,
                        Scale = fieldElement.Scale,
                        Output = fieldElement.Output,
                        Input = fieldElement.Input,
                        FieldType = FieldType.Field,
                        Default = fieldElement.Default
                    };
                    foreach (XmlConfigurationElement xmlElement in fieldElement.Xml) {
                        field.InnerXml.Add(xmlElement.Alias, new Xml {
                            Entity = entity.Name,
                            Schema = entity.Schema,
                            Parent = fieldElement.Name,
                            XPath = fieldElement.Xml.XPath + xmlElement.XPath,
                            Name = xmlElement.XPath,
                            Alias = xmlElement.Alias,
                            Index = xmlElement.Index,
                            Type = xmlElement.Type,
                            Length = xmlElement.Length,
                            Precision = xmlElement.Precision,
                            Scale = xmlElement.Scale,
                            Output = xmlElement.Output,
                            Input = true,
                            Default = fieldElement.Default,
                            FieldType = FieldType.Xml
                        });
                    }

                    entity.Fields.Add(fieldElement.Alias, field);
                    entity.All.Add(fieldElement.Alias, field);

                    if (entityElement.Version.Equals(fieldElement.Name)) {
                        entity.Version = field;
                    }
                }

                process.Entities.Add(entityElement.Name, entity);
            }

            foreach (JoinConfigurationElement joinElement in config.Joins) {
                var join = new Join();
                join.LeftEntity = process.Entities[joinElement.LeftEntity];
                join.LeftField = join.LeftEntity.All[joinElement.LeftField];
                join.LeftField.FieldType = FieldType.ForeignKey;
                join.RightEntity = process.Entities[joinElement.RightEntity];
                join.RightField = join.RightEntity.All[joinElement.RightField];

                join.LeftField.References = new KeyValuePair<string, string>(join.RightField.Name, join.RightField.Alias);
                join.RightField.References = new KeyValuePair<string, string>(join.LeftEntity.Name, join.LeftField.Alias);
                
                process.Joins.Add(join);
            }

            return process;
        }

    }
}