using System.Collections.Generic;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;

namespace Pipeline.Web.Orchard.Impl {
    public class NullPlaceHolderReplacer : IPlaceHolderReplacer {
        public string Replace(string value, IDictionary<string, string> parameters, ILogger logger) {
            return value;
        }
    }
}
