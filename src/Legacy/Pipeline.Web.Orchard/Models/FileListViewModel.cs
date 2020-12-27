using System.Collections.Generic;

namespace Pipeline.Web.Orchard.Models {

    public class FileListViewModel {

        public IEnumerable<PipelineFilePart> Files { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public FileListViewModel(IEnumerable<PipelineFilePart> files, IEnumerable<string> roles, IEnumerable<string> tags) {
            Files = files;
            Roles = roles;
            Tags = tags;
        }
    }
}