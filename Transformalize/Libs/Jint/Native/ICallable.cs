namespace Transformalize.Libs.Jint.Native
{
    public interface ICallable
    {
        JsValue Call(JsValue thisObject, JsValue[] arguments);
    }
}