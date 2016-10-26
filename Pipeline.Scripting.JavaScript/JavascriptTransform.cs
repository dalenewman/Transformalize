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
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Scripting.JavaScript {
    public class JavascriptTransform : BaseTransform {

        readonly Field[] _input;
        readonly Dictionary<int, string> _errors = new Dictionary<int, string>();
        private readonly IJsEngine _engine;

        public JavascriptTransform(string engine, IContext context, IReader reader) : base(context, null) {

            var engineSwitcher = JsEngineSwitcher.Instance;
            engineSwitcher.EngineFactories.Add(new ChakraCoreJsEngineFactory());

            _engine = engineSwitcher.CreateEngine(engine);

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

        void ProcessScript(IContext context, IReader reader, Script script) {
            script.Content = ReadScript(context, reader, script);
            _engine.Execute(script.Content);
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

        public override IRow Transform(IRow row) {
            foreach (var field in _input) {
                _engine.SetVariableValue(field.Alias, row[field]);
            }
            try {
                var value = Context.Field.Convert(_engine.Evaluate(Context.Transform.Script));
                if (value == null && !_errors.ContainsKey(0)) {
                    Context.Error($"{_engine.Name} transform in {Context.Field.Alias} returns null!");
                    _errors[0] = $"{_engine.Name} transform in {Context.Field.Alias} returns null!";
                } else {
                    row[Context.Field] = value;
                }
            } catch (JsRuntimeException jse) {
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

        public override void Dispose() {
            if (_engine.SupportsGarbageCollection) {
                _engine.CollectGarbage();
            }
            _engine.Dispose();
        }
    }
}
