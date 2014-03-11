using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public interface IEntityCreator {
        IEntityExists EntityExists { get; set; }
        void Create(AbstractConnection connection, Process process, Entity entity);
    }
}