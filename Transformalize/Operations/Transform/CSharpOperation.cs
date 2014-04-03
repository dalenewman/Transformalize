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
        private readonly string _script;
        private readonly IParameters _parameters;
        private readonly bool _addSelf;
        private readonly StringBuilder _scriptAppender = new StringBuilder();

        public Evaluator Engine { get; private set; }
        public IDictionary<string, dynamic> Dictionary { get; private set; }

        public CSharpOperation(string outKey, string script, Dictionary<string, Script> scripts, IParameters parameters)
            : base(string.Empty, outKey) {
            _script = script;
            _parameters = parameters;
            _addSelf = !parameters.Any();

            foreach (var pair in scripts) {
                _scriptAppender.AppendLine(pair.Value.Content);
            }

            Engine = new Evaluator(new CompilerContext(new CompilerSettings(), new ConsoleReportPrinter()));

            Engine.Run("using System;");
            Engine.Run("using System.Collections.Generic;");
            Engine.Run("using System.Linq;");
            Engine.Run("var dictionary = new Dictionary<string, dynamic>();");
            Dictionary = Engine.Evaluate("dictionary") as IDictionary<string, dynamic>;

            if (!_scriptAppender.ToString().Equals(string.Empty)) {
                Engine.Run(_scriptAppender.ToString());
            }

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_addSelf) {
                        SetParameter(OutKey, row[OutKey]);
                    }
                    foreach (var pair in _parameters) {
                        SetParameter(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
                    }
                    row[OutKey] = Engine.Evaluate(_script);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
                yield return row;
            }
        }

        private void SetParameter(string name, object value) {
            Dictionary[name] = value;
            Engine.Run(String.Format("dynamic {0} = dictionary[\"{0}\"]", name));
        }

        public object Evaluate(string script) {
            object result;
            bool resultSet;

            Engine.Evaluate(script, out result, out resultSet);
            if (resultSet) {
                return result;
            }

            return null;
        }

    }
}