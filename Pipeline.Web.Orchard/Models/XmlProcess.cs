using Cfg.Net.Contracts;
using Transformalize.Configuration;

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

    public class XmlProcessPass : Process {
        public XmlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class XmlToJsonProcessPass : Process {
        public XmlToJsonProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }
   
}