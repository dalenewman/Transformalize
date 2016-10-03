using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pipeline.Desktop.Transforms {
    public class CSharpCompiler : MarshalByRefObject {

        public Assembly Assembly { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public void Compile(string code) {

            var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters {
                GenerateInMemory = true,
                GenerateExecutable = false,
                OutputAssembly = Path.Combine(CSharpHost.AssemblyDirectory, Path.GetRandomFileName() + ".dll")
            };

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            try {
                var result = codeProvider.CompileAssemblyFromSource(parameters, code);
                if (result.Errors.Count > 0) {
                    Errors.Add("CSharp Compiler Error!");
                    Errors.AddRange(from object error in result.Errors select error.ToString());
                } else {
                    Assembly = result.CompiledAssembly;
                }

            } catch (Exception ex) {
                Errors.AddRange(new[] { "CSharp Compiler Exception!", ex.Message });
            }

        }

        public override object InitializeLifetimeService() {
            return null;
        }

    }
}