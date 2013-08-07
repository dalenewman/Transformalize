using System.Collections.Generic;
using Transformalize.Core.Field_;

namespace Transformalize.Providers
{
    public interface IDataTypeService {
        string GetDataType(Field field);
        Dictionary<string, string> Types { get; }
        Dictionary<string, string> TypesReverse { get; } 

    }
}