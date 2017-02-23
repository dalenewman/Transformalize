using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class DistinctTransform : BaseTransform {
        private readonly Field _input;
        private readonly char[] _sep;

        public DistinctTransform(IContext context) : base(context, "string") {
            _input = SingleInput();

            // check input type
            var typeReceived = Received();
            if (typeReceived != "string") {
                Error($"The distinct transform takes a string input.  You have a {typeReceived} input.");
            }

            // check separator
            if (context.Transform.Separator == Constants.DefaultSetting) {
                context.Transform.Separator = " ";
            }

            _sep = context.Transform.Separator.ToCharArray();

        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = string.Join(Context.Transform.Separator, ((string)row[_input]).Split(_sep, StringSplitOptions.RemoveEmptyEntries).Distinct());
            Increment();
            return row;
        }
    }
}