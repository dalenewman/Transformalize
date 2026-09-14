using Fluid;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

using System.Threading.Tasks;

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

      public override async global::System.Collections.Generic.IAsyncEnumerable<IRow> OperateStreamAsync(
         global::System.Collections.Generic.IAsyncEnumerable<IRow> rows,
         [global::System.Runtime.CompilerServices.EnumeratorCancellation] global::System.Threading.CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         // A further-derived sequence override still needs the conservative adapter.
         if (GetType().GetMethod(nameof(Operate), new[] { typeof(IEnumerable<IRow>) }).DeclaringType != typeof(FluidTransform)) {
            await foreach (var row in base.OperateStreamAsync(rows, token).ConfigureAwait(false)) yield return row;
            yield break;
         }
         if (!Run)
            yield break;

         var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Template);

         if (fileBasedTemplate != null) {
            Context.Operation.Template = fileBasedTemplate.Content;
         }

         var input = MultipleInput();
         var matches = Context.Entity.GetFieldMatches(Context.Operation.Template);
         _input = input.Union(matches).ToArray();

         if (!_parser.TryParse(Context.Operation.Template, out var template)) {
            Context.Error("Failed to parse fluid template.");
            Utility.CodeToError(Context, Context.Operation.Template);
            yield break;
         }

         var templateContext = new TemplateContext();
         await foreach (var row in rows.WithCancellation(token).ConfigureAwait(false)) {
            token.ThrowIfCancellationRequested();
            foreach (var field in _input) {
               templateContext.SetValue(field.Alias, row[field]);
            }
            row[Context.Field] = _convert(template.Render(templateContext));
            yield return row;
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
