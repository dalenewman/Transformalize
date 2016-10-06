using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Pipeline.Contracts;

namespace Pipeline.Scripting.CSharp {

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

            try {
                var result = codeProvider.CompileAssemblyFromSource(parameters, _codeWriter.Write(_className));
                if (result.Errors.Count > 0) {
                    _context.Error("CSharp Compiler Error!");
                    foreach (var error in result.Errors) {
                        _context.Error(error.ToString());
                    }
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
                _context.Error("CSharp Compiler Exception!");
                _context.Error(ex.Message);
                return false;
            }
            return true;
        }

        public void Dispose() {
            // leave it, you can't recover the memory used by the dynamically generated (and loaded) assembly
        }
    }
}