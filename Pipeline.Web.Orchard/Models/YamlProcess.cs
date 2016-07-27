using Cfg.Net.Contracts;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Models {
    /// <summary>
    /// A process with YAML serializer
    /// </summary>
    public class YamlProcess : Process {
        public YamlProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class YamlToXmlProcess : Process {
        public YamlToXmlProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class YamlToJsonProcess : Process {
        public YamlToJsonProcess(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class YamlProcessPass : Process {
        public YamlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class YamlToXmlProcessPass : Process {
        public YamlToXmlProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

    public class YamlToJsonProcessPass : Process {
        public YamlToJsonProcessPass(params IDependency[] dependencies) : base(dependencies) { }
    }

}