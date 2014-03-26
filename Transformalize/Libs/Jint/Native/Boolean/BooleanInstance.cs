using Transformalize.Libs.Jint.Native.Object;
using Transformalize.Libs.Jint.Runtime;

namespace Transformalize.Libs.Jint.Native.Boolean
{
    public class BooleanInstance : ObjectInstance, IPrimitiveInstance
    {
        public BooleanInstance(Engine engine)
            : base(engine)
        {
        }

        public override string Class
        {
            get
            {
                return "Boolean";
            }
        }

        Types IPrimitiveInstance.Type
        {
            get { return Types.Boolean; }
        }

        JsValue IPrimitiveInstance.PrimitiveValue
        {
            get { return PrimitiveValue; }
        }

        public JsValue PrimitiveValue { get; set; }
    }
}
