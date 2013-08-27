using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class EmptyTransform : AbstractTransform
    {
        public override string Name { get { return "Empty Transform"; }}

        public override void Transform(ref Row row, string resultKey) {}

        public override object Transform(object value)
        {
            return null;
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override void Transform(ref System.Text.StringBuilder sb) {}
    }
}