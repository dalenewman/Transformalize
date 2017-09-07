using System.Collections.Generic;

namespace Transformalize.Contracts {
    public interface IParameterFinder {
        IEnumerable<string> Find(string query);
    }
}
