using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ConvertOperation : TflOperation {

        private readonly string _inType;
        private readonly string _outType;
        private readonly string _fromFormat;
        private readonly Dictionary<string, Func<object, object>> _map = Common.ObjectConversionMap;

        public ConvertOperation(string inKey, string inType, string outKey, string outType, string fromFormat = "")
            : base(inKey, outKey) {
            _inType = inType;
            _outType = outType;
            _fromFormat = fromFormat;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (_outType == "datetime" && !string.IsNullOrEmpty(_fromFormat)) {
                _map[_outType] = (x => DateTime.ParseExact(x.ToString(), _fromFormat, System.Globalization.CultureInfo.InvariantCulture));
            }
            if (_outType == "int32" && _inType == "datetime") {
                _map[_outType] = (x => Common.DateTimeToInt32((DateTime)x));
            }

            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _map[_outType](row[InKey]);
                }
                yield return row;
            }
        }
    }
}