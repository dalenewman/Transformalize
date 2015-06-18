using System.Collections.Generic;
using System.Text;

namespace Transformalize.Main {

    public class Filters : List<Filter> {

        public string ResolveExpression(string textQualifier) {
            var builder = new StringBuilder("(");
            var last = Count - 1;

            for (var i = 0; i < this.Count; i++) {
                var filter = this[i];
                builder.Append(filter.ResolveExpression(textQualifier));
                if (i >= last) continue;
                builder.Append(" ");
                builder.Append(filter.Continuation);
                builder.Append(" ");
            }

            builder.Append(")");
            return builder.ToString();
        }
    }
}