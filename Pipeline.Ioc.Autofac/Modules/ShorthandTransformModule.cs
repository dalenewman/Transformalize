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

                root.Signatures.Add(new Signature { Name = "none" });
                root.Signatures.Add(Simple("separator", ","));
                root.Signatures.Add(new Signature {
                    Name = "separator-space",
                    Parameters = new List<Cfg.Net.Shorthand.Parameter> { new Cfg.Net.Shorthand.Parameter { Name = "separator", Value = " " } }
                });
                root.Signatures.Add(Simple("value", "[default]"));
                root.Signatures.Add(Simple("type", "[default]"));
                root.Signatures.Add(Simple("script"));
                root.Signatures.Add(Simple("pattern"));

                root.Signatures.Add(new Signature {
                    Name = "any",
                    Parameters = new List<Cfg.Net.Shorthand.Parameter> {
                            new Cfg.Net.Shorthand.Parameter { Name = "value" },
                            new Cfg.Net.Shorthand.Parameter { Name = "operator", Value="equals" }
                        }
                });

                root.Signatures.Add(Simple("domain"));
                root.Signatures.Add(new Signature {
                    Name = "web",
                    Parameters = new List<Cfg.Net.Shorthand.Parameter> {
                            new Cfg.Net.Shorthand.Parameter { Name = "url", Value="" },
                            new Cfg.Net.Shorthand.Parameter { Name = "web-method", Value="GET" },
                            new Cfg.Net.Shorthand.Parameter { Name = "body", Value="" }
                        }
                });
                root.Signatures.Add(Simple("expression"));
                root.Signatures.Add(Simple("key"));
                root.Signatures.Add(Simple("units"));

                root.Methods.Add(new Method { Name = "any", Signature = "any" });

                root.Methods.Add(new Method { Name = "contains", Signature = "value" });

                root.Methods.Add(new Method { Name = "is", Signature = "type" });
                root.Methods.Add(new Method { Name = "javascript", Signature = "script" });

                root.Methods.Add(new Method { Name = "js", Signature = "script" });

                root.Methods.Add(new Method { Name = "in", Signature = "domain" });
                root.Methods.Add(new Method { Name = "startswith", Signature = "value" });
                root.Methods.Add(new Method { Name = "endswith", Signature = "value" });
                root.Methods.Add(new Method { Name = "isdefault", Signature = "none" });
                root.Methods.Add(new Method { Name = "isempty", Signature = "none" });
                root.Methods.Add(new Method { Name = "isnumeric", Signature = "none" });

                root.Methods.Add(new Method { Name = "web", Signature = "web" });
                root.Methods.Add(new Method { Name = "urlencode", Signature = "none" });
                root.Methods.Add(new Method { Name = "datemath", Signature = "expression" });

                root.Methods.Add(new Method { Name = "isdaylightsavings", Signature = "none" });
                root.Methods.Add(new Method { Name = "fromaddress", Signature = "key" });
                root.Methods.Add(new Method { Name = "distinct", Signature = "separator-space" });
                root.Methods.Add(new Method { Name = "ismatch", Signature = "pattern" });
                root.Methods.Add(new Method { Name = "matchcount", Signature = "pattern" });
                root.Check();


                if (root.Errors().Any()) {
                    var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(ctx.IsRegistered<IPipelineLogger>() ? ctx.Resolve<IPipelineLogger>() : new TraceLogger(), new Process { Name = "Error" });
                    foreach (var error in root.Errors()) {
                        context.Error(error);
                    }
                    context.Error("Please fix you shorthand configuration.  No short-hand is being processed for the t attribute.");
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
