using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Json {
   public class JsonPathTransform : StringTransform {

      private readonly IField _input;

      public JsonPathTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            return;
         }

         if (IsMissing(Context.Operation.Expression)) {
            return;
         }

         _input = SingleInput();
      }

      public override IRow Operate(IRow row) {
         JObject o;
         var json = GetString(row, _input);
         try {
            o = JObject.Parse(json);
            try {
               var result = o.SelectToken(Context.Operation.Expression);
               row[Context.Field] = result == null ? Context.Field.DefaultValue() : result.ToString();
            } catch (JsonException find) {
               if (find.Message == "Path returned multiple tokens.") {
                  row[Context.Field] = o.SelectTokens(Context.Operation.Expression).First().ToString();
               } else {
                  Context.Warn(find.Message);
                  Context.Warn("No result with JSON expression: {0}", Context.Operation.Expression);
                  Context.Debug(() => json);
                  Context.Debug(() => find.StackTrace);
                  row[Context.Field] = Context.Field.DefaultValue();
               }
            }
         } catch (JsonReaderException parse) {
            Context.Warn("Could not parse JSON in {0}", _input.Alias);
            Context.Warn(parse.Message);
            Context.Debug(() => json);
            Context.Debug(() => parse.StackTrace);
            row[Context.Field] = Context.Field.DefaultValue();
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("jsonpath") { Parameters = new List<OperationParameter>(1) { new OperationParameter("expression") } };
      }

   }
}
