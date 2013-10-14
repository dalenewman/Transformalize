using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ConvertOperation : AbstractOperation {

        private readonly string _outKey;
        private readonly string _inKey;
        private readonly string _inType;
        private readonly string _outType;
        private readonly string _format;
        private readonly Dictionary<string, Func<object, object>> _map = Common.ObjectConversionMap;

        public ConvertOperation(string inKey, string inType, string outKey, string outType, string format) {
            _outKey = outKey;
            _inKey = inKey;
            _inType = inType;
            _outType = outType;
            _format = format;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (_outType == "datetime" && !string.IsNullOrEmpty(_format)) {
                _map[_outType] = (x => DateTime.ParseExact(x.ToString(), _format, System.Globalization.CultureInfo.InvariantCulture));
            }
            if (_outType == "int32" && _inType == "datetime") {
                _map[_outType] = (x => Common.DateTimeToInt32((DateTime)x));
            }

            foreach (var row in rows) {
                row[_outKey] = _map[_outType](row[_inKey]);
                yield return row;
            }
        }
    }
}