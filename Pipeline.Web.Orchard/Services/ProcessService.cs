using Orchard;
using Pipeline.Configuration;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {

    public interface IProcessService : IDependency {
        Process Resolve(string input, string output);
    }

    /// <summary>
    /// Orchard's WorkContext does not allow resolve with parameters, so I had to create a class for each combination and resolve as such
    /// </summary>
    public class ProcessService : IProcessService {

        private readonly IOrchardServices _orchard;

        public ProcessService(IOrchardServices orchard) {
            _orchard = orchard;
        }

        public Process Resolve(string input, string output) {
            switch (input) {
                case "json":
                    switch (output) {
                        case "json":
                            return _orchard.WorkContext.Resolve<JsonProcess>();
                        case "yaml":
                            return _orchard.WorkContext.Resolve<JsonToYamlProcess>();
                        default:
                            return _orchard.WorkContext.Resolve<JsonToXmlProcess>();
                    }
                case "yaml":
                    switch (output) {
                        case "json":
                            return _orchard.WorkContext.Resolve<YamlToJsonProcess>();
                        case "yaml":
                            return _orchard.WorkContext.Resolve<YamlProcess>();
                        default:
                            return _orchard.WorkContext.Resolve<YamlToXmlProcess>();
                    }
                default:
                    switch (output) {
                        case "json":
                            return _orchard.WorkContext.Resolve<XmlToJsonProcess>();
                        case "yaml":
                            return _orchard.WorkContext.Resolve<XmlToYamlProcess>();
                        default:
                            return _orchard.WorkContext.Resolve<XmlProcess>();
                    }
            }
        }
    }
}