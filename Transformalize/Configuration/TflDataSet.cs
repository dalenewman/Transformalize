using System.Collections.Generic;
using Cfg.Net;

namespace Transformalize.Configuration
{
    public class TflDataSet : CfgNode {
        [Cfg(required = true)]
        public string Name { get; set; }

        [Cfg()]
        public List<Dictionary<string, string>> Rows { get; set; }
    }
}