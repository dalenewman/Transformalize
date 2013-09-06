using System;
using System.Data;
using Transformalize.Providers.MySql;

namespace Transformalize.Providers
{
    public class DefaultScriptRunner : IScriptRunner
    {
        private readonly IConnection _connection;

        public DefaultScriptRunner(IConnection connection)
        {
            _connection = connection;
        }

        public IScriptReponse Execute(string script)
        {
            var response = new ScriptResponse();
            
            using (var cn = _connection.GetConnection())
            {
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