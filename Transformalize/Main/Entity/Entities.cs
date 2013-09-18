using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main {
    
    public class Entities : List<Entity>
    {
        public IEnumerable<string> OutputKeys()
        {
            return this.SelectMany(e2 => e2.Fields.OutputKeys());
        }
    }
}
