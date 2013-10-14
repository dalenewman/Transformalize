using System.Collections.Generic;
using Noesis.Javascript;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class JavascriptOperation : AbstractOperation {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _script;
        private readonly string _outKey;
        private readonly IParameters _parameters;

        public JavascriptOperation(string outKey, string script, Dictionary<string, Script> scripts, IParameters parameters) {
            _script = script;
            _outKey = outKey;
            _parameters = parameters;

            foreach (var pair in scripts) {
                _log.Debug("Running script {0}.", pair.Value.File);
                _context.Run(pair.Value.Content);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                _context.SetParameter(_outKey, row[_outKey]);
                foreach (var pair in _parameters) {
                    _context.SetParameter(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
                }
                row[_outKey] = _context.Run(_script);
                yield return row;
            }
        }

        public override void Dispose() {
            _context.Dispose();
            base.Dispose();
        }
    }
}