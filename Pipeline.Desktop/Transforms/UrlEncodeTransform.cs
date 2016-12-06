using System;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Desktop.Transforms {
    public class UrlEncodeTransform : BaseTransform {
        private readonly Field _input;

        public UrlEncodeTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = Uri.EscapeDataString((string)row[_input]);
            Increment();
            return row;
        }
    }
}
