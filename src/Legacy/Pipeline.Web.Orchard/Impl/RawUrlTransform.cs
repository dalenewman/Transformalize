using System.Collections.Generic;
using System.Web;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Pipeline.Web.Orchard.Impl {
   public class RawUrlTransform : StringTransform {

      public RawUrlTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = HttpContext.Current.Request.RawUrl;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("rawurl");
      }
   }
}