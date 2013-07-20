using System.Collections.Generic;
using Transformalize.Model;

namespace Transformalize.Data
{
    public interface IDataTypeService {
        string GetDataType(Field field);
        Dictionary<string, string> Types { get; }
        Dictionary<string, string> TypesReverse { get; } 

    }
}