using System.Collections.Generic;

namespace Pipeline.Web.Orchard.Models {
    public class ConfigurationListViewModel {

        public IEnumerable<PipelineConfigurationPart> Configurations { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string TagFilter { get; set; }

        public ConfigurationListViewModel(IEnumerable<PipelineConfigurationPart> configurations, IEnumerable<string> tags, string tagFilter) {
            TagFilter = tagFilter;
            Configurations = configurations;
            Tags = tags;
        }
    }
}