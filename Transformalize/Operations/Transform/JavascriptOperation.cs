using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Jint;
using Transformalize.Libs.Jint.Parser.Ast;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class JavascriptOperation : ShouldRunOperation {

        private readonly Engine _engine = new Engine();
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly string _script;
        private readonly IParameters _parameters;

        public JavascriptOperation(string outKey, string script, Dictionary<string, Script> scripts, IParameters parameters)
            : base(string.Empty, outKey) {
            _script = script;
            _parameters = parameters;

            foreach (var pair in scripts) {
                _log.Debug("Running script {0}.", pair.Value.File);
                _engine.Execute(pair.Value.Content);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    foreach (var pair in _parameters) {
                        _engine.SetValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
                    }
                    row[OutKey] = _engine.Execute(_script).GetCompletionValue().ToObject();
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}