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

using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Validate.Web;
using Transformalize.Validators;
using Transformalize.Validators.Jint;
using CompareValidator = Transformalize.Validators.CompareValidator;
using Parameter = Cfg.Net.Shorthand.Parameter;
using RegularExpressionValidator = Transformalize.Validators.RegularExpressionValidator;

namespace Pipeline.Web.Orchard.Modules {

   public class ValidateShorthandCustomizer : ShorthandCustomizer {
      public ValidateShorthandCustomizer(ShorthandRoot root, IEnumerable<string> shortHandCollections, string shortHandProperty, string longHandCollection, string longHandProperty) : base(root, shortHandCollections, shortHandProperty, longHandCollection, longHandProperty) { }
   }

   public class ValidateModule : Module {

      private readonly HashSet<string> _methods = new HashSet<string>();
      private readonly ShorthandRoot _shortHand = new ShorthandRoot();

      protected override void Load(ContainerBuilder builder) {

         // This reader is used to load the initial configuration and nested resources for tfl actions, etc.
         builder.Register(c => new DefaultReader(new FileReader(), new WebReader())).As<IReader>();

         // new style
         RegisterValidator(builder, (ctx, c) => new AnyValidator(c), new AnyValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new AllValidator(c), new AllValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new StartsWithValidator(c), new StartsWithValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new EndsWithValidator(c), new EndsWithValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new MapValidator(c), new MapValidator().GetSignatures());
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
         RegisterValidator(builder, (ctx, c) => new CompareValidator("min", c), new CompareValidator("min").GetSignatures());
         RegisterValidator(builder, (ctx, c) => new CompareValidator("max", c), new CompareValidator("max").GetSignatures());
         RegisterValidator(builder, (ctx, c) => new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric", c), new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric").GetSignatures());
         RegisterValidator(builder, (ctx, c) => new RegularExpressionValidator("digits", "^[0-9]*$", "must be digits", c), new RegularExpressionValidator("digits", "^[0-9]*$", "must be digits").GetSignatures());
         RegisterValidator(builder, (ctx, c) => new JintValidator(ctx.Resolve<IReader>(), c), new JintValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new InvertValidator(c), new InvertValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new EmailValidator(c), new EmailValidator().GetSignatures());
         RegisterValidator(builder, (ctx, c) => new UrlValidator(c), new UrlValidator().GetSignatures());


         // register the short hand
         builder.Register((c, p) => new ValidateShorthandCustomizer(_shortHand, new[] { "fields", "calculated-fields" }, "v", "validators", "method")).As<ValidateShorthandCustomizer>().SingleInstance();

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

   }
}