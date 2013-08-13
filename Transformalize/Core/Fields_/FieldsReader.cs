using Transformalize.Configuration;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;

namespace Transformalize.Core.Fields_
{
    public class FieldsReader : IFieldsReader
    {
        private readonly Entity _entity;
        private readonly FieldElementCollection _fields;
        private readonly FieldType _fieldType;

        public FieldsReader(Entity entity, FieldElementCollection fields, FieldType fieldType = FieldType.Field)
        {
            _entity = entity;
            _fields = fields;
            _fieldType = fieldType;
        }

        public IFields Read()
        {
            var fields = new Fields();
            foreach (FieldConfigurationElement f in _fields)
            {
                var field = new FieldReader(_entity).Read(f, _fieldType);
                fields.Add(field.Alias, field);
            }
            return fields;
        }
    }
}