using System.Linq;
using Jint;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Scripting.Jint {

    public class JintRowCondition : IRowCondition {
        private readonly string _expression;
        private readonly Field[] _input;
        readonly Engine _jint = new Engine();
        public JintRowCondition(IContext context, string expression) {
            _expression = expression;
            _input = new global::Jint.Parser.JavaScriptParser().Parse(expression, new global::Jint.Parser.ParserOptions { Tokens = true }).Tokens
                .Where(o => o.Type == global::Jint.Parser.Tokens.Identifier)
                .Select(o => o.Value.ToString())
                .Intersect(context.GetAllEntityFields().Select(f => f.Alias))
                .Distinct()
                .Select(a => context.Entity.GetField(a))
                .ToArray();
        }

        public bool Eval(IRow row) {
            foreach (var field in _input) {
                _jint.SetValue(field.Alias, row[field]);
            }
            return _jint.Execute(_expression).GetCompletionValue().AsBoolean();
        }
    }
}