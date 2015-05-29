using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Transformalize.Configuration;
using Transformalize.Main;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Models {
    public class TransformalizeRequest {

        public Options Options { get; set; }
        public ConfigurationPart Part { get; set; }
        public string Configuration { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public TflRoot Root { get; set; }

        public TransformalizeRequest(
            ConfigurationPart part,
            Dictionary<string, string> query,
            string modifiedConfiguration,
            ILogger logger,
            TflRoot root = null) {
                Part = part;
                Configuration = modifiedConfiguration ?? part.Configuration;
                Query = query ?? new Dictionary<string, string>();
                Options = Query.ContainsKey("mode") ? new Options { Mode = Query["mode"] } : new Options();
                Root = root ?? new TflRoot(modifiedConfiguration ?? part.Configuration, Query, new CfgNetLogger(logger));
        }
    }


}