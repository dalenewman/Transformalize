using System;

namespace Transformalize.Core.Transform_
{
    public class ToLocalTimeTransform : AbstractTransform
    {
        public override string Name
        {
            get { return "To Local Time Transform"; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override object Transform(object value)
        {
            return ((DateTime) value).ToLocalTime();
        }
    }
}