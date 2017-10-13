using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;
using Orchard.Logging;
using Orchard.Templates.Compilation.Razor;
using Orchard.Utility.Extensions;

namespace Pipeline.Web.Orchard.Impl {
    public class RazorReportCompiler : IRazorCompiler {

        private const string DynamicallyGeneratedClassName = "RazorReportTemplate";
        private const string NamespaceForDynamicClasses = "Orchard.Compilation.RazorReport";
        private static readonly ConcurrentDictionary<string, Assembly> _cache = new ConcurrentDictionary<string, Assembly>();
        private static readonly string[] DefaultNamespaces = {
            "System",
            "System.Linq",
            "System.Collections",
            "System.Collections.Generic",
            "System.Dynamic",
            "System.Text",
            "Orchard.Templates.Compilation.Razor"
        };

        public RazorReportCompiler() {
            Logger = NullLogger.Instance;
        }

        private ILogger Logger { get; set; }

        public IRazorTemplateBase<TModel> CompileRazor<TModel>(string code, string name, IDictionary<string, object> parameters) {
            return (RazorTemplateBase<TModel>)Compile(code, name);
        }

        public IRazorTemplateBase CompileRazor(string code, string name, IDictionary<string, object> parameters) {
            return (IRazorTemplateBase)Compile(code, name);
        }

        private static object Compile(string code, string name) {

            var cacheKey = (name ?? DynamicallyGeneratedClassName) + GetHash(code);
            var generatedClassName = name != null ? name.Strip(c => !c.IsLetter() && !Char.IsDigit(c)) : DynamicallyGeneratedClassName;

            Assembly assembly;
            if (_cache.TryGetValue(cacheKey, out assembly)) {
                return assembly.CreateInstance(NamespaceForDynamicClasses + "." + generatedClassName);
            }

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language) {
                DefaultBaseClass = "RazorTemplateBase<dynamic>",
                DefaultClassName = generatedClassName,
                DefaultNamespace = NamespaceForDynamicClasses
            };

            foreach (var n in DefaultNamespaces) {
                host.NamespaceImports.Add(n);
            }

            var engine = new RazorTemplateEngine(host);
            var razorTemplate = engine.GenerateCode(new StringReader(code));
            assembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode, name);
            _cache.TryAdd(cacheKey, assembly);

            return assembly.CreateInstance(NamespaceForDynamicClasses + "." + generatedClassName);
        }

        public static string GetHash(string value) {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new MD5CryptoServiceProvider().ComputeHash(data);

            var strBuilder = new StringBuilder();
            hashData.Aggregate(strBuilder, (current, b) => strBuilder.Append((byte)b));

            return strBuilder.ToString();
        }

        private static Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile, string templateName) {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.AddRange(AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .ToArray());

            compilerParameters.GenerateInMemory = true;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            if (compilerResults.Errors.HasErrors) {
                var errors = compilerResults.Errors.Cast<CompilerError>().Aggregate(string.Empty, (s, error) => s + "\r\nTemplate '" + templateName + "': " + error.ToString());
                throw new Exception(string.Format("Razor template compilation errors:\r\n{0}", errors));
            }

            var compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }
    }
}