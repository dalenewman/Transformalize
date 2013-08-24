using Transformalize.Configuration;
using Transformalize.Core.Entity_;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Process_;
using System.Linq;

namespace Transformalize.Core.Field_
{
    public class FieldReader : IFieldReader
    {
        private readonly Entity _entity;

        public FieldReader(Entity entity)
        {
            _entity = entity ?? new Entity();
        }

        public Field Read(FieldConfigurationElement field, FieldType fieldType = FieldType.Field)
        {
            var f = new Field(field.Type, field.Length, fieldType, field.Output, field.Default)
                        {
                            Process = Process.Name,
                            Entity = _entity.Alias,
                            Schema = _entity.Schema,
                            Name = field.Name,
                            Alias = field.Alias,
                            Precision = field.Precision,
                            Scale = field.Scale,
                            Input = field.Input,
                            Unicode = field.Unicode,
                            VariableLength = field.VariableLength,
                            Aggregate = field.Aggregate.ToLower(),
                            AsParameter = new Parameter(field.Alias, null)
                        };
            f.Transforms = new FieldTransformReader(f, field.Transforms).Read();
            return f;
        }

        public Field Read(XmlConfigurationElement x, FieldConfigurationElement parent)
        {
            var xmlField = new Field(x.Type, x.Length, FieldType.Xml, x.Output, x.Default)
            {
                Entity = _entity.Alias,
                Schema = _entity.Schema,
                Parent = parent.Name,
                XPath = parent.Xml.XPath + x.XPath,
                Name = x.XPath,
                Alias = x.Alias,
                Index = x.Index,
                Precision = x.Precision,
                Scale = x.Scale,
                Aggregate = x.Aggregate.ToLower(),
                AsParameter = new Parameter(x.Alias, null)
            };
            xmlField.Transforms = new FieldTransformReader(xmlField, x.Transforms).Read();
            return xmlField;
        }
    }
}