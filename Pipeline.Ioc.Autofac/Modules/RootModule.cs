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
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Ext;
using Cfg.Net.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Ioc.Autofac.Impl;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Globalization;
using Module = Autofac.Module;
using Parameter = Autofac.Core.Parameter;

// ReSharper disable PossibleMultipleEnumeration

namespace Transformalize.Ioc.Autofac.Modules {


    public class RootModule : Module {

        protected override void Load(ContainerBuilder builder) {

            // This reader is used to load the initial configuration and nested resources for tfl actions, etc.
            builder.RegisterType<FileReader>().Named<IReader>("file");
            builder.RegisterType<WebReader>().Named<IReader>("web");
            builder.Register<IReader>(ctx => new DefaultReader(
                ctx.ResolveNamed<IReader>("file"),
                new ReTryingReader(ctx.ResolveNamed<IReader>("web"), attempts: 3))
            );

            builder.Register((ctx, p) => {

                var placeHolderStyle = "@()";
                if (ctx.IsRegisteredWithName<string>("placeHolderStyle")) {
                    placeHolderStyle = ctx.ResolveNamed<string>("placeHolderStyle");
                }

                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    new FormParameterModifier(new DateMathModifier()),
                    new EnvironmentModifier(new PlaceHolderReplacer(placeHolderStyle[0], placeHolderStyle[1], placeHolderStyle[2]))
                };

                if (ctx.IsRegisteredWithName<IDependency>("shorthand-t")) {
                    dependencies.Add(ctx.ResolveNamed<IDependency>("shorthand-t"));
                }

                if (ctx.IsRegisteredWithName<IDependency>("shorthand-v")) {
                    dependencies.Add(ctx.ResolveNamed<IDependency>("shorthand-v"));
                }

                var process = GetProcess(ctx, p, dependencies, TransformConfiguration(ctx, p));

                if (process.Errors().Any()) {
                    return process;
                }

                // this might be put into it's own type and injected (or not)
                if (process.Entities.Count == 1) {
                    var entity = process.Entities.First();
                    if (!entity.HasInput() && ctx.IsRegistered<ISchemaReader>()) {
                        var schemaReader = ctx.Resolve<ISchemaReader>(new TypedParameter(typeof(Process), process));
                        var schema = schemaReader.Read(entity);
                        var newEntity = schema.Entities.First();
                        foreach (var sf in newEntity.Fields.Where(f => f.Name == Constants.TflKey || f.Name == Constants.TflDeleted || f.Name == Constants.TflBatchId || f.Name == Constants.TflHashCode)) {
                            sf.Alias = newEntity.Name + sf.Name;
                        }
                        process.Entities.Clear();
                        process.Entities.Add(newEntity);
                        process.Connections.First(c => c.Name == newEntity.Connection).Delimiter = schema.Connection.Delimiter;
                    }
                }

                if (process.Output().Provider == Constants.DefaultSetting || process.Output().Provider == "internal") {
                    try {
                        Console.WindowHeight = Console.WindowHeight + 1 - 1;
                        Console.Title = process.Name;
                        if (!System.Environment.CommandLine.Contains("TESTWINDOW") && !System.Environment.CommandLine.Contains("TESTPLATFORM")) {
                            process.Output().Provider = "console";
                        }
                    } catch (IOException) {

                        // just a hack to determine if in console
                    }
                }

                // handling multiple entities with non-relational output
                var originalOutput = process.Output().Clone();
                originalOutput.Name = Constants.OriginalOutput;
                originalOutput.Key = process.Name + originalOutput.Name;

                if (!process.OutputIsRelational() && (process.Entities.Count > 1 || process.Buffer)) {

                    process.Output().Provider = process.InternalProvider;
                    var folder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), Constants.ApplicationFolder);
                    var file = new FileInfo(Path.Combine(folder, SlugifyTransform.Slugify(process.Name) + "." + (process.InternalProvider == "sqlce" ? "sdf" : "sqlite3")));
                    var exists = file.Exists;
                    process.Output().File = file.FullName;
                    process.Output().RequestTimeout = process.InternalProvider == "sqlce" ? 0 : process.Output().RequestTimeout;
                    process.Flatten = process.InternalProvider == "sqlce";
                    process.Mode = exists ? process.Mode : "init";
                    process.Connections.Add(originalOutput);

                    if (!exists) {
                        if (!Directory.Exists(file.DirectoryName)) {
                            Directory.CreateDirectory(folder);
                        }
                    }
                }

                return process;

            }).As<Process>().InstancePerDependency();  // because it has state, if you run it again, it's not so good

        }

        private static string TransformConfiguration(IComponentContext ctx, IEnumerable<Parameter> p) {

            // short hand for parameters is defined, try to transform parameters in advance
            if (!ctx.IsRegisteredWithName<IDependency>("shorthand-p"))
                return null;

            if (!p.Any() && !ctx.IsRegisteredWithName<string>("cfg"))
                return null;

            var dependencies = new List<IDependency> {
                ctx.Resolve<IReader>(),
                new DateMathModifier(),
                new EnvironmentModifier(new NullPlaceHolderReplacer()),
                ctx.ResolveNamed<IDependency>("shorthand-p")
            };

            var preCfg = (p.Any() ? p.Named<string>("cfg") : null) ?? ctx.ResolveNamed<string>("cfg");
            var preProcess = new ConfigurationFacade.Process(preCfg, new Dictionary<string, string>(), dependencies.ToArray());

            var parameters = preProcess.GetActiveParameters();
            if (!parameters.Any(pr => pr.Transforms.Any()))
                return null;

            var fields = parameters.Select(pr => new Field {
                Name = pr.Name,
                Alias = pr.Name,
                Default = pr.Value,
                Type = pr.Type,
                Transforms = pr.Transforms.Select(o => o.ToOperation()).ToList()
            }).ToList();
            var len = fields.Count;
            var entity = new Entity { Name = "Parameters", Alias = "Parameters", Fields = fields };
            var mini = new Process {
                Name = "ParameterTransform",
                ReadOnly = true,
                Entities = new List<Entity> { entity },
                Maps = preProcess.Maps.Select(m => m.ToMap()).ToList(), // for map transforms
                Scripts = preProcess.Scripts.Select(s => s.ToScript()).ToList() // for transforms that use scripts (e.g. js)
            };

            mini.Check(); // very important to check after creating, as it runs validation and even modifies!

            if (!mini.Errors().Any()) {

                // modification in Check() do not make it out to local variables so overwrite them
                fields = mini.Entities.First().Fields;
                entity = mini.Entities.First();

                var transforms = TransformFactory.GetTransforms(ctx, new PipelineContext(ctx.Resolve<IPipelineLogger>(), mini, entity), fields);

                // make an input out of the parameters
                var input = new List<IRow>();
                var row = new MasterRow(len);
                for (var i = 0; i < len; i++) {
                    row[fields[i]] = parameters[i].Value;
                }

                input.Add(row);

                var output = transforms.Aggregate(input.AsEnumerable(), (rows, t) => t.Operate(rows)).ToList().First();

                for (var i = 0; i < len; i++) {
                    var parameter = parameters[i];
                    parameter.Value = output[fields[i]].ToString();
                    parameter.T = string.Empty;
                    parameter.Transforms.Clear();
                }

                return preProcess.Serialize();
            }

            var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), mini, entity);
            foreach (var error in mini.Errors()) {
                context.Error(error);
            }

            return null;
        }

        // this is confusing, refactor
        private static Process GetProcess(IComponentContext ctx, IEnumerable<Parameter> p, IEnumerable<IDependency> dependencies, string cfg = null) {
            var process = new Process(dependencies.ToArray());
            if (cfg != null) {
                process.Load(cfg);
                return process;
            }

            switch (p.Count()) {
                case 2:
                    process.Load(
                        p.Named<string>("cfg"),
                        p.Named<Dictionary<string, string>>("parameters")
                    );
                    return process;
                case 1:
                    process.Load(p.Named<string>("cfg"));
                    return process;
                default:
                    if (ctx.IsRegisteredWithName<string>("cfg")) {
                        process.Load(ctx.ResolveNamed<string>("cfg"));
                        return process;
                    }

                    return process;  // unloaded

            }
        }

    }
}
