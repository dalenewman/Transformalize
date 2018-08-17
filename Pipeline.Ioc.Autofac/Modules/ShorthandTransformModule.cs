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
using System.Collections.Generic;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using System.Linq;
using Transformalize.Contracts;
using Transformalize.Context;
using Transformalize.Configuration;
using Transformalize.Nulls;
using Transformalize.Providers.Trace;

namespace Transformalize.Ioc.Autofac.Modules {

    public class ShorthandTransformModule : Module {

        public const string Name = "shorthand-t";

        protected override void Load(ContainerBuilder builder) {

            builder.Register<IDependency>((ctx, p) => {
                ShorthandRoot root;
                if (ctx.IsRegisteredWithName<ShorthandRoot>(Name)) {
                    root = ctx.ResolveNamed<ShorthandRoot>(Name);
                } else {
                    root = new ShorthandRoot();
                    builder.Register(c => root).Named<ShorthandRoot>(Name).InstancePerLifetimeScope();
                }

                root.Check();


                if (root.Errors().Any()) {
                    var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(ctx.IsRegistered<IPipelineLogger>() ? ctx.Resolve<IPipelineLogger>() : new TraceLogger(), new Process { Name = "Error" });
                    foreach (var error in root.Errors()) {
                        context.Error(error);
                    }
                    context.Error("Please fix your shorthand configuration.  No short-hand is being processed for the t attribute.");
                } else {
                    return new ShorthandCustomizer(root, new[] { "fields", "calculated-fields" }, "t", "transforms", "method");
                }

                return new NullCustomizer();
            }).Named<IDependency>(Name).InstancePerLifetimeScope();
        }

        private static Signature Simple(string name, string value = null) {
            if (value == null) {
                return new Signature {
                    Name = name,
                    Parameters = new List<Cfg.Net.Shorthand.Parameter> {
                            new Cfg.Net.Shorthand.Parameter { Name = name }
                        }
                };
            }
            return new Signature {
                Name = name,
                Parameters = new List<Cfg.Net.Shorthand.Parameter> {
                    new Cfg.Net.Shorthand.Parameter { Name = name, Value = value }
                }
            };
        }

    }
}
