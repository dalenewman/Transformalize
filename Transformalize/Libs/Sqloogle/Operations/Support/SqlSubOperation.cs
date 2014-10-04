using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;
using Guard = Transformalize.Libs.Rhino.Etl.Guard;

namespace Transformalize.Libs.Sqloogle.Operations.Support {

    public class SqlSubOperation : AbstractOperation {

        private const string COLUMN_WITH_CONNECTION_STRING = "connectionstring";
        private readonly AbstractConnection _connection;
        private readonly string _columnWithSql;

        public SqlSubOperation(AbstractConnection connection, string columnWithSql)
        {
            _connection = connection;
            _columnWithSql = columnWithSql;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                foreach (var subRow in GetSubOperation(_connection, row).Execute(null)) {
                    foreach (var column in row.Columns)
                        if (column != _columnWithSql)
                            subRow.Add(column, row[column]);

                    yield return subRow;
                }
            }

        }

        protected IOperation GetSubOperation(AbstractConnection connection, Row row)
        {
            Guard.Against(!row.Contains(COLUMN_WITH_CONNECTION_STRING), string.Format("Rows must contain \"{0}\" key.", COLUMN_WITH_CONNECTION_STRING));
            Guard.Against(!row.Contains(_columnWithSql), string.Format("Rows must contain \"{0}\" key.", _columnWithSql));

            connection.Database = ConnectionStringParser.GetDatabaseName(row[COLUMN_WITH_CONNECTION_STRING].ToString());
            var sql = row[_columnWithSql].ToString();
            return new SqlOperation(connection, sql);            
        }
    }
}
