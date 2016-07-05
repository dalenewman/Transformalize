using System;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class EqualsValidator : BaseTransform, ITransform {
        private readonly bool _sameTypes;
        private readonly Field _first;
        private readonly Field[] _rest;
        private readonly object _value;
        private readonly Action<IRow> _validator;

        public EqualsValidator(IContext context) : base(context) {
            var input = MultipleInput();
            _first = input.First();

            if (context.Transform.Value == Constants.DefaultSetting) {
                _rest = input.Skip(1).ToArray();
                _sameTypes = _rest.All(f => f.Type == _first.Type);
            } else {
                _value = _first.Convert(context.Transform.Value);
                _rest = input.ToArray();
                _sameTypes = input.All(f => f.Type == _first.Type);
            }

            if (_sameTypes) {
                if (_value == null) {
                    _validator = row => row[Context.Field] = _rest.All(f => row[f].Equals(row[_first]));
                } else {
                    _validator = row => row[Context.Field] = _rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => row[Context.Field] = false;
            }
        }

        public IRow Transform(IRow row) {
            _validator(row);
            Increment();
            return row;
        }
    }
}