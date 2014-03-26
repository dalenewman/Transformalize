using Transformalize.Libs.Jint.Native;

namespace Transformalize.Libs.Jint.Runtime.Environments
{
    public sealed class ExecutionContext
    {
        public LexicalEnvironment LexicalEnvironment { get; set; }
        public LexicalEnvironment VariableEnvironment { get; set; }
        public JsValue ThisBinding { get; set; }

    }
}
