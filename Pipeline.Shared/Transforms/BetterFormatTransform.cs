using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class BetterFormatTransform : BaseTransform {
        readonly Field[] _input;
        private const string Pattern = "(?<={)[^}]+(?=})";
        private readonly Func<IRow, string> _transform;

        public BetterFormatTransform(IContext context) : base(context, "string") {

            _transform = row => context.Transform.Format;
            Regex regex = null;
#if NETS10
            regex = new Regex(Pattern);
#else
            regex = new Regex(Pattern, RegexOptions.Compiled);
#endif
            var matches = regex.Matches(context.Transform.Format);

            if (matches.Count == 0) {
                Error($"a format transform in {Context.Field.Alias} is missing place-holders.");
                return;
            }

            var values = new List<string>(); // using list to maintain insertion order (HashSet<string> does not)
            foreach (Match match in matches) {
                if (!values.Contains(match.Value)) {
                    values.Add(match.Value);
                }
            }

            var numeric = true;
            var names = new List<string>();

            foreach (var value in values) {
                var left = value.Split(':')[0];
                if (left.ToCharArray().All(c => c >= '0' && c <= '9'))
                    continue;
                if (!names.Contains(left)) {
                    names.Add(left);
                }
                numeric = false;
            }

            if (numeric) {
                _input = MultipleInput();  // receiving fields from parameters (or copu(x,y,etc))
            } else {
                var fields = new List<Field>();
                var count = 0;
                foreach (var name in names) {
                    Field field;
                    if (context.Process.TryGetField(name, out field)) {
                        fields.Add(field);
                        context.Transform.Format = context.Transform.Format.Replace("{" + name, "{" + count);
                        count++;
                    } else {
                        Error($"Invalid field name {name} found in a format transform in field {Context.Field.Alias}.");
                        return;
                    }
                }
                _input = fields.ToArray();
            }


            if (values.Count != _input.Length) {
                Error($"The number of supplied fields does not match the number of place-holders in a format transform in field {Context.Field.Alias}.");
                return;
            }

            _transform = row => string.Format(Context.Transform.Format, _input.Select(f => row[f]).ToArray());
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }

    }
}