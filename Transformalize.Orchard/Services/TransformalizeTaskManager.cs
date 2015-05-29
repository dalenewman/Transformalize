using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Tasks.Scheduling;

namespace Transformalize.Orchard.Services {
    public class TransformalizeTaskManager : ITransformalizeTaskManager {
        private readonly IScheduledTaskManager _manager;

        public TransformalizeTaskManager(IScheduledTaskManager manager) {
            _manager = manager;
        }

        public void CreateTask(ContentItem item) {
            var name = TransformalizeTaskHandler.GetName(item.As<TitlePart>().Title);

            var tasks = _manager.GetTasks(name);
            if (tasks != null && tasks.Any())
                return;

            _manager.CreateTask(name, DateTime.UtcNow.AddSeconds(30), item);
        }
    }
}