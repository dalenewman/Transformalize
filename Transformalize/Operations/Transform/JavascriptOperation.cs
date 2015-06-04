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

        public JavascriptOperation(
            string outKey, 
            string script, 
            Dictionary<string, Script> scripts, 
            IParameters parameters,
            ILogger logger)
            : base(string.Empty, outKey) {
            _script = script;
            _parameters = parameters;
            _addSelf = !parameters.Any();

            foreach (var pair in scripts) {
                logger.Debug("Running script {0}.", pair.Value.File);
                try
                {
                    var program = new JavaScriptParser().Parse(pair.Value.Content, new ParserOptions { Tolerant = true});
                    if (program.Errors != null && program.Errors.Count > 0) {
                        logger.Warn("Javascript Parse Failed. Script: {0}.", pair.Value.Name);
                        foreach (var error in program.Errors) {
                            logger.Warn(error.Description);
                        }
                    } else {
                        _scriptAppender.AppendLine(pair.Value.Content);
                    }
                }
                catch (Exception e) {
                    logger.Error("Javascript Parse Failed. Name: {0}. Script: {1}.", pair.Value.Name, pair.Value.Content);
                    logger.Error(e.Message);
                }
            }

            var externalScripts = _scriptAppender.ToString();
            if (externalScripts.Equals(string.Empty))
                return;

            logger.Debug("Loading scripts into Javascript engine...");
            logger.Debug(externalScripts.Replace("{","{{").Replace("}","}}"));
            _jint.Execute(externalScripts);

            Name = string.Format("Javascript({0}=>{1})", InKey, OutKey);
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