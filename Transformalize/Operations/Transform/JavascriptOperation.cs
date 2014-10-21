using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Transformalize.Libs.Jint;
using Transformalize.Libs.Jint.Parser;
using Transformalize.Libs.Jint.Parser.Ast;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class JavascriptOperation : ShouldRunOperation {

        private readonly Engine _jint = new Engine();
        private readonly string _script;
        private readonly IParameters _parameters;
        private readonly bool _addSelf;
        private readonly StringBuilder _scriptAppender = new StringBuilder();

        public JavascriptOperation(string outKey, string script, Dictionary<string, Script> scripts, IParameters parameters)
            : base(string.Empty, outKey) {
            _script = script;
            _parameters = parameters;
            _addSelf = !parameters.Any();

            Program program;

            foreach (var pair in scripts) {
                TflLogger.Debug(string.Empty, string.Empty, "Running script {0}.", pair.Value.File);
                try {
                    program = new JavaScriptParser().Parse(pair.Value.Content);
                    if (program.Errors != null && program.Errors.Count > 0) {
                        TflLogger.Warn(string.Empty, string.Empty, "Javascript Parse Failed. Script: {0}.", pair.Value.Name);
                        foreach (var error in program.Errors) {
                            TflLogger.Warn(string.Empty, string.Empty, error.Description);
                        }
                    } else {
                        _scriptAppender.AppendLine(pair.Value.Content);
                    }
                } catch (Exception e) {
                    TflLogger.Error(string.Empty, string.Empty, "Javascript Parse Failed. Name: {0}. Script: {1}.", pair.Value.Name, pair.Value.Content);
                    TflLogger.Error(string.Empty, string.Empty, e.Message);
                }
            }

            try {
                program = new JavaScriptParser().Parse(_script);
                if (program.Errors != null && program.Errors.Count > 0) {
                    TflLogger.Warn(string.Empty, string.Empty, "Javascript Parse Failed. Inline: {0}.", _script);
                    foreach (var error in program.Errors) {
                        TflLogger.Warn(string.Empty, string.Empty, error.Description);
                    }
                }
            } catch (Exception e) {
                TflLogger.Error(string.Empty, string.Empty, "Javascript Parse Failed. Inline: '{0}'. Message: ", _script, e.Message);
            }

            var externalScripts = _scriptAppender.ToString();
            if (externalScripts.Equals(string.Empty))
                return;

            TflLogger.Debug(string.Empty, string.Empty, "Loading scripts into Javascript engine...");
            TflLogger.Debug(string.Empty, string.Empty, externalScripts);
            _jint.Execute(externalScripts);

            Name = string.Format("JavascriptOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_addSelf) {
                        _jint.SetValue(OutKey, row[OutKey]);
                    }
                    foreach (var pair in _parameters) {
                        _jint.SetValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
                    }
                    row[OutKey] = _jint.Execute(_script).GetCompletionValue().ToObject();
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}