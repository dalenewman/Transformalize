using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class NullScriptRunner : IScriptRunner {
        public IScriptReponse Execute(AbstractConnection connection, string script, int timeOut) {
            return new ScriptResponse() { Messages = new List<string>(), RowsAffected = 0, Success = true };
        }
    }
}