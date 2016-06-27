using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class EqualsValidator : BaseTransform, ITransform {
        private readonly bool _sameTypes;
        private readonly Field _first;
        private readonly Field[] _rest;

        public EqualsValidator(IContext context) : base(context) {
            var input = MultipleInput();
            _first = input.First();
            _rest = input.Skip(1).ToArray();
            _sameTypes = _rest.All(f => f.Type == _first.Type);
        }

        public IRow Transform(IRow row) {
            if (_sameTypes) {
                row[Context.Field] = _rest.All(f => row[f].Equals(row[_first]));
            } else {
                row[Context.Field] = false;
            }
            Increment();
            return row;
        }
    }
}