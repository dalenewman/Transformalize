using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class EmptyTransform : AbstractTransform
    {
        public EmptyTransform()
        {
            Name = "Empty";
        }

        public override void Transform(ref Row row, string resultKey) { }

        public override object Transform(object value)
        {
            return null;
        }

        public override void Transform(ref System.Text.StringBuilder sb) { }
    }
}