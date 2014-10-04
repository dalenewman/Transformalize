using System;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Options
{
    public class SqlOptionScript
    {
        private Boolean alterObjectOnSchemaBinding = true;

        public Boolean AlterObjectOnSchemaBinding
        {
            get { return alterObjectOnSchemaBinding; }
            set { alterObjectOnSchemaBinding = value; }
        }
    }
}
