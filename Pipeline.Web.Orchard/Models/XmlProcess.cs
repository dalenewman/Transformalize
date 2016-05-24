using Cfg.Net.Contracts;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Models {

    /// <summary>
    /// A process with XML serializer
    /// </summary>
    public class XmlProcess : Process {
        public XmlProcess(params IDependency[] dependencies)
            : base(dependencies) {
        }
    }
}