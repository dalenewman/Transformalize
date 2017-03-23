using Cfg.Net.Contracts;
using Transformalize.Configuration;

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

    public class JsonProcessPass : Process {
        public JsonProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class JsonToXmlProcessPass : Process {
        public JsonToXmlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

}