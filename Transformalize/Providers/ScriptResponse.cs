using System.Collections.Generic;

namespace Transformalize.Providers
{
    public class ScriptResponse : IScriptReponse
    {
        public List<string> Messages { get; set; }
        public bool Success { get; set; }
        public int RowsAffected { get; set; }

        public ScriptResponse()
        {
            Messages = new List<string>();
        }
    }
}