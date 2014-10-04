using System;
using System.Data;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Operations {

    public class SqlAgentJobExtract : InputCommandOperation {

        private const string SCHEMA = "sys";
        private const string SQL_TYPE = "Sql Agent Job";
        private const string PATH = @"/SqlAgentJobs/";
        private const string DATABASE = "msdb";

        public SqlAgentJobExtract(AbstractConnection connection) : base(connection) { }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = Row.FromReader(reader);
            row["name"] = String.Format("{0} - Step {1:00} - {2}", row["jobname"], row["stepid"], row["stepname"]);
            row["schema"] = SCHEMA;
            row["type"] = SQL_TYPE;
            row["path"] = PATH;
            row["database"] = DATABASE;
            row["created"] = row["created"];
            row["modified"] = row["modified"];
            row["lastused"] = row["lastused"] == null ? null : StringToDate(row["lastused"].ToString());
            return row;
        }

        private object StringToDate(string date) {
            var year = Convert.ToInt32(date.Left(4));
            var month = Convert.ToInt32(date.Substring(4, 2));
            var day = Convert.ToInt32(date.Right(2));
            return new DateTime(year, month, day);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */

                USE msdb;

                SELECT
                    sj.name AS jobname,
                    sjs.step_id AS stepid,
                    sjs.step_name AS stepname,
                    sjs.command AS sql,
                    sj.date_created AS created,
                    sj.date_modified AS modified,
                    sjh.lastused
                FROM msdb.dbo.sysjobsteps sjs WITH (NOLOCK)
                INNER JOIN msdb.dbo.sysjobs sj WITH (NOLOCK) ON (sjs.job_id = sj.job_id)
                LEFT OUTER JOIN (
	                select job_id, step_id, MAX(run_date) AS lastused
	                from msdb.dbo.sysjobhistory sjh WITH (NOLOCK)
	                group by job_id, step_id
                ) sjh  ON (sjs.job_id = sjh.job_id AND sjs.step_id = sjh.step_id) 
                WHERE subsystem = 'TSQL'
                ORDER BY sj.name, sjs.step_id;
            ";
        }
    }
}
