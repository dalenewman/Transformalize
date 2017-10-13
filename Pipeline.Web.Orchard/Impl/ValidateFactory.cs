using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Validators;

namespace Pipeline.Web.Orchard.Impl {

    public static class ValidateFactory {

        public static IEnumerable<IValidate> GetValidators(IComponentContext ctx, IContext context, IEnumerable<Field> fields) {
            var validators = new List<IValidate>();

            foreach (var f in fields.Where(f => f.Validators.Any())) {
                var field = f;

                foreach (var v in field.Validators) {
                    var validatorContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), context.Process, context.Entity, field, v);
                    IValidate add;
                    if (TryValidator(ctx, validatorContext, out add)) {
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
                context.Error(string.Format("The {0} method used in the {1} field is not registered.", context.Operation.Method, context.Field.Alias));
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