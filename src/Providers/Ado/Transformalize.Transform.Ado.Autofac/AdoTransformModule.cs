using Autofac;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Transforms.Ado.Autofac {
    public class AdoTransformModule : Module {

        private HashSet<string> _methods;
        private ShorthandRoot _shortHand;

        protected override void Load(ContainerBuilder builder) {

            // get methods and shorthand from builder
            _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
            _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

            RegisterShortHand(new AdoRunTransform().GetSignatures());

            if (!builder.Properties.ContainsKey("Process")) {
                return;
            }

            var adoProviders = new HashSet<string>(new[] { "sqlserver", "mysql", "postgresql", "sqlite", "sqlce", "access" }, StringComparer.OrdinalIgnoreCase);
            var adoMethods = new HashSet<string>(new[] { "fromquery", "run" });

            var process = (Process)builder.Properties["Process"];

            // only register by transform key if provider is ado, this allows other providers to implement fromquery and run methods
            foreach (var entity in process.Entities) {
                foreach (var field in entity.GetAllFields().Where(f => f.Transforms.Any())) {
                    foreach (var transform in field.Transforms.Where(t => adoMethods.Contains(t.Method))) {
                        if (transform.Connection == string.Empty) {
                            transform.Connection = field.Connection;
                        }
                        var connection = process.Connections.FirstOrDefault(c => c.Name == transform.Connection);
                        if (connection != null && adoProviders.Contains(connection.Provider)) {
                            if (transform.Method == "fromquery") { // ado from query
                                builder.Register<ITransform>(ctx => {
                                    var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity, field, transform);
                                    return new AdoFromQueryTransform(context, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                                }).Named<ITransform>(transform.Key);
                            } else {  // ado run
                                builder.Register<ITransform>(ctx => {
                                    var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity, field, transform);
                                    return new AdoRunTransform(context, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                                }).Named<ITransform>(transform.Key);
                            }
                        }
                    }
                }
            }
        }

        private void RegisterShortHand(IEnumerable<OperationSignature> signatures) {

            foreach (var s in signatures) {
                if (!_methods.Add(s.Method)) {
                    continue;
                }

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
        }

    }
}
