using System;
using Transformalize.Libs.Jint.Native;
using Transformalize.Libs.Jint.Native.Function;

namespace Transformalize.Libs.Jint.Runtime.Interop
{
    /// <summary>
    /// Represents a FunctionInstance wrapping a Clr getter.
    /// </summary>
    public sealed class GetterFunctionInstance: FunctionInstance
    {
        private readonly Func<JsValue, JsValue> _getter;

        public GetterFunctionInstance(Engine engine, Func<JsValue, JsValue> getter)
            : base(engine,  null, null, false)
        {
            _getter = getter;
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            return _getter(thisObject);
        }
    }
}
