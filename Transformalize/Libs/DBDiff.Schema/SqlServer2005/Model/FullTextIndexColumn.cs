namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class FullTextIndexColumn
    {
        private string columnName;
        private string language;

        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

    }
}
