using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.LamdaParser {
    public class LamdaParserEvalTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly List<Field> _input;
        private readonly Dictionary<string, object> _typeDefaults = Constants.TypeDefaults();

        public LamdaParserEvalTransform(IContext context) : base(context, "object")
        {

            try {
                _input = new List<Field>(MultipleInput());

                var lambdaParser = new NReco.Linq.LambdaParser { UseCache = true };
                var exp = lambdaParser.Parse(context.Transform.Expression);

                var matches = context.Entity.FieldMatcher.Matches(exp.ToString());
                foreach (Match match in matches) {
                    Field field;
                    if (context.Entity.TryGetField(match.Value, out field) && _input.All(f => f.Alias != field.Alias)) {
                        _input.Add(field);
                    }
                }

                while (exp.CanReduce) {
                    exp = exp.Reduce();
                    context.Debug(() => $"The expression {context.Transform.Expression} can be reduced to {exp}");
                }
                _transform = row => context.Field.Convert(lambdaParser.Eval(context.Transform.Expression, _input.ToDictionary(k => k.Alias, v => row[v])));
            } catch (NReco.Linq.LambdaParserException ex) {
                context.Error($"The expression {context.Transform.Expression} in field {context.Field.Alias} can not be parsed. {ex.Message}");
                _transform = row => _typeDefaults[context.Field.Type];
            }

        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}
