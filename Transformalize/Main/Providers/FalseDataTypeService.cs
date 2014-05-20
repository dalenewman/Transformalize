using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class FalseDataTypeService : IDataTypeService {

        public Dictionary<string, string> Types { get; private set; }
        public Dictionary<string, string> TypesReverse { get; private set; }
        public string GetDataType(Field field)
        {
            throw new System.NotImplementedException();
        }
    }
}