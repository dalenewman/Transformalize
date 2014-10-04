using System.Data;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Operations {

    public class CachedObjectStatsExtract : InputCommandOperation {

        public CachedObjectStatsExtract(AbstractConnection connection)
            : base(connection) {
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */

                SELECT  
                    DB_NAME(ph.dbid) AS [database]
	                ,ph.objectid AS objectid
                    ,MAX(cp.usecounts) AS [use]
                FROM master.sys.dm_exec_cached_plans cp WITH (NOLOCK)
                CROSS APPLY master.sys.dm_exec_sql_text(plan_handle) ph
                WHERE cp.cacheobjtype != 'Extended Proc'
                AND ph.objectid IS NOT NULL
                AND DB_NAME(ph.dbid) IS NOT NULL
                GROUP BY 
                    DB_NAME(ph.dbid),
                    ph.objectid
                ORDER BY [Database], ObjectId
            ";
        }
    }
}
