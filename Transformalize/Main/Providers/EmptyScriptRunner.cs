using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class EmptyScriptRunner : IScriptRunner {
        public IScriptReponse Execute(AbstractConnection connection, string script) {
            return new ScriptResponse() { Messages = new List<string>(), RowsAffected = 0, Success = true };
        }
    }
}