using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Models {
    public class ReportViewModel {
        public Process Process { get; set; }
        public PipelineConfigurationPart Part { get; set; }

        public ReportViewModel(Process process, PipelineConfigurationPart part) {
            Process = process;
            Part = part;
        }
    }
}