#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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

using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Transformalize.Configuration;
using Module = Autofac.Module;

namespace Transformalize.Containers.Autofac.Modules {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in transforms
   /// </summary>
   public class ProviderPlugInModule : Module {

      private readonly Process _process;

      public ProviderPlugInModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         // provider loading section
         var providers = new HashSet<string>(_process.Connections.Select(c => c.Provider).Distinct(), StringComparer.OrdinalIgnoreCase);

         var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
         if (Directory.Exists(pluginsFolder)) {

            var assemblies = new HashSet<Assembly>();
            foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Provider.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
               var info = new FileInfo(file);
               var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "provider" && f != "autofac");

               switch (name) {
                  case "filehelpers" when (providers.Contains("file") || providers.Contains("folder")):
                     assemblies.Add(Assembly.LoadFile(new FileInfo(file).FullName));
                     break;
                  case "ado":
                     assemblies.Add(Assembly.LoadFile(new FileInfo(file).FullName));
                     break;
                  default:
                     if (providers.Contains(name)) {
                        var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                        assemblies.Add(assembly);
                     }
                     break;
               }
            }
            if (assemblies.Any()) {
               builder.RegisterAssemblyModules(assemblies.ToArray());
            }
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
