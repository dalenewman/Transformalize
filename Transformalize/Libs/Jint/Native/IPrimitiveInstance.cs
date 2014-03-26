using Transformalize.Libs.Jint.Runtime;

namespace Transformalize.Libs.Jint.Native
{
    public interface IPrimitiveInstance
    {
        Types Type { get; } 
        JsValue PrimitiveValue { get; }
    }
}
