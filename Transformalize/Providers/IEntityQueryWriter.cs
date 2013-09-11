using Transformalize.Core.Entity_;

namespace Transformalize.Providers
{
    public interface IEntityQueryWriter
    {
        string Write(Entity entity);
    }
}