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
        private readonly EntityConfigurationElement _element;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public EntityConfigurationReader(EntityConfigurationElement element)
        {
            _element = element;
        }

        public Entity Read(int count)
        {
            var entity = new Entity
                {
                    ProcessName = Process.Name,
                    Schema = _element.Schema,
                    Name = _element.Name,
                    InputConnection = Process.Connections[_element.Connection],
                    OutputConnection = Process.Connections["output"],
                    Prefix = _element.Prefix == "Default" ? _element.Name.Replace(" ", string.Empty) : _element.Prefix,
                    Group = _element.Group,
                    Auto = _element.Auto,
                    Alias = string.IsNullOrEmpty(_element.Alias) ? _element.Name : _element.Alias,
                };

            if (entity.Auto)
            {
                var autoReader = new SqlServerEntityAutoFieldReader(entity, count);
                entity.All = autoReader.ReadAll();
                entity.Fields = autoReader.ReadFields();
                entity.PrimaryKey = autoReader.ReadPrimaryKey();
            }

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in _element.PrimaryKey)
            {
                var fieldType = count == 0 ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = new FieldReader(entity, new FieldTransformParametersReader(pk.Alias), new EmptyParametersReader()).Read(pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey[pk.Alias] = keyField;
                entity.All[pk.Alias] = keyField;

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in _element.Fields)
            {
                var field = new FieldReader(entity, new FieldTransformParametersReader(f.Alias), new EmptyParametersReader()).Read(f);
                field.Index = fieldIndex;

                foreach (XmlConfigurationElement x in f.Xml)
                {
                    var xmlField = new FieldReader(entity, new FieldTransformParametersReader(x.Alias), new EmptyParametersReader()).Read(x, f);
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

            if (entity.All.ContainsKey(_element.Version))
            {
                entity.Version = entity.All[_element.Version];
            }
            else
            {
                if (entity.All.Any(kv => kv.Value.Name.Equals(_element.Version, IC)))
                {
                    entity.Version = entity.All.ToEnumerable().First(v => v.Name.Equals(_element.Version, IC));
                }
                else
                {
                    _log.Error("{0} | version field reference '{1}' is undefined in {2}.", Process.Name, _element.Version, _element.Name);
                    Environment.Exit(0);
                }
            }

            foreach (FieldConfigurationElement field in _element.CalculatedFields)
            {
                entity.CalculatedFields.Add(field.Alias, new FieldReader(null, new EntityTransformParametersReader(entity), new EntityParametersReader(entity)).Read(field));
            }

            return entity;
        }

    }
}