using Orchard.Events;

namespace Transformalize.Orchard.Services
{
    public interface IJobsQueueService : IEventHandler {
        void Enqueue(string message, object parameters, int priority);
    }
}