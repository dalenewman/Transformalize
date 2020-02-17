using System.Collections.Generic;
using System.Web;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Pipeline.Web.Orchard.Impl {
   public class MapPathTransform : StringTransform {

      private readonly Field _input;
      public MapPathTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            Error("The mapPath method expects string input.");
            return;
         }

         _input = SingleInput();
      }

      public override IRow Operate(IRow row) {
         var path = GetString(row, _input);
         row[Context.Field] = HttpContext.Current.Server.MapPath(path);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("mappath");
      }
   }
}