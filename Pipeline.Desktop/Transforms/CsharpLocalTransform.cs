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
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {

    public class CsharpLocalTransform : CSharpBaseTransform {

        private readonly Func<object[], object> _userCode;

        private static ConcurrentDictionary<string, Func<object[], object>> _cache;
        private static ConcurrentDictionary<string, Func<object[], object>> Cache => _cache ?? (_cache = new ConcurrentDictionary<string, Func<object[], object>>());

        public CsharpLocalTransform(IContext context) : base(context) {

            var timer = new Stopwatch();
            timer.Start();

            if (!Cache.TryGetValue(context.Key, out _userCode)) {
                var input = MultipleInput();
                var code = WrapCode(input, Context.Transform.Script, context.Entity.IsMaster);
                context.Debug((() => code));


                var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                var parameters = new CompilerParameters {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("mscorlib.dll");
                parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                var sb = new StringBuilder();

                try {
                    var result = codeProvider.CompileAssemblyFromSource(parameters, code);
                    if (result.Errors.Count > 0) {
                        context.Error("CSharp Compiler Error!");
                        foreach (var error in result.Errors) {
                            context.Error(error.ToString());
                        }
                        context.Error(context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                    } else {
                        var type = result.CompiledAssembly.GetType("CSharpRunTimeTransform");
                        var methodInfo = type.GetMethod("UserCode", BindingFlags.Static | BindingFlags.Public);
                        _userCode = (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), methodInfo);
                        context.Info($"Compiled user's code in {timer.Elapsed}");
                        if (Cache.TryAdd(context.Key, _userCode)) {
                            context.Info("Cached user's code.");
                        }
                    }
                } catch (Exception ex) {
                    sb.AppendLine("CSharp Compiler Exception!");
                    sb.AppendLine(ex.Message);
                    context.Error(context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                }
            }

            timer.Stop();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _userCode(row.ToArray());
            Increment();
            return row;
        }

    }

}