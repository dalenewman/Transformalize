using System.Collections.Generic;
using System.Web;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Pipeline.Web.Orchard.Impl {
   public class ResolveUrlTransform : StringTransform {

      private readonly Field _input;
      public ResolveUrlTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            Error("The resolveUrl method expects string input.");
            return;
         }

         _input = SingleInput();
      }

      public override IRow Operate(IRow row) {
         var url = GetString(row, _input);
         row[Context.Field] = url.StartsWith("http",System.StringComparison.OrdinalIgnoreCase) ? url : VirtualPathUtility.ToAbsolute(url);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("resolveurl");
      }
   }
}