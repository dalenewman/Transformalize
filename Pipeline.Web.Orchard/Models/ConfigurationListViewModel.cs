using System.Collections.Generic;

namespace Pipeline.Web.Orchard.Models {
    public class ConfigurationListViewModel {

        public IEnumerable<PipelineConfigurationPart> Configurations { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public ConfigurationListViewModel(IEnumerable<PipelineConfigurationPart> configurations, IEnumerable<string> tags) {
            Configurations = configurations;
            Tags = tags;
        }
    }
}