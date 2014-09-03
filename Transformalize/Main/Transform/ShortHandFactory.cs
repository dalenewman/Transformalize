using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Main.Transform {
    public class ShortHandFactory {

        private static readonly Dictionary<string, string> Methods = new Dictionary<string, string> {
            {"r","replace"},
            {"replace","replace"},
            {"l","left"},
            {"left","left"},
            {"ri","right"},
            {"right","right"},
            {"a","append"},
            {"append","append"},
            {"i","if"},
            {"if","if"},
            {"iif","if"},
            {"cv","convert"},
            {"convert","convert"},
            {"cp","copy"},
            {"copy","copy"},
            {"cc","concat"},
            {"concat","concat"},
            {"hc","hashcode"},
            {"hash","hashcode"},
            {"hashcode","hashcode"},
            {"co","compress"},
            {"compress","compress"},
            {"de","decompress"},
            {"decompress","decompress"},
            {"e","elipse"},
            {"elipse","elipse"},
            {"rr","regexreplace"},
            {"regexreplace","regexreplace"},
            {"sh","striphtml"},
            {"striphtml","striphtml"},
            {"join","join"},
            {"j","join"},
            {"f","format"},
            {"format","format"},
            {"in","insert"},
            {"insert","insert"},
            {"ii","insertinterval"},
            {"insertinterval","insertinterval"},
            {"tl","transliterate"},
            {"transliterate","transliterate"},
            {"sl","slug"},
            {"slug","slug"},
            {"slugify","slug"},
            {"cl","cyrtolat"},
            {"cyrtolat","cyrtolat"},
            {"distinctwords","distinctwords"},
            {"dw","distinctwords"},
            {"guid","guid"},
            {"g","guid"},
            {"now","now"},
            {"n","now"},
            {"remove","remove"},
            {"rm","remove"},
            {"ts","trimstart"},
            {"trimstart","trimstart"},
            {"ta","trimstartappend"},
            {"tsa","trimstartappend"},
            {"trimstartappend","trimstartappend"},
            {"trimend","trimend"},
            {"te", "trimend"},
            {"t","trim"},
            {"tr","trim"},
            {"trim","trim"},
            {"substring","substring"},
            {"ss","substring"},
            {"sub","substring"},
            {"map","map"},
            {"m","map"}
        };

        public static readonly Dictionary<string, Func<string, TransformConfigurationElement>> Functions = new Dictionary<string, Func<string, TransformConfigurationElement>> {
            {"replace", Replace},
            {"left", Left},
            {"right", Right},
            {"append", arg => new TransformConfigurationElement() { Method="append", Parameter = arg}},
            {"if", If},
            {"convert", Convert},
            {"copy", arg => new TransformConfigurationElement() { Method ="copy", Parameter = arg}},
            {"concat", Concat},
            {"hashcode", arg => new TransformConfigurationElement() {Method = "gethashcode"}},
            {"compress", arg => new TransformConfigurationElement() {Method = "compress", Parameter = arg} },
            {"decompress", arg => new TransformConfigurationElement() {Method = "decompress", Parameter = arg}},
            {"elipse", Elipse},
            {"regexreplace", RegexReplace},
            {"striphtml", arg=> new TransformConfigurationElement() {Method = "striphtml", Parameter = arg}},
            {"join",Join},
            {"format", Format},
            {"insert", Insert},
            {"insertinterval", InsertInterval},
            {"transliterate", arg=> new TransformConfigurationElement() { Method="transliterate", Parameter = arg}},
            {"slug", Slug},
            {"cyrtolat", arg=> new TransformConfigurationElement() {Method = "cyrtolat", Parameter = arg}},
            {"distinctwords", arg=> new TransformConfigurationElement() {Method = "distinctwords", Separator = arg}},
            {"guid", arg=>new TransformConfigurationElement() { Method = "guid"}},
            {"now", arg=>new TransformConfigurationElement() { Method = "now"}},
            {"remove", Remove},
            {"trimstart", arg=> new TransformConfigurationElement() {Method = "trimstart", TrimChars = arg}},
            {"trimstartappend", TrimStartAppend},
            {"trimend", arg=> new TransformConfigurationElement() {Method = "trimend", TrimChars = arg}},
            {"trim", arg=> new TransformConfigurationElement() {Method = "trim", TrimChars = arg}},
            {"substring", Substring},
            {"map", Map}
        };

        private static TransformConfigurationElement Map(string arg) {
            Guard.Against(arg.Equals(string.Empty), "The map method requires at least one parameter; the map name.  An additional parameter may reference another field to represent the value being mapped.");
            var split = SplitComma(arg);
            var element = new TransformConfigurationElement() { Method = "map" };
            var hasInlineMap = arg.Contains("=");
            foreach (var p in split) {
                if (hasInlineMap) {
                    if (p.Contains("=")) {
                        element.Map = element.Map + "," + p;
                    } else {
                        element.Parameter = p;
                    }
                } else {
                    if (element.Map.Equals(string.Empty)) {
                        element.Map = p;
                    } else {
                        element.Parameter = p;
                    }
                }
            }
            if (hasInlineMap) {
                element.Map = element.Map.TrimStart(new[] { ',' });
            }

            return element;
        }

        private static TransformConfigurationElement TrimStartAppend(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The trimstartappend method requires at least one parameter indicating the trim characters.");

            var element = new TransformConfigurationElement() { Method = "trimstartappend", TrimChars = split[0] };

            if (split.Length > 1) {
                element.Separator = split[1];
            }
            return element;
        }

        private static TransformConfigurationElement Substring(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The substring method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return new TransformConfigurationElement() { Method = "substring", StartIndex = startIndex, Length = length };
            }

            throw new TransformalizeException("The substring method requires two integers indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TransformConfigurationElement Remove(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The remove method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return new TransformConfigurationElement() { Method = "remove", StartIndex = startIndex, Length = length };
            }

            throw new TransformalizeException("The remove method requires two integer parameters indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TransformConfigurationElement Slug(string arg) {
            var element = new TransformConfigurationElement() { Method = "slug" };

            var split = SplitComma(arg);

            Guard.Against(split.Length > 2, "The slug method takes up to 2 arguments; an integer representing the maximum length of the slug, and a parameter.  The parameter is optional if you intend to operate on a field (instead of a calculated field). '{0}' has too many arguments.", arg);

            foreach (var p in split) {
                int length;
                if (int.TryParse(p, out length)) {
                    element.Length = length;
                } else {
                    element.Parameter = p;
                }
            }

            return element;
        }

        private static TransformConfigurationElement InsertInterval(string arg) {
            //interval, value
            var split = SplitComma(arg);

            Guard.Against(split.Length != 2, "The insertinterval method requires two parameters: the interval (e.g. every certain number of characters), and the value to insert. '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() { Method = "insertinterval" };

            int interval;
            if (int.TryParse(split[0], out interval)) {
                element.Interval = interval;
            } else {
                throw new TransformalizeException("The insertinterval method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Value = split[1];
            return element;
        }

        private static TransformConfigurationElement Insert(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length != 2, "The insert method requires two parameters; the start index, and the value (or field reference) you'd like to insert.  '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() { Method = "insert" };

            int startIndex;
            if (int.TryParse(split[0], out startIndex)) {
                element.StartIndex = startIndex;
            } else {
                throw new TransformalizeException("The insert method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Parameter = split[1];
            return element;
        }

        private static TransformConfigurationElement Join(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The join method requires a a separator, and then a * (for all fields) or a comma delimited list of parameters that reference fields.");

            var element = new TransformConfigurationElement() { Method = "join", Separator = split[0] };

            if (split.Length == 2) {
                element.Parameter = split[1];
                return element;
            }

            foreach (var p in split.Skip(1)) {
                element.Parameters.Add(new ParameterConfigurationElement() { Field = p });
            }

            return element;
        }

        private static TransformConfigurationElement Concat(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The concat method requires a * parameter, or a comma delimited list of parameters that reference fields.");

            var element = new TransformConfigurationElement() { Method = "concat" };

            if (split.Length == 1) {
                element.Parameter = split[0];
            } else {
                foreach (var p in split) {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = p });
                }
            }
            return element;
        }

        public static TransformConfigurationElement Interpret(string expression) {

            Guard.Against(expression == null, "You may not pass a null expression.");
            // ReSharper disable once PossibleNullReferenceException

            var split = expression.Contains("(") ?
                expression.Split(new[] { '(' }, StringSplitOptions.None) :
                new[] { expression, "" };

            var method = split[0].ToLower();

            Guard.Against(!Methods.ContainsKey(method), "Sorry. Your expression '{0}' references an undefined method: '{1}'.", expression, method);

            return Functions[Methods[method]](split[1].TrimEnd(new[] { ')' }));
        }

        private static TransformConfigurationElement RegexReplace(string arg) {

            Guard.Against(arg.Equals(string.Empty), "The regexreplace requires two parameters: a regular expression pattern, and replacement text.  You didn't pass in any parameters.");

            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The regexreplace method requires at least two parameters: the pattern, and the replacement text.  A third parameter, count (how many to replace) is optional. The argument '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() {
                Method = "regexreplace",
                Pattern = split[0],
                Replacement = split[1]
            };

            if (split.Length <= 2)
                return element;

            int count;
            Guard.Against(!int.TryParse(split[2], out count), "The regexreplace's third parameter; count, must be an integer. The argument '{0}' contains '{1}'.", arg, split[2]);
            element.Count = count;

            return element;
        }

        private static TransformConfigurationElement Replace(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The replace method requires two parameters: an old value, and a new value. Your arguments '{0}' resolve {1} parameter{2}.", arg, split.Length, split.Length.Plural());
            var oldValue = split[0];
            var newValue = split[1];
            return new TransformConfigurationElement() {
                Method = "replace", OldValue = oldValue, NewValue = newValue
            };
        }

        private static TransformConfigurationElement Convert(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The convert method requires the first parameter reference another field's alias (or name).");

            var element = new TransformConfigurationElement() { Method = "convert", Parameter = split[0] };
            if (split.Length <= 1)
                return element;

            foreach (var p in split.Skip(1)) {
                if (System.Text.Encoding.GetEncodings().Any(e => e.Name.Equals(p, StringComparison.OrdinalIgnoreCase))) {
                    element.Encoding = p;
                } else if (Common.TypeMap.ContainsKey(Common.ToSimpleType(p))) {
                    element.To = Common.ToSimpleType(p);
                } else {
                    element.Format = p;
                }
            }

            return element;
        }

        private static TransformConfigurationElement If(string arg) {
            var linked = new LinkedList<string>(SplitComma(arg));

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

        private static string[] SplitComma(string arg) {
            if (arg.Equals(string.Empty))
                return new string[0];

            var placeHolder = arg.GetHashCode().ToString(CultureInfo.InvariantCulture);
            var split = arg.Replace("\\,", placeHolder).Split(new[] { ',' }, StringSplitOptions.None);
            return split.Select(s => s.Replace(placeHolder, ",")).ToArray();
        }

        private static TransformConfigurationElement Elipse(string arg) {
            var element = new TransformConfigurationElement() { Method = "elipse" };
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The elipse method requires a an integer representing the number of characters allowed before the elipse.");

            int length;
            Guard.Against(!int.TryParse(split[0], out length), "The elipse method requires a an integer representing the number of characters allowed before the elipse. You passed in '{0}'.", split[0]);
            element.Length = length;

            if (split.Length > 1) {
                element.Elipse = split[1];
            }
            return element;
        }

        private static TransformConfigurationElement Format(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The format method requires at least one parameter; the format with {{0}} style place-holders in it.  For each place-holder, add additional parameters that reference fields.  If no fields are referenced, the first parameter is assumed to be the field this transform is nested in.");
            var element = new TransformConfigurationElement() { Method = "format", Format = split[0] };

            if (split.Length <= 1)
                return element;

            if (split.Length == 2) {
                element.Parameter = split[1];
                return element;
            }

            foreach (var s in split.Skip(1)) {
                element.Parameters.Add(new ParameterConfigurationElement() { Field = s });
            }
            return element;
        }
    }
}