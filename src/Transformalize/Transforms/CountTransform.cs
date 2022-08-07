using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class CountTransform : StringTransform {
      private readonly Field _input;
      private readonly Func<IRow, int> _transform;

      public CountTransform(IContext context = null) : base(context, "int") {
         if (IsMissingContext()) {
            return;
         }

         _input = SingleInput();

         if (IsMissing(Context.Operation.Value)) {
            Context.Warn("The count transform expects value to count.  It expects something like count(foo) or count(bar).");
            Run = false;
            return;
         }

         if (Received() != "string") {
            Context.Error($"The count transform expects a string as input.  It will not run against {Received()}.");
            Run = false;
            return;
         }

         _transform = (row) => CountStringOccurrences(GetString(row, _input), Context.Operation.Value);

      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = _transform(row);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("count") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
      }

      // https://www.dotnetperls.com/string-occurrence
      public static int CountStringOccurrences(string text, string pattern) {
         var count = 0;
         var i = 0;
         while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1) {
            i += pattern.Length;
            count++;
         }
         return count;
      }
   }
}