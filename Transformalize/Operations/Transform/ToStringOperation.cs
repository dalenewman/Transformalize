using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public class ToStringOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _inType;
        private readonly string _outKey;
        private readonly string _format;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Func<object, string, string>> _toString = new Dictionary<string, Func<object, string, string>>() {
            { "datetime", ((value,format) => (Convert.ToDateTime(value)).ToString(format))},
            { "int32", ((value,format) => (Convert.ToInt32(value)).ToString(format))},
            { "decimal", ((value,format) => (Convert.ToDecimal(value)).ToString(format))},
            { "double",  ((value,format) => (Convert.ToDouble(value)).ToString(format))},
            { "int16",  ((value,format) => (Convert.ToInt16(value)).ToString(format))},
            { "int64",  ((value,format) => (Convert.ToInt64(value)).ToString(format))},
            { "byte",  ((value,format) => (Convert.ToByte(value)).ToString(format))},
            { "float",  ((value,format) => ((float)value).ToString(format))},
            { "single",  ((value,format) => (Convert.ToSingle(value)).ToString(format))},
        };

        public ToStringOperation(string inKey, string inType, string outKey, string format) {
            _inKey = inKey;
            _inType = inType;
            _outKey = outKey;
            _format = format;

            if (_inType == "string") {
                _log.Error("You have ToString transform on {0}.  It is already a string.", _inKey);
                Environment.Exit(1);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = _toString[_inType](row[_inKey], _format);
                yield return row;
            }

        }
    }
}