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
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Transformalize.Contracts;

namespace Transformalize.Transforms.CSharp {

    public class CSharpHost : IHost {

        public delegate object UserCodeInvoker(object[] input);

        private readonly IContext _context;
        private readonly IWriteSomething _codeWriter;
        private readonly string _className;

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, UserCodeInvoker>> Cache { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, UserCodeInvoker>>();

        public CSharpHost(IContext context, IWriteSomething codeWriter, string className = "UserCode") {
            _context = context;
            _codeWriter = codeWriter;
            _className = className;
        }

        public bool Start() {

            if (Cache.ContainsKey(_context.Process.Name)) {
                _context.Debug(() => "Using cached user code.");
                return true;
            }

            var timer = new Stopwatch();
            timer.Start();

            var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            var code = _codeWriter.Write(_className);
            try {
                var result = codeProvider.CompileAssemblyFromSource(parameters, code);
                if (result.Errors.Count > 0) {
                    foreach (CompilerError error in result.Errors) {
                        _context.Error($"C# error on line {error.Line}, column {error.Column}.");
                        _context.Error(error.ErrorText);
                    }
                    CodeToError(code);
                } else {
                    timer.Stop();
                    _context.Info($"Compiled {_context.Process.Name} user code in {timer.Elapsed}.");
                    if (Cache.TryAdd(_context.Process.Name, new ConcurrentDictionary<string, UserCodeInvoker>())) {
                        foreach (var method in result.CompiledAssembly.GetType(_className).GetMethods(BindingFlags.Static | BindingFlags.Public)) {
                            Cache[_context.Process.Name].TryAdd(method.Name, (UserCodeInvoker)DynamicMethodHelper.ConvertFrom(method).CreateDelegate(typeof(UserCodeInvoker)));
                        }
                    }
                }

            } catch (Exception ex) {
                _context.Error("C# Compiler Exception!");
                _context.Error(ex.Message);
                CodeToError(code);
                return false;
            }
            return true;
        }

        private void CodeToError(string code) {
            var lineNo = 1;
            using (var sr = new StringReader(code)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    _context.Error($"{lineNo:0000} {line}");
                }
                ++lineNo;
            }
        }

        public void Dispose() {
            // leave it, you can't recover the memory used by the dynamically generated (and loaded) assembly
        }
    }
}