using System;

namespace Transformalize.Core.Transform_
{
    public class ToLocalTimeTransform : AbstractTransform
    {
        protected override string Name
        {
            get { return "To Local Time Transform"; }
        }

        public override void Transform(ref object value)
        {
            value = ((DateTime) value).ToLocalTime();
        }
    }
}