using Cfg.Net.Contracts;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Models {

    /// <summary>
    /// A process with XML serializer
    /// </summary>
    public class XmlProcess : Process {
        public XmlProcess(params IDependency[] dependencies) : base(dependencies) {}
    }

    public class XmlToJsonProcess : Process {
        public XmlToJsonProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class XmlToYamlProcess : Process {
        public XmlToYamlProcess(params IDependency[] dependencies) : base(dependencies) { }
    }
    
}