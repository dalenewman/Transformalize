using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Core.Entity_
{
    public class EntityConfigurationReader : IEntityReader
    {
        private readonly Process _process;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public EntityConfigurationReader(Process process)
        {
            _process = process;
        }

        public Entity Read(EntityConfigurationElement element, bool isMaster)
        {
            var entity = new Entity
                {
                    ProcessName = _process.Name,
                    Schema = element.Schema,
                    Name = element.Name,
                    InputConnection = _process.Connections[element.Connection],
                    OutputConnection = _process.Connections["output"],
                    Prefix = element.Prefix == "Default" ? element.Name.Replace(" ", string.Empty) : element.Prefix,
                    Group = element.Group,
                    Auto = element.Auto,
                    Alias = string.IsNullOrEmpty(element.Alias) ? element.Name : element.Alias,
                };

            if (entity.Auto)
            {
                var autoReader = new SqlServerEntityAutoFieldReader();
                entity.All = autoReader.Read(entity, isMaster);
                entity.Fields = new FieldSqlWriter(entity.All).FieldType(FieldType.Field).Context();
                entity.PrimaryKey = new FieldSqlWriter(entity.All).FieldType(FieldType.PrimaryKey, FieldType.MasterKey).Context();
            }

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in element.PrimaryKey)
            {
                var fieldType = isMaster ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = new FieldReader(_process, entity, new FieldTransformParametersReader(pk.Alias), new EmptyParametersReader()).Read(pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey[pk.Alias] = keyField;
                entity.All[pk.Alias] = keyField;

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in element.Fields)
            {
                var field = new FieldReader(_process, entity, new FieldTransformParametersReader(f.Alias), new EmptyParametersReader()).Read(f);
                field.Index = fieldIndex;

                foreach (XmlConfigurationElement x in f.Xml)
                {
                    var xmlField = new FieldReader(_process, entity, new FieldTransformParametersReader(x.Alias), new EmptyParametersReader()).Read(x, f);
                    field.InnerXml.Add(x.Alias, xmlField);
                }

                if (entity.Auto && entity.Fields.ContainsKey(field.Name))
                {
                    entity.Fields.Remove(field.Name);
                    entity.All.Remove(field.Name);
                }

                entity.Fields[f.Alias] = field;
                entity.All[f.Alias] = field;

                fieldIndex++;
            }

            if (!String.IsNullOrEmpty(element.Version))
            {
                if (entity.All.ContainsKey(element.Version))
                {
                    entity.Version = entity.All[element.Version];
                }
                else
                {
                    if (entity.All.Any(kv => kv.Value.Name.Equals(element.Version, IC)))
                    {
                        entity.Version = entity.All.ToEnumerable().First(v => v.Name.Equals(element.Version, IC));
                    }
                    else
                    {
                        _log.Error("version field reference '{0}' is undefined in {1}.", element.Version, element.Name);
                        Environment.Exit(0);
                    }
                }
            }

            foreach (FieldConfigurationElement field in element.CalculatedFields)
            {
                entity.CalculatedFields.Add(field.Alias, new FieldReader(_process, entity, new EntityTransformParametersReader(entity), new EntityParametersReader(entity)).Read(field));
            }

            return entity;
        }

    }
}