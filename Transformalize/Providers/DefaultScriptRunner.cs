using System;
using System.Data;

namespace Transformalize.Providers
{
    public class DefaultScriptRunner : IScriptRunner
    {

        public IScriptReponse Execute(AbstractConnection connection, string script)
        {
            var response = new ScriptResponse();
            
            using (var cn = connection.GetConnection())
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