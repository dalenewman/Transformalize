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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Validators;

namespace Transformalize.Ioc.Autofac {
    public static class ValidateFactory {

        public static IEnumerable<IValidate> GetValidators(IComponentContext ctx, IContext context, IEnumerable<Field> fields) {
            var validators = new List<IValidate>();

            foreach (var f in fields.Where(f => f.Validators.Any())) {
                var field = f;

                foreach (var v in field.Validators) {
                    var validatorContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), context.Process, context.Entity, field, v);
                    if (TryValidator(ctx, validatorContext, out var add)) {
                        validators.Add(add);
                    }
                }
            }

            return validators;
        }

        public static bool TryValidator(IComponentContext ctx, IContext context, out IValidate validator) {
            validator = null;
            var result = true;

            if (ctx.IsRegisteredWithName<IValidate>(context.Operation.Method)) {
                var v = ShouldRunValidator(ctx, context);

                foreach (var warning in v.Warnings()) {
                    context.Warn(warning);
                }

                if (v.Errors().Any()) {
                    foreach (var error in v.Errors()) {
                        context.Error(error);
                    }
                    result = false;
                } else {
                    validator = v;
                }
            } else {
                context.Error($"The {context.Operation.Method} method used in the {context.Field.Alias} field is not registered.");
                result = false;
            }
            return result;
        }

        public static IValidate ShouldRunValidator(IComponentContext ctx, IContext context) {
            return context.Operation.ShouldRun == null ?
                ctx.ResolveNamed<IValidate>(context.Operation.Method, new PositionalParameter(0, context)) :
                new ShouldRunValidator(context, ctx.ResolveNamed<IValidate>(context.Operation.Method, new PositionalParameter(0, context)));
        }

    }
}