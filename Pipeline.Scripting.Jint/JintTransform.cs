#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Cfg.Net.Ext;
using Jint;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Scripting.Jint {
    public class JintTransform : BaseTransform, ITransform {
        readonly Field[] _input;
        readonly Engine _jint = new Engine();
        readonly Dictionary<int, string> _errors = new Dictionary<int, string>();

        public JintTransform(PipelineContext context, IReader reader) : base(context) {

            // automatic parameter binding
            if (!context.Transform.Parameters.Any()) {
                var parameters = new global::Jint.Parser.JavaScriptParser().Parse(context.Transform.Script, new global::Jint.Parser.ParserOptions { Tokens = true }).Tokens
                    .Where(o => o.Type == global::Jint.Parser.Tokens.Identifier)
                    .Select(o => o.Value.ToString())
                    .Intersect(context.GetAllEntityFields().Select(f => f.Alias))
                    .Distinct()
                    .ToArray();
                if (parameters.Any()) {
                    foreach (var parameter in parameters) {
                        context.Transform.Parameters.Add(new Parameter { Field = parameter }.WithDefaults());
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
            if (context.Transform.Scripts.Any()) {
                foreach (var sc in context.Transform.Scripts) {
                    ProcessScript(context, reader, context.Process.Scripts.First(s => s.Name == sc.Name));
                }
            }

            // make this reference the host field
            context.Transform.Script = $"var self = {context.Field.Alias};\r\n{context.Transform.Script}";
            context.Debug(() => $"Script in {context.Field.Alias} : {context.Transform.Script.Replace("{", "{{").Replace("}", "}}")}");
        }

        void ProcessScript(PipelineContext context, IReader reader, Script script) {
            script.Content = ReadScript(context, reader, script);
            var parser = new JintParser();

            if (parser.Parse(script.Content, context.Error)) {
                _jint.Execute(script.Content);
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
        static string ReadScript(PipelineContext context, IReader reader, Script script) {
            var content = string.Empty;

            if (script.Content != string.Empty)
                content += script.Content + "\r\n";

            

            if (script.File != string.Empty) {
                var p = new Dictionary<string,string>();
                var l = new Cfg.Net.Loggers.MemoryLogger();
                var response = reader.Read(script.File, p, l);
                if (l.Errors().Any()) {
                    foreach (var error in l.Errors())
                    {
                        context.Error(error);
                    }
                    context.Error($"Could not load {script.File}.");
                } else {
                    content += response + "\r\n";
                }

            }

            return content;
        }

        public override IRow Transform(IRow row) {
            foreach (var field in _input) {
                _jint.SetValue(field.Alias, row[field]);
            }
            try {
                var value = Context.Field.Convert(_jint.Execute(Context.Transform.Script).GetCompletionValue().ToObject());
                if (value == null && !_errors.ContainsKey(0)) {
                    Context.Error($"Jint transform in {Context.Field.Alias} returns null!");
                    _errors[0] = $"Jint transform in {Context.Field.Alias} returns null!";
                } else {
                    row[Context.Field] = value;
                }
            } catch (global::Jint.Runtime.JavaScriptException jse) {
                if (!_errors.ContainsKey(jse.LineNumber)) {
                    Context.Error("Script: " + Context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
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
