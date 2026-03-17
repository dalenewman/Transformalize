using Json.Path;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
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
         var json = GetString(row, _input);
         try {
            var node = JsonNode.Parse(json);
            var path = JsonPath.Parse(Context.Operation.Expression);
            var results = path.Evaluate(node);
            var match = results.Matches.FirstOrDefault();
            row[Context.Field] = match == null ? Context.Field.DefaultValue() : (match.Value == null ? Context.Field.DefaultValue() : match.Value.ToString());
         } catch (global::System.Text.Json.JsonException parse) {
            Context.Warn("Could not parse JSON in {0}", _input.Alias);
            Context.Warn(parse.Message);
            row[Context.Field] = Context.Field.DefaultValue();
         } catch (PathParseException ex) {
            Context.Warn(ex.Message);
            row[Context.Field] = Context.Field.DefaultValue();
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("jsonpath") { Parameters = new List<OperationParameter>(1) { new OperationParameter("expression") } };
      }

   }
}
