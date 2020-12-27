using System.Collections.Generic;
using Orchard;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface ICfgService : IDependency {
        IEnumerable<PipelineConfigurationPart> List(string tag);
    }
}