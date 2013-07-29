using System.Collections.Generic;

namespace Transformalize.Data
{
    public interface IMapReader {
        Dictionary<string, object> Read();
    }
}