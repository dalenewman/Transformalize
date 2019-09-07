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

using Autofac;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Validators;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Tests.TestContainer {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in validators
   /// </summary>
   public class ValidateBuilder {

      private readonly List<ValidatorHolder> _validators;

      public const string Name = "shorthand-v";
      private readonly HashSet<string> _methods;
      private readonly ShorthandRoot _shortHand;
      private readonly Process _process;
      private readonly IPipelineLogger _logger;
      private readonly ContainerBuilder _builder;

      public ValidateBuilder(Process process, ContainerBuilder builder, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = new HashSet<string>();
         _shortHand = new ShorthandRoot();
         _builder = builder;
         _validators = new List<ValidatorHolder>();
      }

      public ValidateBuilder(Process process, ContainerBuilder builder, HashSet<string> methods, ShorthandRoot shortHand, List<ValidatorHolder> validators, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = methods;
         _shortHand = shortHand;
         _builder = builder;
         _validators = validators;
      }

      public void Build() {

         var loadContext = new PipelineContext(_logger, _process);

         // return true or false, validators

         // new style
         RegisterValidator(_builder, (ctx, c) => new AnyValidator(c), new AnyValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new AllValidator(c), new AllValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new StartsWithValidator(c), new StartsWithValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new EndsWithValidator(c), new EndsWithValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new MapValidator(true, c), new MapValidator(inMap: true).GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new MapValidator(false, c), new MapValidator(inMap: false).GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new ContainsValidator(c), new ContainsValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new IsValidator(c), new IsValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new EqualsValidator(c), new EqualsValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new NotEqualValidator(c), new NotEqualValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new EmptyValidator(c), new EmptyValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new DefaultValidator(c), new DefaultValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new NumericValidator(c), new NumericValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new MatchValidator(c), new MatchValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new RequiredValidator(c), new RequiredValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new LengthValidator(c), new LengthValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new MinLengthValidator(c), new MinLengthValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new MaxLengthValidator(c), new MaxLengthValidator().GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new CompareValidator("min", c), new CompareValidator("min").GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new CompareValidator("max", c), new CompareValidator("max").GetSignatures());
         RegisterValidator(_builder, (ctx, c) => new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric", c), new RegularExpressionValidator("alphanum", "^[a-zA-Z0-9]*$", "must be alphanumeric").GetSignatures());

         foreach (var v in _validators) {
            RegisterValidator(_builder, (ctx, c) => v.GetValidator(c), v.Signatures);
         }

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

      public void AddValidator(ValidatorHolder v) {
         _validators.Add(v);
      }

   }
}