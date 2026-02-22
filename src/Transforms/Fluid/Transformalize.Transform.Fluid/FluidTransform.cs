using Fluid;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Fluid {
   public class FluidTransform : BaseTransform {

      private readonly Func<string, object> _convert;
      private static readonly FluidParser _parser = new FluidParser();
      private Field[] _input;

      public FluidTransform(IContext context = null) : base(context, null) {
         if (IsMissingContext()) {
            return;
         }

         Returns = Context.Field.Type;

         if (IsMissing(Context.Operation.Template)) {
            Run = false;
            return;
         }

         if (Returns == "string") {
            _convert = o => (o.Trim('\n', '\r'));
         } else {
            _convert = o => Context.Field.Convert(o.Trim(' ', '\n', '\r'));
         }
      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException("Not implemented here so it can wait for file based templates to load.");
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         if (!Run)
            yield break;

         var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Template);

         if (fileBasedTemplate != null) {
            Context.Operation.Template = fileBasedTemplate.Content;
         }

         var input = MultipleInput();
         var matches = Context.Entity.GetFieldMatches(Context.Operation.Template);
         _input = input.Union(matches).ToArray();

         if (_parser.TryParse(Context.Operation.Template, out var template)) {
            var context = new TemplateContext();
            foreach(var row in rows) {
               foreach (var field in _input) {
                  context.SetValue(field.Alias, row[field]);
               }
               row[Context.Field] = _convert(template.Render(context));
               yield return row;
            }
         } else {
            Context.Error("Failed to parse fluid template.");
            Utility.CodeToError(Context, Context.Operation.Template);
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("fluid") {
            Parameters = new List<OperationParameter>(1) {
               new OperationParameter("template")
            }
         };
      }
   }
}
