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
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Validators;
using Module = Autofac.Module;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Ioc.Autofac.Modules {
    public class ValidateModule : Module {

        public const string Name = "shorthand-v";
        private readonly HashSet<string> _methods = new HashSet<string>();
        private readonly ShorthandRoot _shortHand = new ShorthandRoot();
        private readonly Process _process;
        private readonly IPipelineLogger _logger;

        public ValidateModule(Process process, IPipelineLogger logger) {
            _process = process;
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder) {

            var loadContext = new PipelineContext(_logger, _process);

            builder.Properties["ShortHand"] = _shortHand;
            builder.Properties["Methods"] = _methods;
            builder.Properties["Process"] = _process;

            // return true or false, validators

            // new style
            RegisterValidator(builder, (ctx, c) => new AnyValidator(c), new AnyValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new AllValidator(c), new AllValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new StartsWithValidator(c), new StartsWithValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new EndsWithValidator(c), new EndsWithValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new MapValidator(true, c), new MapValidator(inMap:true).GetSignatures());
            RegisterValidator(builder, (ctx, c) => new MapValidator(false, c), new MapValidator(inMap: false).GetSignatures());
            RegisterValidator(builder, (ctx, c) => new ContainsValidator(c), new ContainsValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new IsValidator(c), new IsValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new EqualsValidator(c), new EqualsValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new NotEqualValidator(c), new NotEqualValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new EmptyValidator(c), new EmptyValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new DefaultValidator(c), new DefaultValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new NumericValidator(c), new NumericValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new MatchValidator(c), new MatchValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new RequiredValidator(c), new RequiredValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new LengthValidator(c), new LengthValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new MinLengthValidator(c), new MinLengthValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new MaxLengthValidator(c), new MaxLengthValidator().GetSignatures());
            RegisterValidator(builder, (ctx, c) => new CompareValidator("min",c), new CompareValidator("min").GetSignatures());
            RegisterValidator(builder, (ctx, c) => new CompareValidator("max", c), new CompareValidator("max").GetSignatures());
            RegisterValidator(builder, (ctx, c) => new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric", c), new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric").GetSignatures());

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {

                var assemblies = new List<Assembly>();

                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Validate.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
                    var info = new FileInfo(file);
                    var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "validate" && f != "autofac");
                    loadContext.Debug(() => $"Loading {name} validator(s)");
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }

                if (assemblies.Any()) {
                    builder.RegisterAssemblyModules(assemblies.ToArray());
                }
            }

            // register the short hand
            builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(Name).InstancePerLifetimeScope();
            builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(Name), new[] { "fields", "calculated-fields" }, "v", "validators", "method")).Named<IDependency>(Name).InstancePerLifetimeScope();

        }

        private void RegisterValidator(ContainerBuilder builder, Func<IComponentContext, IContext, IValidate> getValidator, IEnumerable<OperationSignature> signatures) {

            foreach (var s in signatures) {
                if (_methods.Add(s.Method)) {

                    var method = new Method { Name = s.Method, Signature = s.Method, Ignore = s.Ignore };
                    _shortHand.Methods.Add(method);

                    var signature = new Signature {
                        Name = s.Method,
                        NamedParameterIndicator = s.NamedParameterIndicator
                    };

                    foreach (var parameter in s.Parameters) {
                        signature.Parameters.Add(new Parameter {
                            Name = parameter.Name,
                            Value = parameter.Value
                        });
                    }
                    _shortHand.Signatures.Add(signature);
                }

                builder.Register((ctx, p) => getValidator(ctx, p.Positional<IContext>(0))).Named<IValidate>(s.Method);
            }

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