using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Transformalize.Libs.Jint;
using Transformalize.Libs.Jint.Parser;
using Transformalize.Libs.Jint.Parser.Ast;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class JavascriptOperation : ShouldRunOperation {

        private readonly Engine _jint = new Engine();
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
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
                _log.Debug("Running script {0}.", pair.Value.File);
                try {
                    program = new JavaScriptParser().Parse(pair.Value.Content);
                    if (program.Errors != null && program.Errors.Count > 0) {
                        _log.Warn("Javascript Parse Failed. Script: {0}.", pair.Value.Name);
                        foreach (var error in program.Errors) {
                            _log.Warn(error.Description);
                        }
                    } else {
                        _scriptAppender.AppendLine(pair.Value.Content);
                    }
                } catch (Exception e) {
                    _log.Error("Javascript Parse Failed. Name: {0}. Script: {1}.", pair.Value.Name, pair.Value.Content);
                    _log.Error(e.Message);
                }
            }

            try {
                program = new JavaScriptParser().Parse(_script);
                if (program.Errors != null && program.Errors.Count > 0) {
                    _log.Warn("Javascript Parse Failed. Inline: {0}.", _script);
                    foreach (var error in program.Errors) {
                        _log.Warn(error.Description);
                    }
                }
            } catch (Exception e) {
                _log.Error("Javascript Parse Failed. Inline: '{0}'. Message: ", _script, e.Message);
            }

            var externalScripts = _scriptAppender.ToString();
            if (externalScripts.Equals(string.Empty))
                return;

            _log.Debug("Loading scripts into Javascript engine...");
            _log.Debug(externalScripts);
            _jint.Execute(externalScripts);
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