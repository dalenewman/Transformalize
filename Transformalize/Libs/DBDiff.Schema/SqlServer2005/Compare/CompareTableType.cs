using Transformalize.Libs.DBDiff.Schema.Model;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Compare
{
    internal class CompareTableType : CompareBase<TableType>
    {
        protected override void DoUpdate<Root>(SchemaList<TableType, Root> CamposOrigen, TableType node)
        {
            if (node.Status != Enums.ObjectStatusType.DropStatus)
            {
                TableType tablaOriginal = CamposOrigen[node.FullName];
                (new CompareColumns()).GenerateDiferences<TableType>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDiferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDiferences<TableType>(tablaOriginal.Indexes, node.Indexes);
            }
        }

        /*public static void GenerateDiferences(SchemaList<TableType, Database> tablasOrigen, SchemaList<TableType, Database> tablasDestino)
        {
            MarkDrop(tablasOrigen, tablasDestino);

            foreach (TableType node in tablasDestino)
            {
                if (!tablasOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    node.Parent = tablasOrigen.Parent;
                    tablasOrigen.Add(node);
                }
                else
                {
                    if (node.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        TableType tablaOriginal = tablasOrigen[node.FullName];
                        CompareColumns.GenerateDiferences<TableType>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDiferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDiferences(tablaOriginal.Indexes, node.Indexes);
                    }
                }
            }
        }*/
    }
}
