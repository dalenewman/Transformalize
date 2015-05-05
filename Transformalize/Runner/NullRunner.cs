using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class NullRunner : IProcessRunner {
        public IEnumerable<Row> Run(Process process) {
            return Enumerable.Empty<Row>();
        }
    }
}