using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using System.Collections.Generic;

namespace Transformalize.Ioc.Autofac.Impl {
    public class NullPlaceHolderReplacer : IPlaceHolderReplacer {
        public string Replace(string value, IDictionary<string, string> parameters, ILogger logger) {
            return value;
        }
    }
}
