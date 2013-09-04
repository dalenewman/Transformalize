using System;
using System.Collections.Generic;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class ConvertTransform : AbstractTransform
    {
        private readonly string _to;
        private readonly Dictionary<string, Func<object, object>> _conversionMap;
        private readonly Dictionary<string, Func<object, object>> _specialConversionMap = new Dictionary<string, Func<object, object>> {
            { "int32", (x => Common.DateTimeToInt32((DateTime) x)) },
        };

        public ConvertTransform(string to, IParameters parameters)
            : base(parameters)
        {
            Name = "Convert";
            _to = Common.ToSimpleType(to);

            if (HasParameters && FirstParameter.Value.SimpleType == "datetime" && _to == "int32")
                _conversionMap = _specialConversionMap;
            else
                _conversionMap = Common.ObjectConversionMap;
        }

        public override object Transform(object value)
        {
            return Common.ObjectConversionMap[_to](value);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = _conversionMap[_to](row[FirstParameter.Key]);
        }

    }
}