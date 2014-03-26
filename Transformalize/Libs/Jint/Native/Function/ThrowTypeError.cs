using Transformalize.Libs.Jint.Runtime;
using Transformalize.Libs.Jint.Runtime.Descriptors;

namespace Transformalize.Libs.Jint.Native.Function
{
    public sealed class ThrowTypeError : FunctionInstance
    {
        private readonly Engine _engine;

        public ThrowTypeError(Engine engine): base(engine, new string[0], engine.GlobalEnvironment, false)
        {
            _engine = engine;
            DefineOwnProperty("length", new PropertyDescriptor(0, false, false, false), false);
            Extensible = false;
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            throw new JavaScriptException(_engine.TypeError);
        }
    }
}
