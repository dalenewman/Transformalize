using Orchard;
using Pipeline.Configuration;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {

    public interface IProcessService : IDependency {
        Process Resolve(string input, string output, bool pass = false);
    }

    /// <summary>
    /// Orchard's WorkContext does not allow resolve with parameters, so I had to create a class for each combination and resolve as such
    /// </summary>
    public class ProcessService : IProcessService {

        private readonly IOrchardServices _orchard;

        public ProcessService(IOrchardServices orchard) {
            _orchard = orchard;
        }

        public Process Resolve(string input, string output, bool pass = false) {
            switch (input) {
                case "json":
                    switch (output) {
                        case "json":
                            return pass ? _orchard.WorkContext.Resolve<JsonProcessPass>() : _orchard.WorkContext.Resolve<JsonProcess>() as Process;
                        case "yaml":
                            return pass ? _orchard.WorkContext.Resolve<JsonToYamlProcessPass>() : _orchard.WorkContext.Resolve<JsonToYamlProcess>() as Process;
                        default:
                            return pass ? _orchard.WorkContext.Resolve<JsonToXmlProcessPass>() :  _orchard.WorkContext.Resolve<JsonToXmlProcess>() as Process;
                    }
                case "yaml":
                    switch (output) {
                        case "json":
                            return pass ? _orchard.WorkContext.Resolve<YamlToJsonProcessPass>() : _orchard.WorkContext.Resolve<YamlToJsonProcess>() as Process;
                        case "yaml":
                            return pass ? _orchard.WorkContext.Resolve<YamlProcessPass>() : _orchard.WorkContext.Resolve<YamlProcess>() as Process;
                        default:
                            return pass ? _orchard.WorkContext.Resolve<YamlToXmlProcessPass>() : _orchard.WorkContext.Resolve<YamlToXmlProcess>() as Process;
                    }
                default:
                    switch (output) {
                        case "json":
                            return pass ? _orchard.WorkContext.Resolve<XmlToJsonProcessPass>() : _orchard.WorkContext.Resolve<XmlToJsonProcess>() as Process;
                        case "yaml":
                            return pass ? _orchard.WorkContext.Resolve<XmlToYamlProcessPass>() : _orchard.WorkContext.Resolve<XmlToYamlProcess>() as Process;
                        default:
                            return pass ? _orchard.WorkContext.Resolve<XmlProcessPass>() : _orchard.WorkContext.Resolve<XmlProcess>() as Process;
                    }
            }
        }
    }
}