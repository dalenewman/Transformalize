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
using System.Linq;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Ext;
using Cfg.Net.Reader;
using Transformalize.Configuration;
using Transformalize.Contracts;
using System.IO;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Globalization;
using Transformalize.Transforms.Jint;

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
                    new DateMathModifier(),
                    new EnvironmentModifier(new PlaceHolderReplacer(placeHolderStyle[0], placeHolderStyle[1], placeHolderStyle[2])),
                    new JintValidator()
                };

                if (ctx.IsRegisteredWithName<IDependency>("shorthand-t")) {
                    dependencies.Add(ctx.ResolveNamed<IDependency>("shorthand-t"));
                }

                if (ctx.IsRegisteredWithName<IDependency>("shorthand-v")) {
                    dependencies.Add(ctx.ResolveNamed<IDependency>("shorthand-v"));
                }

                var process = new Process(dependencies.ToArray());

                switch (p.Count()) {
                    case 2:
                        process.Load(
                            p.Named<string>("cfg"),
                            p.Named<Dictionary<string, string>>("parameters")
                        );
                        break;
                    default:
                        process.Load(p.Named<string>("cfg"));
                        break;
                }

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

                if (process.Output().Provider == Constants.DefaultSetting) {
                    try {
                        Console.WindowHeight = Console.WindowHeight + 1 - 1;
                        Console.Title = process.Name;
                        if (!System.Environment.CommandLine.Contains("TESTWINDOW")) {
                            process.Output().Provider = "console";
                        }
                    } catch (Exception) {
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
    }
}
