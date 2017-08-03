using System.Collections.Generic;
using Autofac;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Ioc.Autofac;
using Transformalize.Logging;

namespace Tests {
    public class TestBase {

        public static IDictionary<string, string> InitMode() {
            return new Dictionary<string, string> { { "Mode", "init" } };
        }

        public static Process ResolveRoot(IContainer container, string cfg, IDictionary<string, string> parameters = null) {
            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }
            return container.Resolve<Process>(new NamedParameter("cfg", cfg), new NamedParameter("parameters", parameters));
        }

        public static ActionResponse Execute(Process process, string placeHolderStyle = "@()") {
            var container = DefaultContainer.Create(process, new DebugLogger(), placeHolderStyle);
            return new PipelineAction(container).Execute();
        }
    }
}