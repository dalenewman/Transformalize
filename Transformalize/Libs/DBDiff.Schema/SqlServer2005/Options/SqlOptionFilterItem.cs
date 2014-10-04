namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Options
{
    public class SqlOptionFilterItem
    {
        private Enums.ObjectType type;
        private string filter;

        public SqlOptionFilterItem(Enums.ObjectType type, string value)
        {
            this.filter = value;
            this.type = type;
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Filter
        {
            get { return filter; }
            set { this.filter = value; }
        }
        
    }
}
