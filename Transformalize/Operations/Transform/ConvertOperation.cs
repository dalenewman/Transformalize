using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ConvertOperation : ShouldRunOperation {
        private readonly string _outType;
        private readonly string _fromFormat;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

        public ConvertOperation(string inKey, string inType, string outKey, string outType, string fromFormat = "")
            : base(inKey, outKey) {
            _outType = outType;
            _fromFormat = fromFormat;

            if (_outType == "datetime" && !string.IsNullOrEmpty(_fromFormat)) {
                _conversionMap[_outType] = (x => DateTime.ParseExact(x.ToString(), _fromFormat, System.Globalization.CultureInfo.InvariantCulture));
            }
            if (_outType == "int32" && inType == "datetime") {
                _conversionMap[_outType] = (x => Common.DateTimeToInt32((DateTime)x));
            }
            Name = string.Format("ConvertOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _conversionMap[_outType](row[InKey]);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
                yield return row;
            }
        }
    }
}