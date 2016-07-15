using System;
using System.Collections.Generic;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class InValidator : BaseTransform, ITransform {
        private readonly Field _input;
        private readonly HashSet<object> _set = new HashSet<object>();

        public InValidator(IContext context) : base(context) {
            _input = SingleInput();
            var items = Utility.Split(Context.Transform.Domain, ',');
            foreach (var item in items) {
                try {
                    _set.Add(_input.Convert(item));
                } catch (Exception ex) {
                    context.Warn($"In transform can't convert {item} to {_input.Type} {ex.Message}.");
                }
            }
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = _set.Contains(row[_input]);
            Increment();
            return row;
        }
    }
}