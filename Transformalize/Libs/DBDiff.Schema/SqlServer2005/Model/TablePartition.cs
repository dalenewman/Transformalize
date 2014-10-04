using System;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class TablePartition:SQLServerSchemaBase
    {
        private string compressType;

        public TablePartition(Table parent)
            : base(parent, Enums.ObjectType.Partition)
        {
        }

        public string CompressType
        {
            get { return compressType; }
            set { compressType = value; }
        }


        public override string ToSql()
        {
            throw new NotImplementedException();
        }

        public override string ToSqlDrop()
        {
            throw new NotImplementedException();
        }

        public override string ToSqlAdd()
        {
            throw new NotImplementedException();
        }
    }
}
