using System.Collections.Generic;
using Orchard;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {
    public interface ICfgService : IDependency {
        IEnumerable<PipelineConfigurationPart> List(string tag);
    }
}