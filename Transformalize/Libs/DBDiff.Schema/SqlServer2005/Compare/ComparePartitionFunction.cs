using Transformalize.Libs.DBDiff.Schema.Model;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class ComparePartitionFunction:CompareBase<PartitionFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionFunction, Root> CamposOrigen, PartitionFunction node)
        {
            if (!PartitionFunction.Compare(node, CamposOrigen[node.FullName]))
            {
                PartitionFunction newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                CamposOrigen[node.FullName] = newNode;
            }
            else
            {
                if (!PartitionFunction.CompareValues(node, CamposOrigen[node.FullName]))
                {
                    PartitionFunction newNode = node.Clone(CamposOrigen.Parent);
                    if (newNode.Values.Count == CamposOrigen[node.FullName].Values.Count)
                        newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                    else
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                    newNode.Old = CamposOrigen[node.FullName].Clone(CamposOrigen.Parent);
                    CamposOrigen[node.FullName] = newNode;
                }
            }            
        }
    }
}
