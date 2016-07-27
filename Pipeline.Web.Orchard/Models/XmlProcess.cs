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

    public class XmlProcessPass : Process {
        public XmlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class XmlToJsonProcessPass : Process {
        public XmlToJsonProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class XmlToYamlProcessPass : Process {
        public XmlToYamlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }
    
}