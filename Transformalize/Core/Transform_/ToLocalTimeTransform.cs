using System;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class ToLocalTimeTransform : AbstractTransform
    {
        public ToLocalTimeTransform(IParameters parameters) : base(parameters)
        {
            Name = "To Local Time";
        }

        public override object Transform(object value)
        {
            return ((DateTime)value).ToLocalTime();
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = ((DateTime) row[FirstParameter.Key]).ToLocalTime();
        }
    }
}