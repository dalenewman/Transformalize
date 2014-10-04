using Transformalize.Libs.DBDiff.Schema.Model;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareStoreProcedures:CompareBase<StoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<StoreProcedure, Root> CamposOrigen, StoreProcedure node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                StoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
