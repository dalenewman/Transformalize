using System;
using System.Data;

namespace Transformalize.Providers.MySql
{
    public class MySqlScriptRunner : IScriptRunner
    {
        private readonly MySqlConnection _connection;

        public MySqlScriptRunner(MySqlConnection connection)
        {
            _connection = connection;
        }

        public IScriptReponse Execute(string script)
        {
            var response = new ScriptResponse();

            var type = Type.GetType(_connection.Provider, false, true);
            using (var cn = (IDbConnection) Activator.CreateInstance(type))
            {
                cn.ConnectionString = _connection.ConnectionString;
                try
                {
                    cn.Open();
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = script;
                    cmd.CommandType = CommandType.Text;
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