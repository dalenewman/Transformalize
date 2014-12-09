using System.Collections.Specialized;
using System.Diagnostics.Tracing;
using Transformalize.Main;

namespace Transformalize.Orchard.Models {
    public class TransformalizeRequest {
        public bool DisplayLog { get; set; }
        public EventLevel LogLevel { get; set; }
        public Options Options { get; set; }
        public string Configuration { get; set; }
        public NameValueCollection Query { get; set; }

        public TransformalizeRequest(ConfigurationPart part) {
            DisplayLog = part.DisplayLog;
            LogLevel = part.ToLogLevel();
            Configuration = part.Configuration;
        }
    }


}