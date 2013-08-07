using Transformalize.Configuration;

namespace Transformalize.Core.Field_
{
    public interface IFieldReader
    {
        Field Read(FieldConfigurationElement field, FieldType fieldType = FieldType.Field);
    }
}