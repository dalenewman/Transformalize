using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {

    public class CSharpHost : MarshalByRefObject, IDisposable {

        public delegate object UserCodeInvoker(object[] input);

        private readonly IContext _context;
        private readonly IWriteSomething _codeWriter;
        private readonly string _className;
        private readonly AppDomain _appDomain;

        public static ConcurrentDictionary<string, UserCodeInvoker> Cache { get; } = new ConcurrentDictionary<string, UserCodeInvoker>();

        public CSharpHost(IContext context, IWriteSomething codeWriter, string className = "UserCode") {
            _context = context;
            _codeWriter = codeWriter;
            _className = className;
            _context.Info("Creating new app domain.");
            _appDomain = AppDomain.CreateDomain(_context.Key);
        }

        public void Start() {
            var host = Path.Combine(AssemblyDirectory, "Pipeline.Desktop.dll");
            var timer = new Stopwatch();
            timer.Start();

            var compiler = (CSharpCompiler)_appDomain.CreateInstanceFromAndUnwrap(host, "Pipeline.Desktop.Transforms.CSharpCompiler", false, BindingFlags.Default, null, null, null, null);

            try {
                compiler.Compile(_codeWriter.Write(_className));

                if (compiler.Assembly == null) {
                    foreach (var error in compiler.Errors) {
                        _context.Error(error);
                    }
                    return;
                }
                timer.Stop();
                _context.Info($"Compiled in {timer.Elapsed}");

                foreach (var method in compiler.Assembly.GetType(_className).GetMethods(BindingFlags.Static | BindingFlags.Public)) {
                    Cache.TryAdd(method.Name, (UserCodeInvoker)DynamicMethodHelper.ConvertFrom(method).CreateDelegate(typeof(UserCodeInvoker)));
                }

            } catch (Exception ex) {
                _context.Error(ex.Message);
            }

        }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public void Dispose() {
            Cache.Clear();
            AppDomain.Unload(_appDomain);
            _context.Info("Unloaded user code app domain.");
        }
    }
}