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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;

namespace Transformalize.Ioc.Autofac {
    public static class DefaultContainer {

        public static ILifetimeScope Create(Process process, IPipelineLogger logger, string placeHolderStyle) {

            if (process.OutputIsConsole()) {
                logger.SuppressConsole();
            }

            var builder = new ContainerBuilder();

            builder.Register(ctx => placeHolderStyle).Named<string>("placeHolderStyle");
            builder.RegisterInstance(logger).As<IPipelineLogger>().SingleInstance();
            builder.RegisterCallback(new TransformModule().Configure);
            builder.RegisterCallback(new ShorthandTransformModule().Configure);
            builder.RegisterCallback(new ValidateModule().Configure);
            builder.RegisterCallback(new ShorthandValidateModule().Configure);
            builder.RegisterCallback(new RootModule().Configure);
            builder.RegisterCallback(new ContextModule(process).Configure);

            // providers
            builder.RegisterCallback(new AdoModule(process).Configure);
            builder.RegisterCallback(new LuceneModule(process).Configure);
            builder.RegisterCallback(new SolrModule(process).Configure);
            builder.RegisterCallback(new ElasticModule(process).Configure);
            builder.RegisterCallback(new InternalModule(process).Configure);
            builder.RegisterCallback(new ConsoleModule(process).Configure);
            builder.RegisterCallback(new FileModule(process).Configure);
            builder.RegisterCallback(new GeoJsonModule(process).Configure);
            builder.RegisterCallback(new KmlModule(process).Configure);
            builder.RegisterCallback(new FolderModule(process).Configure);
            builder.RegisterCallback(new DirectoryModule(process).Configure);
            builder.RegisterCallback(new ExcelModule(process).Configure);
            builder.RegisterCallback(new WebModule(process).Configure);
            builder.RegisterCallback(new SSASModule(process).Configure);
            builder.RegisterCallback(new RethinkDBModule(process).Configure);

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {

                builder.Properties["Process"] = process;
                var assemblies = new List<Assembly>();
                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Provider.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }
                builder.RegisterAssemblyModules(assemblies.ToArray());
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