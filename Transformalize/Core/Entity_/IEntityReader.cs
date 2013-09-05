using Transformalize.Configuration;

namespace Transformalize.Core.Entity_
{
    public interface IEntityReader
    {
        Entity Read(EntityConfigurationElement element, int count);
    }
}