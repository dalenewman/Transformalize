using Transformalize.Contracts;
using Transformalize.Transforms;
using System.Web;
using Transformalize.Configuration;

namespace Transformalize.Transform.Html {

    public class HtmlEncodeTransform : BaseTransform {
        private readonly Field _input;

        public HtmlEncodeTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = HttpUtility.HtmlEncode(row[_input]);
            Increment();
            return row;
        }
    }
}
