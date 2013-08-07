namespace Transformalize.Core.Transform_
{
    public class EmptyTransform : AbstractTransform
    {
        protected override string Name { get { return "Empty Transform"; }}

        public override void Transform(ref Libs.Rhino.Etl.Core.Row row) {}

        public override void Transform(ref object value) {}

        public override void Transform(ref System.Text.StringBuilder sb) {}
    }
}