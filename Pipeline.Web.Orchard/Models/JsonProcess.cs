using Cfg.Net.Contracts;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Models {
    /// <summary>
    /// A process with a JSON serializer
    /// </summary>
    public class JsonProcess : Process {
        public JsonProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class JsonToXmlProcess : Process {
        public JsonToXmlProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class JsonToYamlProcess : Process {
        public JsonToYamlProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

}