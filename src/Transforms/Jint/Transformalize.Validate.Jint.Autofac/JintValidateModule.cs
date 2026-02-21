using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using Transformalize.Contracts;
using Transformalize.Validators.Jint;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Validate.Jint.Autofac {
    public class JintValidateModule : Module {

        private HashSet<string> _methods;
        private ShorthandRoot _shortHand;

        protected override void Load(ContainerBuilder builder) {

            var signatures = new JintValidator().GetSignatures().ToArray();

            // get methods and shorthand from builder
            _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
            _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

            RegisterShortHand(signatures);
            RegisterValidator(builder, c=> new JintValidator(new DefaultReader(new FileReader(), new WebReader()), c), signatures);
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

        private static void RegisterValidator(ContainerBuilder builder, Func<IContext, IValidate> getValidator, IEnumerable<OperationSignature> signatures) {
            foreach (var s in signatures) {
                builder.Register((c, p) => getValidator(p.Positional<IContext>(0))).Named<IValidate>(s.Method);
            }
        }
    }
}
