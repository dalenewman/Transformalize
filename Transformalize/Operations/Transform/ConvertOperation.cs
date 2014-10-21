using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ConvertOperation : ShouldRunOperation {
        private readonly string _outType;
        private readonly string _fromFormat;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

        public ConvertOperation(string inKey, string inType, string outKey, string outType, string encoding, string fromFormat = "")
            : base(inKey, outKey) {
            _outType = Common.ToSimpleType(outType);
            _fromFormat = fromFormat;

            if (!_conversionMap.ContainsKey(_outType)) {
                throw new TransformalizeException("Type {0} is not mapped for conversion.", _outType);
            }

            if (_outType == "datetime" && !string.IsNullOrEmpty(_fromFormat)) {
                _conversionMap[_outType] = (x => DateTime.ParseExact(x.ToString(), _fromFormat, System.Globalization.CultureInfo.InvariantCulture));
            }
            if (_outType == "int32" && inType == "datetime") {
                _conversionMap[_outType] = (x => Common.DateTimeToInt32((DateTime)x));
            }
            if (_outType == "long" && inType == "datetime") {
                _conversionMap[_outType] = (x => ((DateTime)x).Ticks);
            }
            if (_outType.Equals("string") && inType == "byte[]") {
                if (!encoding.Equals(Common.DefaultValue)) {
                    if (System.Text.Encoding.GetEncodings().Any(e => e.Name.Equals(encoding))) {
                        throw new TransformalizeException("The encoding `{0}` declared in your convert transform does not exist in System.Text.Encoding (see www.iana.org for standard encoding names).");
                    }
                    _conversionMap[_outType] = (x => System.Text.Encoding.GetEncoding(encoding).GetString((byte[])x));
                }
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