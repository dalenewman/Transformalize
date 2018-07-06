using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Models {
    public class BuilderViewModel {
        public Process Process { get; set; }
        public BuilderViewModel(Process process) {
            Process = process;
        }
    }
}