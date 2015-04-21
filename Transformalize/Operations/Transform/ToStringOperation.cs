using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class ToStringOperation : ShouldRunOperation {

        private string _inType;
        private readonly string _format;
        private bool _gotType;

        private readonly Dictionary<string, Func<object, string, string>> _toString = new Dictionary<string, Func<object, string, string>>() {
            { "datetime", ((value,format) => (Convert.ToDateTime(value)).ToString(format))},
            { "int32", ((value,format) => (Convert.ToInt32(value)).ToString(format))},
            { "decimal", ((value,format) => (Convert.ToDecimal(value)).ToString(format))},
            { "timespan", ((value, format) => ((TimeSpan)value).ToString(format)) },
            { "double",  ((value,format) => (Convert.ToDouble(value)).ToString(format))},
            { "int16",  ((value,format) => (Convert.ToInt16(value)).ToString(format))},
            { "int64",  ((value,format) => (Convert.ToInt64(value)).ToString(format))},
            { "byte",  ((value,format) => (Convert.ToByte(value)).ToString(format))},
            { "float",  ((value,format) => ((float)value).ToString(format))},
            { "single",  ((value,format) => (Convert.ToSingle(value)).ToString(format))}
        };

        public ToStringOperation(string inKey, string inType, string outKey, string format)
            : base(inKey, outKey) {
            _inType = inType;
            _format = format;
            _gotType = _toString.ContainsKey(_inType);

            Name = string.Format("ToStringOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (_gotType) {
                foreach (var row in rows) {
                    if (ShouldRun(row)) {
                        row[OutKey] = _toString[_inType](row[InKey], _format);
                    } else {
                        Interlocked.Increment(ref SkipCount);
                    }
                    yield return row;
                }
            } else {
                foreach (var row in rows) {
                    if (ShouldRun(row)) {
                        if (!_gotType) {
                            _inType = Common.ToSimpleType(row[InKey].GetType().Name);
                            _gotType = true;
                        }
                        row[OutKey] = _toString[_inType](row[InKey], _format);
                    } else {
                        Interlocked.Increment(ref SkipCount);
                    }
                    yield return row;
                }
            }
        }
    }
}