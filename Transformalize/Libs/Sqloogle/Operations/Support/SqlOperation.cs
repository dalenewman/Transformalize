using System.Data;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Operations.Support
{

    /// <summary>
    /// Generic input command operation that takes the
    /// connection string and sql you want to run.  Use
    /// this when your program is creating the connection
    /// string and sql (you don't know it up front).
    /// </summary>
    public class SqlOperation : InputCommandOperation
    {
        private readonly string _sql;

        public SqlOperation(AbstractConnection connection, string sql) : base(connection)
        {
            _sql = sql;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
        }
    }
}