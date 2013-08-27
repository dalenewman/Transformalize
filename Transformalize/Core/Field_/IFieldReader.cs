using Transformalize.Configuration;

namespace Transformalize.Core.Field_
{
    public interface IFieldReader
    {
        Field Read(FieldConfigurationElement element, FieldType fieldType = FieldType.Field);
        Field Read(XmlConfigurationElement element, FieldConfigurationElement parent);
    }
}