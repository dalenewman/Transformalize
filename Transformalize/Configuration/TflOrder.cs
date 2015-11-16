using Cfg.Net;

namespace Transformalize.Configuration
{
    public class TflOrder : CfgNode {
        [Cfg(required=true, unique = true)]
        public string Field { get; set; }

        [Cfg(value = "asc", domain = "asc,desc", toLower = true, ignoreCase = true)]
        public string Sort { get; set; }
    }
}