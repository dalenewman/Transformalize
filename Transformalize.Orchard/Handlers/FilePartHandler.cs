using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public class FilePartHandler : ContentHandler {
        public FilePartHandler(IRepository<FilePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
