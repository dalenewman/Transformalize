using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Transform {
    public class ShortHandFactory {
        private static readonly Dictionary<string, string> Methods = new Dictionary<string, string> {
            {"r","replace"},
            {"l","left"},
            {"ri","right"},
            {"a","append"},
            {"i","if"}
        };
        public static readonly Dictionary<string, Func<string, TransformConfigurationElement>> Functions = new Dictionary<string, Func<string, TransformConfigurationElement>> {
            {"replace", Replace},
            {"left", Left},
            {"right", Right},
            {"append", arg => new TransformConfigurationElement() { Method="append", Value = arg}},
            {"if", If}

        };

        public static TransformConfigurationElement Interpret(string expression) {

            Guard.Against(expression == null, "You may not pass a null expression.");
            // ReSharper disable once PossibleNullReferenceException
            Guard.Against(!expression.Contains("("), "The short-hand expression must contain a '(' character that separates the method from the arguments.  Your expression of '{0}' does not have one.", expression);

            var split = expression.Split(new[] { '(' }, StringSplitOptions.None);

            var method = split[0].ToLower();

            Guard.Against(!Methods.ContainsKey(method), "Sorry. Your expression '{0}' references an undefined method: '{1}'.", expression, method);

            return Functions[Methods[method]](split[1]);
        }

        private static TransformConfigurationElement Replace(string arg) {
            var split = Split(arg);
            Guard.Against(split.Length < 2, "The replace method requires two parameters: an old value, and a new value. Your arguments '{0}' resolve {1} parameter{2}.", arg, split.Length, split.Length.Plural());
            var oldValue = split[0];
            var newValue = split[1];
            return new TransformConfigurationElement() {
                Method = "replace", OldValue = oldValue, NewValue = newValue
            };
        }

        private static TransformConfigurationElement If(string arg) {
            var linked = new LinkedList<string>(Split(arg));

            Guard.Against(linked.Count < 2, "The if method requires at least 2 arguments. Your argument '{0}' has {1}.", arg, linked.Count);

            // left is required first, assign and remove
            var element = new TransformConfigurationElement() {
                Method = "if",
                Left = linked.First.Value
            };
            linked.RemoveFirst();

            // operator is second, but optional, assign and remove if present
            ComparisonOperator op;
            if (Enum.TryParse(linked.First.Value, true, out op)) {
                element.Operator = op.ToString();
                linked.RemoveFirst();
            }

            // right, then, and else in that order
            var split = linked.ToList();
            for (var i = 0; i < split.Count; i++) {
                switch (i) {
                    case 0:
                        element.Right = split[i];
                        break;
                    case 1:
                        element.Then = split[i];
                        break;
                    case 2:
                        element.Else = split[i];
                        break;
                }
            }
            return element;
        }

        private static TransformConfigurationElement Right(string arg) {
            int length;
            Guard.Against(!int.TryParse(arg, out length), "The right method requires a single integer representing the length, or how many right-most characters you want. You passed in '{0}'.", arg);
            return new TransformConfigurationElement() { Method = "right", Length = length };
        }

        public static TransformConfigurationElement Left(string arg) {
            int length;
            Guard.Against(!int.TryParse(arg, out length), "The left method requires a single integer representing the length, or how many left-most characters you want. You passed in '{0}'.", arg);
            return new TransformConfigurationElement() { Method = "left", Length = length };
        }

        private static string[] Split(string arg) {
            return arg.Split(new[] { ',' }, StringSplitOptions.None);
        }

    }
}