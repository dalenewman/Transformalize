using System;
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

            var process = new Process { Name = config.Name, Output = config.Output, Time = config.Time };

            foreach (ConnectionConfigurationElement element in config.Connections) {
                var connection = new Connection {
                    ConnectionString = element.Value,
                    Provider = element.Provider,
                    Year = element.Year,
                    BatchInsertSize = element.BatchInsertSize,
                    BulkInsertSize = element.BulkInsertSize,
                    BatchUpdateSize = element.BatchUpdateSize
                };
                process.Connections.Add(element.Name, connection);
                if (element.Name.Equals("output", StringComparison.OrdinalIgnoreCase)) {
                    process.OutputConnection = connection;
                }
            }

            foreach (EntityConfigurationElement entityElement in config.Entities) {
                var entity = new Entity {
                    ProcessName = process.Name,
                    Schema = entityElement.Schema,
                    Name = entityElement.Name,
                    InputConnection = process.Connections[entityElement.Connection],
                    OutputConnection = process.OutputConnection
                };

                foreach (FieldConfigurationElement fieldElement in entityElement.Keys) {
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
                        FieldType = FieldType.Key
                    };
                    entity.Keys.Add(fieldElement.Alias, keyField);
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
                        FieldType = FieldType.Field
                    };
                    foreach (XmlConfigurationElement xmlElement in fieldElement.Xml) {
                        field.Xml.Add(xmlElement.Alias, new Xml {
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
                            Output = xmlElement.Output
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
                join.RightEntity = process.Entities[joinElement.RightEntity];
                join.RightField = join.RightEntity.All[joinElement.RightField];
                process.Joins.Add(join);
            }

            return process;
        }

    }
}