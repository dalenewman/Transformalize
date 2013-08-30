using System.Collections;
using Transformalize.Configuration;
using Transformalize.Core.Entity_;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Field_
{
    public class FieldReader : IFieldReader
    {
        private readonly ITransformParametersReader _transformParametersReader;
        private readonly IParametersReader _parametersReader;
        private readonly Entity _entity;

        public FieldReader(Entity entity, ITransformParametersReader transformParametersReader, IParametersReader parametersReader)
        {
            _transformParametersReader = transformParametersReader;
            _parametersReader = parametersReader;
            _entity = entity ?? new Entity();
        }

        public Field Read(FieldConfigurationElement element, FieldType fieldType = FieldType.Field)
        {
            var field = new Field(element.Type, element.Length, fieldType, element.Output, element.Default) {
                Process = Process.Name,
                Entity = _entity.Alias,
                Schema = _entity.Schema,
                Name = element.Name,
                Alias = element.Alias,
                Precision = element.Precision,
                Scale = element.Scale,
                Input = element.Input,
                Unicode = element.Unicode,
                VariableLength = element.VariableLength,
                Aggregate = element.Aggregate.ToLower(),
                AsParameter = new Parameter(element.Alias, null) { SimpleType = Common.ToSimpleType(element.Type) }
            };

            FieldTransformLoader(field, element.Transforms);

            return field;
        }

        public Field Read(XmlConfigurationElement element, FieldConfigurationElement parent)
        {
            var xmlField = new Field(element.Type, element.Length, FieldType.Xml, element.Output, element.Default)
            {
                Entity = _entity.Alias,
                Schema = _entity.Schema,
                Parent = parent.Name,
                XPath = parent.Xml.XPath + element.XPath,
                Name = element.XPath,
                Alias = element.Alias,
                Index = element.Index,
                Precision = element.Precision,
                Scale = element.Scale,
                Aggregate = element.Aggregate.ToLower(),
                AsParameter = new Parameter(element.Alias, null) { SimpleType = Common.ToSimpleType(element.Type) }
            };

            FieldTransformLoader(xmlField, element.Transforms);

            return xmlField;
        }

        private void FieldTransformLoader(Field field, IEnumerable transformElements)
        {
            foreach (TransformConfigurationElement t in transformElements)
            {
                field.Transforms.Add(new TransformFactory(t, _transformParametersReader, _parametersReader).Create(field.Alias));
            }
        }

    }
}