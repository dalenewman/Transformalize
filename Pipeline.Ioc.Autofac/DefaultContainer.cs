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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
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
            builder.RegisterCallback(new ShorthandTransformModule().Configure);
            builder.RegisterCallback(new ValidateModule().Configure);
            builder.RegisterCallback(new ShorthandValidateModule().Configure);

            builder.RegisterCallback(new RootModule().Configure);
            builder.RegisterCallback(new ContextModule(process).Configure);

            // provider loading section
            var providers = new HashSet<string>(process.Connections.Select(c => c.Provider).Distinct(), StringComparer.OrdinalIgnoreCase);

            builder.RegisterCallback(new InternalModule(process).Configure);
            builder.RegisterCallback(new AdoModule(process).Configure);

            if (providers.Contains("solr")) { builder.RegisterCallback(new SolrModule(process).Configure); }
            if (providers.Contains("elasticsearch")) { builder.RegisterCallback(new ElasticModule(process).Configure); }
            if (providers.Contains("console")) { builder.RegisterCallback(new ConsoleModule(process).Configure); }
            if (providers.Contains("file")) { builder.RegisterCallback(new FileModule(process).Configure); }
            if (providers.Contains("geojson")) { builder.RegisterCallback(new GeoJsonModule(process).Configure); }
            if (providers.Contains("kml")) { builder.RegisterCallback(new KmlModule(process).Configure); }
            if (providers.Contains("folder")) { builder.RegisterCallback(new FolderModule(process).Configure); }
            if (providers.Contains("filesystem")) { builder.RegisterCallback(new FileSystemModule(process).Configure); }
            if (providers.Contains("excel")) { builder.RegisterCallback(new ExcelModule(process).Configure); }
            if (providers.Contains("web")) { builder.RegisterCallback(new WebModule(process).Configure); }
            if (providers.Contains("rethinkdb")) { builder.RegisterCallback(new RethinkDBModule(process).Configure); }

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {
                
                var assemblies = new List<Assembly>();
                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Provider.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
                    var info = new FileInfo(file);
                    var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "provider" && f != "autofac");
                    if (!providers.Contains(name))
                        continue;
                    loadContext.Debug(()=>$"Loading {name} provider");
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }
                if (assemblies.Any()) {
                    builder.RegisterAssemblyModules(assemblies.ToArray());
                }
            }

            // template providers
            builder.RegisterCallback(new RazorModule(process).Configure);

            // etc
            builder.RegisterCallback(new MapModule(process).Configure);
            builder.RegisterCallback(new TemplateModule(process).Configure);
            builder.RegisterCallback(new ActionModule(process).Configure);

            builder.RegisterCallback(new EntityPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessControlModule(process).Configure);

            return builder.Build().BeginLifetimeScope();

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