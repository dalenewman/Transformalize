using Transformalize.Libs.Jint.Runtime.Environments;

namespace Transformalize.Libs.Jint.Native.Function
{
    public sealed class FunctionShim : FunctionInstance
    {
        public FunctionShim(Engine engine, string[] parameters, LexicalEnvironment scope) : base(engine, parameters, scope, false)
        {
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            return Undefined.Instance;
        }
    }
}
