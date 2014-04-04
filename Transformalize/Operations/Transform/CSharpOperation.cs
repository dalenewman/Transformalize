using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Transformalize.Libs.Mono.CSharp;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class CSharpOperation : ShouldRunOperation {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly string _outType;
        private readonly string _script;
        private readonly IParameters _parameters;
        private readonly bool _addSelf;
        private readonly StringBuilder _scriptAppender = new StringBuilder(string.Empty);
        private readonly Evaluator _engine;
        public IDictionary<string, object> PipeVars { get; private set; }

        public CSharpOperation(string outKey, string outType, string script, Dictionary<string, Script> scripts, IParameters parameters)
            : base(string.Empty, outKey) {
            _outType = outType;
            _script = script;
            _parameters = parameters;
            _addSelf = !parameters.Any();

            var settings = new CompilerSettings { Optimize = true };
            _engine = new Evaluator(new CompilerContext(settings, new ConsoleReportPrinter()));

            var commonLines = new[] {
                "using System;",
                "using System.Collections.Generic;",
                "using System.Linq;"
            };

            foreach (var line in commonLines) {
                _engine.Run(line);
                _scriptAppender.AppendLine(line);
            }

            foreach (var pair in scripts) {
                _engine.Run(pair.Value.Content);
                _scriptAppender.AppendLine(pair.Value.Content);
            }

            const string conduit = "var pipeVars = new Dictionary<string, object>();";
            _engine.Run(conduit);
            PipeVars = _engine.Evaluate("pipeVars") as IDictionary<string, object>;
            _scriptAppender.AppendLine(conduit); ;

            string declaration;
            if (_addSelf) {
                declaration = String.Format("{0} {1};", Common.ToSystemType(_outType), OutKey);
                _engine.Run(declaration);
                _scriptAppender.AppendLine(declaration);
            }
            foreach (var pair in _parameters) {
                declaration = String.Format("{0} {1};", Common.ToSystemType(pair.Value.SimpleType), pair.Value.Name);
                _engine.Run(declaration);
                _scriptAppender.AppendLine(declaration);
            }

            Debug("Initialized CSharp engine...");
            Debug(_scriptAppender.ToString());
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_addSelf) {
                        SetParameter(OutKey, _outType, row[OutKey]);
                    }
                    foreach (var pair in _parameters) {
                        SetParameter(pair.Value.Name, pair.Value.SimpleType, pair.Value.Value ?? row[pair.Key]);
                    }
                    row[OutKey] = _engine.Evaluate(_script);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
                yield return row;
            }
        }

        private void SetParameter(string name, string simpleType, object value) {
            PipeVars[name] = value;
            var declaration = String.Format("{0} = ({1}) pipeVars[\"{0}\"]", name, Common.ToSystemType(simpleType));
            _engine.Run(declaration);
        }

    }
}