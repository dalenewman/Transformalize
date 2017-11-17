using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Transformalize.Contracts;
using Process = Transformalize.Configuration.Process;

namespace Transformalize.Ioc.Autofac
{
    public class LibaryVersionChecker {

        private readonly IContext _context;

        public LibaryVersionChecker(IContext context) {
            _context = context;
        }

        public void Check() {
            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {
                var versions = new Dictionary<string, string>();
                foreach (var file in Directory.GetFiles(AssemblyDirectory, "*.dll", SearchOption.AllDirectories)) {
                    var fileInfo = new FileInfo(file);
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
                    if (versions.ContainsKey(fileVersionInfo.FileName)) {
                        if (versions[fileVersionInfo.FileName] != fileVersionInfo.FileVersion) {
                            _context.Warn($"There is more than one version of {fileVersionInfo.FileName}.");
                            _context.Warn($"The 1st version is {versions[fileVersionInfo.FileName]}.");
                            _context.Warn($"The 2nd version is {fileVersionInfo.FileVersion}");
                        }
                    } else {
                        versions[fileVersionInfo.FileName] = fileVersionInfo.FileVersion;
                    }
                }
            } else {
                _context.Debug(() => "No plugins folder found");
            }
        }


        public static string AssemblyDirectory {
            get {
                var codeBase = typeof(Process).Assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}