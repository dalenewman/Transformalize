using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class CoalesceTransform : BaseTransform, ITransform {
        class FieldWithDefault {
            public Field Field { get; set; }
            public object Default { get; set; }
        }

        private readonly FieldWithDefault[] _input;

        public CoalesceTransform(IContext context) : base(context) {
            _input = MultipleInput().Select(f => new FieldWithDefault { Field = f, Default = f.Convert(f.Default) }).ToArray();

        }

        public IRow Transform(IRow row) {
            var first = _input.FirstOrDefault(f => !row[f.Field].Equals(f.Default));
            if (first != null) {
                row[Context.Field] = Context.Field.Type == first.Field.Type ? row[first.Field] : Context.Field.Convert(row[first.Field]);
            }
            Increment();
            return row;
        }
    }
}