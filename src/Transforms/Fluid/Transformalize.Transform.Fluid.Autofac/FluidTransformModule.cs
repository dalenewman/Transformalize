using Autofac;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Fluid.Autofac {
   public class FluidTransformModule : Module {

      private HashSet<string> _methods;
      private ShorthandRoot _shortHand;

      protected override void Load(ContainerBuilder builder) {

         // get methods and shorthand from builder
         _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
         _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

         var signatures = new FluidTransform().GetSignatures().ToArray();

         RegisterShortHand(signatures);
         RegisterTransform(builder, c => new FluidTransform(c), signatures);

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

      private static void RegisterTransform(ContainerBuilder builder, Func<IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {
         foreach (var s in signatures) {
            builder.Register((c, p) => getTransform(p.Positional<IContext>(0))).Named<ITransform>(s.Method);
         }
      }

   }
}
