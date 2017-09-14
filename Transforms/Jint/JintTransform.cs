#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Jint;
using Jint.Parser;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms.Jint {

    public class JintTransform : BaseTransform {

        readonly Field[] _input;
        readonly Engine _jint = new Engine();
        readonly JavaScriptParser _parser = new JavaScriptParser();
        readonly Dictionary<int, string> _errors = new Dictionary<int, string>();
        private readonly ParserOptions _parserOptions = new ParserOptions { Tolerant = true };

        public JintTransform(IContext context, IReader reader) : base(context, null) {

            if (IsMissing(context.Operation.Script)) {
                return;
            }

            // automatic parameter binding
            if (!context.Operation.Parameters.Any()) {
                var parameters = _parser.Parse(context.Operation.Script, new global::Jint.Parser.ParserOptions { Tokens = true }).Tokens
                    .Where(o => o.Type == global::Jint.Parser.Tokens.Identifier)
                    .Select(o => o.Value.ToString())
                    .Intersect(context.GetAllEntityFields().Select(f => f.Alias))
                    .Distinct()
                    .ToArray();
                if (parameters.Any()) {
                    foreach (var parameter in parameters) {
                        context.Operation.Parameters.Add(new Parameter { Field = parameter });
                    }
                }
            }

            // for js, always add the input parameter
            _input = MultipleInput().Union(new[] { context.Field }).Distinct().ToArray();

            // load any global scripts
            foreach (var sc in context.Process.Scripts.Where(s => s.Global)) {
                ProcessScript(context, reader, context.Process.Scripts.First(s => s.Name == sc.Name));
            }

            // load any specified scripts
            if (context.Operation.Scripts.Any()) {
                foreach (var sc in context.Operation.Scripts) {
                    ProcessScript(context, reader, context.Process.Scripts.First(s => s.Name == sc.Name));
                }
            }

            // make this reference the host field
            context.Operation.Script = $"var self = {context.Field.Alias};\r\n{context.Operation.Script}";
            context.Debug(() => $"Script in {context.Field.Alias} : {context.Operation.Script.Replace("{", "{{").Replace("}", "}}")}");
        }

        void ProcessScript(IContext context, IReader reader, Script script) {
            script.Content = ReadScript(context, reader, script);

            try {
                var program = _parser.Parse(script.Content, _parserOptions);
                if (program?.Errors == null || !program.Errors.Any()) {
                    _jint.Execute(script.Content);
                    return;
                }

                foreach (var e in program.Errors) {
                    Context.Error("{0}, script: {1}...", e.Message, script.Content.Left(30).Replace("{", "{{").Replace("}", "}}"));
                }
            } catch (ParserException ex) {
                Context.Error("{0}, script: {1}...", ex.Message, script.Content.Left(30).Replace("{", "{{").Replace("}", "}}"));
            }

        }

        /// <summary>
        /// Read the script.  The script could be from the content attribute, 
        /// from a file referenced in the file attribute, or a combination.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reader"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        static string ReadScript(IContext context, IReader reader, Script script) {
            var content = string.Empty;

            if (script.Content != string.Empty)
                content += script.Content + "\r\n";

            if (script.File != string.Empty) {
                var p = new Dictionary<string, string>();
                var l = new Cfg.Net.Loggers.MemoryLogger();
                var response = reader.Read(script.File, p, l);
                if (l.Errors().Any()) {
                    foreach (var error in l.Errors()) {
                        context.Error(error);
                    }
                    context.Error($"Could not load {script.File}.");
                } else {
                    content += response + "\r\n";
                }
            }

            return content;
        }

        public override IRow Operate(IRow row) {
            foreach (var field in _input) {
                _jint.SetValue(field.Alias, row[field]);
            }
            try {
                var value = Context.Field.Convert(_jint.Execute(Context.Operation.Script).GetCompletionValue().ToObject());
                if (value == null && !_errors.ContainsKey(0)) {
                    Context.Error($"Jint transform in {Context.Field.Alias} returns null!");
                    _errors[0] = $"Jint transform in {Context.Field.Alias} returns null!";
                } else {
                    row[Context.Field] = value;
                }
            } catch (global::Jint.Runtime.JavaScriptException jse) {
                if (!_errors.ContainsKey(jse.LineNumber)) {
                    Context.Error("Script: " + Context.Operation.Script.Replace("{", "{{").Replace("}", "}}"));
                    Context.Error(jse, "Error Message: " + jse.Message);
                    Context.Error("Variables:");
                    foreach (var field in _input) {
                        Context.Error($"{field.Alias}:{row[field]}");
                    }
                    _errors[jse.LineNumber] = jse.Message;
                }
            }

            Increment();
            return row;
        }
    }
}
