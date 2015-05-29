using Orchard.Events;

namespace Transformalize.Orchard.Services {
    public interface ITransformalizeJobService : IEventHandler {
        void Run(string args);
    }
}