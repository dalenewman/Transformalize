using Autofac;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transform.GoogleMaps.Autofac {
    public class GoogleMapsModule : Module {
        private HashSet<string> _methods;
        private ShorthandRoot _shortHand;
        protected override void Load(ContainerBuilder builder) {

            // get methods and shorthand from builder
            _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
            _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

            RegisterTransform(builder, c => new GeocodeTransform(c), new GeocodeTransform().GetSignatures());
            RegisterTransform(builder, c => new PlaceTransform(c), new PlaceTransform().GetSignatures());

        }

        private void RegisterTransform(ContainerBuilder builder, Func<IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {

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

                builder.Register((c, p) => getTransform(p.Positional<IContext>(0))).Named<ITransform>(s.Method);
            }

        }
    }
}
