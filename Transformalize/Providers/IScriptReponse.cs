using System.Collections.Generic;

namespace Transformalize.Providers
{
    public interface IScriptReponse
    {
        List<string> Messages { get; set; }
        bool Success { get; set; }
        int RowsAffected { get; set; }
    }
}