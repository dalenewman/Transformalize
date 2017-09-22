using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Models {
    public class FormViewModel {
        public PipelineConfigurationPart Part { get; set; }
        public Process Process { get; set; }
        public FormViewModel(PipelineConfigurationPart part, Process process) {
            Part = part;
            Process = process;
        }
    }
}