using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Cfg.Net.Shorthand;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms.Humanizer.Actions;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Transforms.Humanizer.Autofac {
   public class HumanizeModule : Module {

      private HashSet<string> _methods;
      private ShorthandRoot _shortHand;
      protected override void Load(ContainerBuilder builder) {

         // get methods and shorthand from builder
         _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
         _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

         // Humanizer Transforms
         RegisterTransform(builder, c => new CamelizeTransform(c), new CamelizeTransform().GetSignatures());
         RegisterTransform(builder, c => new HumanizeTransform(c), new HumanizeTransform().GetSignatures());
         RegisterTransform(builder, c => new FromMetricTransform(c), new FromMetricTransform().GetSignatures());
         RegisterTransform(builder, c => new FromRomanTransform(c), new FromRomanTransform().GetSignatures());
         RegisterTransform(builder, c => new DehumanizeTransform(c), new DehumanizeTransform().GetSignatures());
         RegisterTransform(builder, c => new HyphenateTransform(c), new HyphenateTransform().GetSignatures());
         RegisterTransform(builder, c => new OrdinalizeTransform(c), new OrdinalizeTransform().GetSignatures());
         RegisterTransform(builder, c => new PascalizeTransform(c), new PascalizeTransform().GetSignatures());
         RegisterTransform(builder, c => new PluralizeTransform(c), new PluralizeTransform().GetSignatures());
         RegisterTransform(builder, c => new SingularizeTransform(c), new SingularizeTransform().GetSignatures());
         RegisterTransform(builder, c => new TitleizeTransform(c), new TitleizeTransform().GetSignatures());
         RegisterTransform(builder, c => new ToMetricTransform(c), new ToMetricTransform().GetSignatures());
         RegisterTransform(builder, c => new ToOrdinalWordsTransform(c), new ToOrdinalWordsTransform().GetSignatures());
         RegisterTransform(builder, c => new ToRomanTransform(c), new ToRomanTransform().GetSignatures());
         RegisterTransform(builder, c => new ToWordsTransform(c), new ToWordsTransform().GetSignatures());
         RegisterTransform(builder, c => new UnderscoreTransform(c), new UnderscoreTransform().GetSignatures());

         RegisterTransform(builder, c => new ByteSizeTransform(c), new ByteSizeTransform().GetSignatures());

         // these all return a bytesize that must be handled by a subsequent call to humanize()
         RegisterTransform(builder, c => new BitsTransform(c), new BitsTransform().GetSignatures());
         RegisterTransform(builder, c => new BytesTransform(c), new BytesTransform().GetSignatures());
         RegisterTransform(builder, c => new KilobytesTransform(c), new KilobytesTransform().GetSignatures());
         RegisterTransform(builder, c => new MegabytesTransform(c), new MegabytesTransform().GetSignatures());
         RegisterTransform(builder, c => new GigabytesTransform(c), new GigabytesTransform().GetSignatures());
         RegisterTransform(builder, c => new TerabytesTransform(c), new TerabytesTransform().GetSignatures());

         if (builder.Properties.ContainsKey("Process")) {
            var process = (Process)builder.Properties["Process"];
            foreach (var action in process.Actions.Where(a => a.Type == "humanize-labels")) {
               builder.Register<IAction>((c, p) => new HumanizeLabels(c.Resolve<IContext>(), action)).Named<IAction>(action.Key);
            }
         }

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
