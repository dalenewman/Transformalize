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
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Ioc.Autofac.Modules {

    public class ShorthandValidateModule : Module {

        public const string Name = "shorthand-v";

        protected override void Load(ContainerBuilder builder) {

            builder.Register<IDependency>((ctx, p) => {
                ShorthandRoot root;
                if (ctx.IsRegisteredWithName<string>(Name)) {
                    root = new ShorthandRoot(ctx.ResolveNamed<string>(Name), ctx.ResolveNamed<IReader>("file"));
                } else {
                    root = new ShorthandRoot();
                    root.Signatures.Add(new Signature { Name = "none" });
                    root.Signatures.Add(Simple("length"));
                    root.Signatures.Add(Simple("separator", ","));
                    root.Signatures.Add(Simple("value", "[default]"));
                    root.Signatures.Add(Simple("type", "[default]"));
                    root.Signatures.Add(Simple("pattern"));
                    root.Signatures.Add(new Signature {
                        Name = "any",
                        Parameters = new List<Parameter> {
                    new Parameter {Name = "value"},
                    new Parameter {Name = "operator", Value = "equal"}
                }
                    });
                    root.Signatures.Add(new Signature {
                        Name = "range",
                        Parameters = new List<Parameter> {
                    new Parameter {Name = "min"},
                    new Parameter {Name = "max"}
                }
                    });
                    root.Signatures.Add(Simple("domain"));

                    root.Methods.Add(new Method { Name = "required", Signature = "none" });
                    root.Methods.Add(new Method { Name = "maxlength", Signature = "length" });
                    root.Methods.Add(new Method { Name = "minlength", Signature = "length" });
                    root.Methods.Add(new Method { Name = "range", Signature = "range" });
                    root.Methods.Add(new Method { Name = "max", Signature = "value" });
                    root.Methods.Add(new Method { Name = "min", Signature = "value" });
                    root.Methods.Add(new Method { Name = "any", Signature = "any" });
                    root.Methods.Add(new Method { Name = "contains", Signature = "value" });
                    root.Methods.Add(new Method { Name = "equals", Signature = "value" });
                    root.Methods.Add(new Method { Name = "is", Signature = "type" });
                    root.Methods.Add(new Method { Name = "in", Signature = "domain" });
                    root.Methods.Add(new Method { Name = "matches", Signature = "pattern" });
                    root.Methods.Add(new Method { Name = "startswith", Signature = "value" });
                    root.Methods.Add(new Method { Name = "endswith", Signature = "value" });
                    root.Methods.Add(new Method { Name = "isdefault", Signature = "none" });
                    root.Methods.Add(new Method { Name = "isempty", Signature = "none" });
                    root.Methods.Add(new Method { Name = "isnumeric", Signature = "none" });
                    root.Methods.Add(new Method { Name = "isdaylightsavings", Signature = "none" });
                    root.Check();
                }

                if (root.Errors().Any()) {
                    var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(ctx.IsRegistered<IPipelineLogger>() ? ctx.Resolve<IPipelineLogger>() : new TraceLogger(), new Process { Name = "Error" });
                    foreach (var error in root.Errors()) {
                        context.Error(error);
                    }
                    context.Error("Please fix you shorthand configuration.  No short-hand is being processed for the v attribute.");
                } else {
                    return new ShorthandCustomizer(root, new[] { "fields", "calculated-fields" }, "v", "validators", "method");
                }

                return new NullCustomizer();
            }).Named<IDependency>(Name);
        }

        private static Signature Simple(string name, string value = null) {
            if (value == null) {
                return new Signature {
                    Name = name,
                    Parameters = new List<Parameter> {
                            new Parameter { Name = name }
                        }
                };
            }
            return new Signature {
                Name = name,
                Parameters = new List<Parameter> {
                    new Parameter { Name = name, Value = value }
                }
            };
        }

    }
}
