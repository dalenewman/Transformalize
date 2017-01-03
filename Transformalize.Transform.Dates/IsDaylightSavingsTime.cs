using System;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Dates {

    public class IsDaylightSavings : BaseTransform {

        private readonly IContext _context;
        private readonly Field _input;

        public IsDaylightSavings(IContext context) : base(context, "bool") {
            _context = context;
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = ((DateTime)row[_input]).IsDaylightSavingTime();
            Increment();
            return row;
        }
    }
}
