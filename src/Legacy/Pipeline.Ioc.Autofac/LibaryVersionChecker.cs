#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
            try {
               var codeBase = typeof(Process).Assembly.CodeBase;
               var uri = new UriBuilder(codeBase);
               var path = Uri.UnescapeDataString(uri.Path);
               return Path.GetDirectoryName(path);
            } catch (Exception) {
               return ".";
            }
         }
      }
   }
}