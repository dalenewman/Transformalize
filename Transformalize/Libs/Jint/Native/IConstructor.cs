using Transformalize.Libs.Jint.Native.Object;

namespace Transformalize.Libs.Jint.Native
{
    public interface IConstructor
    {
        JsValue Call(JsValue thisObject, JsValue[] arguments);
        ObjectInstance Construct(JsValue[] arguments);
    }
}
