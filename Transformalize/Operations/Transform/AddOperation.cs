using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class AddOperation : ShouldRunOperation {
        private readonly string[] _parameterKeys;
        private readonly string _outType;
        private readonly IParameters _parameters;
        private readonly Dictionary<string, Func<object, object>> _map;
        private bool _typeCheck;

        public AddOperation(string outKey, string outType, IParameters parameters)
            : base(string.Empty, outKey) {
            _outType = outType;
            _parameters = parameters;
            _parameterKeys = parameters.Keys.ToArray();
            _map = Common.GetObjectConversionMap();
            Name = "Add (" + outKey + ")";

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_typeCheck) {
                        var r = row;
                        row[OutKey] = _parameterKeys.Select(k => r[k] ?? _parameters[k].Value).Sum(k => (dynamic)_map[_outType](k));
                    } else {
                        _typeCheck = true;
                        try {
                            var r = row;
                            row[OutKey] = _parameterKeys.Select(k => r[k] ?? _parameters[k].Value).Sum(k => (dynamic)_map[_outType](k));
                        } catch (Exception ex) {
                            throw new TransformalizeException(Logger, "The paramter types passed in to the add operation are not compatible for adding.  Check types for parameters {0}. {1}", string.Join(", ", _parameterKeys), ex.Message);
                        }
                    }
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}