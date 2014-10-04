using Transformalize.Libs.DBDiff.Schema.Model;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareCLRFunction : CompareBase<CLRFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRFunction, Root> CamposOrigen, CLRFunction node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                CLRFunction newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
