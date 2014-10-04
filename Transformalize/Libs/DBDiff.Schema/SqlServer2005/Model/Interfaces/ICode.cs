using System.Collections.Generic;
using Transformalize.Libs.DBDiff.Schema.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces
{
    public interface ICode:ISchemaBase
    {
        SQLScriptList Rebuild();
        List<string> DependenciesIn { get; set; }
        List<string> DependenciesOut { get; set; }
        bool IsSchemaBinding { get; set; }
        string Text { get; set; }
    }
}
