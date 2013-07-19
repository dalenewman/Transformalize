using System.Collections.Generic;
using Transformalize.Model;

namespace Transformalize.Data {

    public interface IEntityAutoFieldReader {
        Dictionary<string, Field> ReadFields();
        Dictionary<string, Field> ReadPrimaryKey();
        Dictionary<string, Field> ReadAll();
    }
}
