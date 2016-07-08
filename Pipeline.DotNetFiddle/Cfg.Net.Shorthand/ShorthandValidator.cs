using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Shorthand {
    public class ShorthandValidator : INodeValidator {
        private readonly ShorthandRoot _root;

        public ShorthandValidator(ShorthandRoot root, string name) {
            _root = root;
            Name = name;
        }

        public string Name { get; set; }
        public void Validate(INode node, string value, IDictionary<string, string> parameters, ILogger logger) {
            if (string.IsNullOrEmpty(value))
                return;
            var expressions = new Expressions(value);
            foreach (var expression in expressions) {
                MethodData methodData;
                if (!_root.MethodDataLookup.TryGetValue(expression.Method, out methodData)) {
                    logger.Warn($"The short-hand expression method {expression.Method} is undefined.");
                }
            }
        }
    }
}