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
using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;
using Process = Transformalize.Configuration.Process;

namespace Transformalize.Ioc.Autofac {
   public static class DefaultContainer {

      public static ILifetimeScope Create(Process process, IPipelineLogger logger, string placeHolderStyle) {

         var loadContext = new PipelineContext(logger, process);

         if (process.OutputIsConsole()) {
            logger.SuppressConsole();
         }

         var builder = new ContainerBuilder();
         builder.Properties["Process"] = process;

         builder.Register(ctx => placeHolderStyle).Named<string>("placeHolderStyle");
         builder.RegisterInstance(logger).As<IPipelineLogger>().SingleInstance();

         /* this stuff is loaded (again) because tfl actions can create processes, which will need short-hand to expand configuration in advance */
         builder.RegisterCallback(new TransformModule(process, logger).Configure);
         builder.RegisterCallback(new ValidateModule(process, logger).Configure);
         builder.RegisterCallback(new RootModule().Configure);
         builder.RegisterCallback(new ContextModule(process).Configure);

         // provider loading section
         var providers = new HashSet<string>(process.Connections.Select(c => c.Provider).Distinct(), StringComparer.OrdinalIgnoreCase);

         builder.RegisterCallback(new InternalModule(process).Configure);
         builder.RegisterCallback(new FileModule(process).Configure);

         if (providers.Contains("console")) { builder.RegisterCallback(new ConsoleModule(process).Configure); }
         if (providers.Contains("kml")) { builder.RegisterCallback(new KmlModule(process).Configure); }
         if (providers.Contains("filesystem")) { builder.RegisterCallback(new FileSystemModule(process).Configure); }

         var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
         if (Directory.Exists(pluginsFolder)) {

            var assemblies = new List<Assembly>();
            var files = Directory.GetFiles(pluginsFolder, "Transformalize.Provider.*.Autofac.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files) {
               var info = new FileInfo(file);
               var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "provider" && f != "autofac");

                // temporary hack
                if (name.StartsWith("amazonkinesis")) {
                    name = name.Replace("amazonkinesis", string.Empty);
                }

               switch (name) {
                  case "filehelpers" when (providers.Contains("file") || providers.Contains("folder")):
                     loadContext.Debug(() => "Loading filehelpers provider");
                     assemblies.Add(Assembly.LoadFile(new FileInfo(file).FullName));
                     break;
                  case "ado":
                     loadContext.Debug(() => "Loading ADO provider");
                     assemblies.Add(Assembly.LoadFile(new FileInfo(file).FullName));
                     break;
                  default:
                     if (providers.Contains(name)) {
                        loadContext.Debug(() => $"Loading {name} provider.");
                        var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                        assemblies.Add(assembly);
                     } else {
                        loadContext.Debug(() => $"Loading {name} isn't necessary for this arrangement.");
                     }
                     break;
               }
            }
            if (assemblies.Any()) {
               builder.RegisterAssemblyModules(assemblies.ToArray());
            }
         }

         // etc
         builder.RegisterCallback(new EntityPipelineModule(process).Configure);
         builder.RegisterCallback(new ProcessPipelineModule(process).Configure);
         builder.RegisterCallback(new ProcessControlModule(process).Configure);

         return builder.Build().BeginLifetimeScope();

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