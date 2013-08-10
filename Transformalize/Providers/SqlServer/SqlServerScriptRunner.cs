using System;
using System.Data.SqlClient;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerScriptRunner : IScriptRunner
    {
        private readonly SqlServerConnection _connection;

        public SqlServerScriptRunner(SqlServerConnection connection)
        {
            _connection = connection;
        }

        public IScriptReponse Execute(string script)
        {
            var response = new ScriptResponse();
            using (var cn = new SqlConnection(_connection.ConnectionString))
            {
                try
                {
                    cn.Open();
                    var cmd = new SqlCommand(script, cn);
                    response.RowsAffected = cmd.ExecuteNonQuery();
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.Messages.Add(e.Message);
                    if (e.InnerException != null)
                    {
                        response.Messages.Add(e.InnerException.Message);
                    }
                }
            }
            return response;
        }
    }
}