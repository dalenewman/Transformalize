using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Main.Transform {

    public class ShortHandFactory {

        private static readonly Dictionary<string, string> Methods = new Dictionary<string, string> {
            {"ad", "add"},
            {"add", "add"},
            {"ap","append"},
            {"append","append"},
            {"cc","concat"},
            {"cl","cyrtolat"},
            {"co","compress"},
            {"col","collapse"},
            {"collapse","collapse"},
            {"compress","compress"},
            {"concat","concat"},
            {"convert","convert"},
            {"copy","copy"},
            {"cp","copy"},
            {"cs","csharp"},
            {"csharp","csharp"},
            {"cv","convert"},
            {"cyrtolat","cyrtolat"},
            {"de","decompress"},
            {"decompress","decompress"},
            {"distinctwords","distinctwords"},
            {"dw","distinctwords"},
            {"e","elipse"},
            {"elipse","elipse"},
            {"f","format"},
            {"fj","fromjson"},
            {"format","format"},
            {"fr","fromregex"},
            {"fromjson","fromjson"},
            {"fromregex", "fromregex"},
            {"fromsplit","fromsplit"},
            {"fromxml","fromxml"},
            {"fs","fromsplit"},
            {"fx", "fromxml"},
            {"g","guid"},
            {"guid","guid"},
            {"hash","hashcode"},
            {"hashcode","hashcode"},
            {"hc","hashcode"},
            {"he","htmlencode"},
            {"htmlencode","htmlencode"},
            {"i","if"},
            {"ids","isdaylightsavings"},
            {"if","if"},
            {"ii","insertinterval"},
            {"iif","if"},
            {"in","insert"},
            {"insert","insert"},
            {"insertinterval","insertinterval"},
            {"isdaylightsavings","isdaylightsavings"},
            {"j","join"},
            {"javascript","javascript"},
            {"join","join"},
            {"js","javascript"},
            {"l","left"},
            {"left","left"},
            {"low","tolower"},
            {"lower","tolower"},
            {"m","map"},
            {"map","map"},
            {"n","now"},
            {"now","now"},
            {"padleft","padleft"},
            {"padright","padright"},
            {"pl","padleft"},
            {"pr","padright"},
            {"r","replace"},
            {"razor","template"},
            {"regexreplace","regexreplace"},
            {"remove","remove"},
            {"replace","replace"},
            {"ri","right"},
            {"right","right"},
            {"rm","remove"},
            {"rr","regexreplace"},
            {"rz", "template"},
            {"sh","striphtml"},
            {"sl","slug"},
            {"slug","slug"},
            {"slugify","slug"},
            {"ss","substring"},
            {"striphtml","striphtml"},
            {"sub","substring"},
            {"substring","substring"},
            {"sum", "add"},
            {"t","trim"},
            {"ta","trimstartappend"},
            {"tag","tag"},
            {"tc","totitlecase"},
            {"te", "trimend"},
            {"template","template"},
            {"timezone","timezone"},
            {"titlecase","totitlecase"},
            {"tj","tojson"},
            {"tl","tolower"},
            {"tlr","transliterate"},
            {"tojson","tojson"},
            {"tolower","tolower"},
            {"tos","tostring"},
            {"tostring","tostring"},
            {"totitlecase","totitlecase"},
            {"toupper","toupper"},
            {"tp","template"},
            {"tr","trim"},
            {"transliterate","transliterate"},
            {"trim","trim"},
            {"trimend","trimend"},
            {"trimstart","trimstart"},
            {"trimstartappend","trimstartappend"},
            {"ts","trimstart"},
            {"tsa","trimstartappend"},
            {"tu","toupper"},
            {"tz","timezone"},
            {"ue","urlencode"},
            {"up","toupper"},
            {"upper","toupper"},
            {"urlencode","urlencode"},
            {"v","velocity"},
            {"velocity","velocity"},
            {"w","web"},
            {"web","web"},
        };

        public static readonly Dictionary<string, Func<string, TransformConfigurationElement>> Functions = new Dictionary<string, Func<string, TransformConfigurationElement>> {
            {"replace", Replace},
            {"left", Left},
            {"right", Right},
            {"append", arg => new TransformConfigurationElement() { Method="append", Parameter = arg, IsShortHand = true}},
            {"if", If},
            {"convert", Convert},
            {"copy", arg => new TransformConfigurationElement() { Method ="copy", Parameter = arg, IsShortHand = true}},
            {"concat", Concat},
            {"hashcode", arg => new TransformConfigurationElement() {Method = "gethashcode", IsShortHand = true}},
            {"compress", arg => new TransformConfigurationElement() {Method = "compress", Parameter = arg, IsShortHand = true} },
            {"decompress", arg => new TransformConfigurationElement() {Method = "decompress", Parameter = arg, IsShortHand = true}},
            {"elipse", Elipse},
            {"regexreplace", RegexReplace},
            {"striphtml", arg=> new TransformConfigurationElement() {Method = "striphtml", Parameter = arg, IsShortHand = true}},
            {"join",Join},
            {"format", Format},
            {"insert", Insert},
            {"insertinterval", InsertInterval},
            {"transliterate", arg=> new TransformConfigurationElement() { Method="transliterate", Parameter = arg, IsShortHand = true}},
            {"slug", Slug},
            {"cyrtolat", arg=> new TransformConfigurationElement() {Method = "cyrtolat", Parameter = arg, IsShortHand = true}},
            {"distinctwords", arg=> new TransformConfigurationElement() {Method = "distinctwords", Separator = arg, IsShortHand = true}},
            {"guid", arg=>new TransformConfigurationElement() { Method = "guid", IsShortHand = true}},
            {"now", arg=>new TransformConfigurationElement() { Method = "now", IsShortHand = true}},
            {"remove", Remove},
            {"trimstart", arg=> new TransformConfigurationElement() {Method = "trimstart", TrimChars = arg, IsShortHand = true}},
            {"trimstartappend", TrimStartAppend},
            {"trimend", arg=> new TransformConfigurationElement() {Method = "trimend", TrimChars = arg, IsShortHand = true}},
            {"trim", arg=> new TransformConfigurationElement() { Method = "trim", TrimChars = arg, IsShortHand = true}},
            {"substring", Substring},
            {"map", Map},
            {"urlencode", arg=>new TransformConfigurationElement() { Method = "urlencode", Parameter = arg, IsShortHand = true}},
            {"web", Web},
            {"add", Add},
            {"fromjson",FromJson},
            {"padleft", PadLeft},
            {"padright", PadRight},
            {"tostring", ToString},
            {"tolower", arg=> new TransformConfigurationElement() { Method = "tolower", Parameter = arg, IsShortHand = true} },
            {"toupper", arg=> new TransformConfigurationElement() { Method = "toupper", Parameter = arg, IsShortHand = true}},
            {"javascript", JavaScript},
            {"csharp", CSharp},
            {"template", Template},
            {"totitlecase", arg=> new TransformConfigurationElement() {Method="totitlecase", Parameter = arg, IsShortHand = true}},
            {"timezone",TimeZone},
            {"tojson", ToJson},
            {"fromxml", FromXml},
            {"fromregex", FromRegex},
            {"fromsplit", FromSplit},
            {"velocity", Velocity},
            {"tag",Tag},
            {"htmlencode", arg=>new TransformConfigurationElement() {Method="htmlencode", Parameter= arg, IsShortHand = true}},
            {"isdaylightsavings", arg=>new TransformConfigurationElement() {Method="isdaylightsavings", Parameter = arg, IsShortHand = true}},
            {"collapse", arg=>new TransformConfigurationElement(){ Method = "collapse", Parameter = arg, IsShortHand = true }}
        };

        /// <summary>
        /// Converts t attribute to transforms.
        /// </summary>
        /// <param name="f">the field</param>
        public static void ExpandShortHandTransforms(FieldConfigurationElement f) {
            var transforms = new List<TransformConfigurationElement>(Common.Split(f.ShortHand, ").").Where(t => !string.IsNullOrEmpty(t)).Select(ShortHandFactory.Interpret));
            var collection = new TransformElementCollection();
            foreach (var transform in transforms) {
                foreach (FieldConfigurationElement field in transform.Fields) {
                    ExpandShortHandTransforms(field);
                }
                collection.Add(transform);
            }
            foreach (TransformConfigurationElement transform in f.Transforms) {
                foreach (FieldConfigurationElement field in transform.Fields) {
                    ExpandShortHandTransforms(field);
                }
                collection.Add(transform);
            }
            f.Transforms = collection;
        }

        /// <summary>
        /// Converts t attribute to configuration items for the whole process
        /// </summary>
        /// <param name="process">the process</param>
        public static void ExpandShortHandTransforms(ProcessConfigurationElement process) {
            foreach (EntityConfigurationElement entity in process.Entities) {
                foreach (FieldConfigurationElement field in entity.Fields) {
                    ExpandShortHandTransforms(field);
                }
                foreach (FieldConfigurationElement field in entity.CalculatedFields) {
                    ExpandShortHandTransforms(field);
                }
            }
            foreach (FieldConfigurationElement field in process.CalculatedFields) {
                ExpandShortHandTransforms(field);
            }
        }

        private static TransformConfigurationElement FromSplit(string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The fromsplit method requires atleast two parameters: the separator, and a field.");

            var element = Fields("fromsplit", arg);
            element.Separator = split[0];
            element.Fields.RemoveAt(0);
            return element;
        }

        private static TransformConfigurationElement FromRegex(string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The fromregex requires at least two parameters; a pattern with a named group in it, and a field name (with the same name).  Hmm... I guess that is redundant, but that's the way it is for now.");

            var element = new TransformConfigurationElement() { Method = "fromregex", Pattern = split[0], IsShortHand = true };

            foreach (var p in split.Skip(1)) {
                element.Fields.Add(new FieldConfigurationElement() { Name = p, Length = "4000", Input = false });
            }
            return element;
        }

        private static TransformConfigurationElement FromXml(string arg) {
            return Fields("fromxml", arg);
        }


        private static TransformConfigurationElement ToJson(string arg) {
            var element = Parameters("tojson", arg, 0);
            return element;
        }

        private static TransformConfigurationElement Template(string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The template/razor method requires at least two paramters: a template, and a parameter.");

            var element = new TransformConfigurationElement() { Method = "template", Template = split[0], IsShortHand = true };

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                foreach (var s in split.Skip(1)) {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = s });
                }
            }

            return element;
        }

        private static TransformConfigurationElement Velocity(string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The velocity method requires at least two paramters: a template, and a parameter.");

            var element = new TransformConfigurationElement() { Method = "velocity", Template = split[0], IsShortHand = true };

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                foreach (var s in split.Skip(1)) {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = s });
                }
            }

            return element;
        }

        private static TransformConfigurationElement Script(string method, string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The {0} method requires at least two paramters: a script, and a parameter.", method);

            var element = new TransformConfigurationElement() { Method = method, Script = split[0], IsShortHand = true };

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                foreach (var s in split.Skip(1)) {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = s });
                }
            }

            return element;
        }

        private static TransformConfigurationElement CSharp(string arg) {
            return Script("csharp", arg);
        }

        private static TransformConfigurationElement JavaScript(string arg) {
            return Script("javascript", arg);
        }

        private static TransformConfigurationElement PadLeft(string arg) {
            return Pad("padleft", arg);
        }

        private static TransformConfigurationElement PadRight(string arg) {
            return Pad("padright", arg);
        }

        private static TransformConfigurationElement Pad(string method, string arg) {

            Guard.Against(arg.Equals(string.Empty), "The {0} method requires two pararmeters: the total width, and the padding character(s).", method);

            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The {0} method requires two pararmeters: the total width, and the padding character(s).  You've provided {1} parameter{2}.", method, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() { Method = method, IsShortHand = true };

            int totalWidth;
            if (int.TryParse(split[0], out totalWidth)) {
                element.TotalWidth = totalWidth;
            } else {
                throw new TransformalizeException("The {0} method requires the first parameter to be total width; an integer. {1} is not an integer", method, split[0]);
            }

            element.PaddingChar = split[1];
            Guard.Against(element.PaddingChar.Length < 1, "The {0} second parameter, the padding character(s), must be at least one character.  You can't pad something with nothing.", method);

            if (split.Length > 2) {
                element.Parameter = split[2];
            }
            return element;
        }

        private static TransformConfigurationElement FromJson(string arg) {
            Guard.Against(arg.Equals(string.Empty), "The fromjson method requires at least one parameter: the json field name, or path in brackets (i.e. [customer] or [customer][first_name]).");

            var element = new TransformConfigurationElement() { Method = "fromjson", IsShortHand = true };
            var type = "string";
            TransformConfigurationElement current = element;
            foreach (var p in SplitComma(arg)) {
                if (Common.TypeMap.ContainsKey(Common.ToSimpleType(p.ToLower()))) {
                    type = Common.ToSimpleType(p);
                } else if (p.StartsWith("[")) {
                    var brackets = new[] { ']', '[' };
                    var fields = p.Trim(brackets).Split(brackets, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < fields.Length; i++) {
                        var field = fields[i];
                        if (i < fields.Length - 1) {
                            current.Fields.Add(new FieldConfigurationElement() { Name = field, Length = "4000", Output = false });
                            var fromJson = new TransformConfigurationElement() { Method = "fromjson" };
                            current.Fields[0].Transforms.Add(fromJson);
                            current = fromJson;
                        } else { //last
                            current.Fields.Add(new FieldConfigurationElement() { Name = field, Length = "4000", Output = true });
                        }
                    }
                } else {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = p });
                }
            }
            current.Fields[0].Type = type;
            return element;
        }

        private static TransformConfigurationElement Web(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length > 2, "The web method takes two optional parameters: a parameter referencing a field, and an integer representing sleep ms in between web requests.  You have {0} parameter{1} in '{2}'.", split.Length, split.Length.Plural(), arg);

            var element = new TransformConfigurationElement() { Method = "web", IsShortHand = true };

            foreach (var p in split) {
                int sleep;
                if (int.TryParse(p, out sleep)) {
                    element.Sleep = sleep;
                } else {
                    element.Parameter = p;
                }
            }
            return element;
        }

        private static TransformConfigurationElement Map(string arg) {
            Guard.Against(arg.Equals(string.Empty), "The map method requires at least one parameter; the map name.  An additional parameter may reference another field to represent the value being mapped.");
            var split = SplitComma(arg);
            var element = new TransformConfigurationElement() { Method = "map", IsShortHand = true };
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

            var element = new TransformConfigurationElement() { Method = "trimstartappend", TrimChars = split[0], IsShortHand = true };

            if (split.Length > 1) {
                element.Separator = split[1];
            }

            if (split.Length > 2) {
                element.Parameter = split[2];
            }

            return element;
        }

        private static TransformConfigurationElement Substring(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The substring method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return new TransformConfigurationElement() { Method = "substring", StartIndex = startIndex, Length = length, IsShortHand = true };
            }

            throw new TransformalizeException(string.Empty, string.Empty, "The substring method requires two integers indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TransformConfigurationElement Remove(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The remove method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return new TransformConfigurationElement() { Method = "remove", StartIndex = startIndex, Length = length, IsShortHand = true };
            }

            throw new TransformalizeException(string.Empty, string.Empty, "The remove method requires two integer parameters indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TransformConfigurationElement Slug(string arg) {
            var element = new TransformConfigurationElement() { Method = "slug", IsShortHand = true };

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

            var element = new TransformConfigurationElement() { Method = "insertinterval", IsShortHand = true };

            int interval;
            if (int.TryParse(split[0], out interval)) {
                element.Interval = interval;
            } else {
                throw new TransformalizeException(string.Empty, string.Empty, "The insertinterval method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Value = split[1];
            return element;
        }

        private static TransformConfigurationElement Insert(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length != 2, "The insert method requires two parameters; the start index, and the value (or field reference) you'd like to insert.  '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() { Method = "insert", IsShortHand = true };

            int startIndex;
            if (int.TryParse(split[0], out startIndex)) {
                element.StartIndex = startIndex;
            } else {
                throw new TransformalizeException(string.Empty, string.Empty, "The insert method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Parameter = split[1];
            return element;
        }

        private static TransformConfigurationElement Join(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The join method requires a separator, and then a * (for all fields) or a comma delimited list of parameters that reference fields.");

            var element = new TransformConfigurationElement() { Method = "join", Separator = split[0], IsShortHand = true };

            if (split.Length == 2) {
                element.Parameter = split[1];
                return element;
            }

            foreach (var p in split.Skip(1)) {
                element.Parameters.Add(new ParameterConfigurationElement() { Field = p });
            }

            return element;
        }

        private static TransformConfigurationElement Add(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The add method requires a * parameter, or a comma delimited list of parameters that reference numeric fields.");

            var element = new TransformConfigurationElement() { Method = "add", IsShortHand = true };

            if (split.Length == 1) {
                element.Parameter = split[0];
            } else {
                foreach (var p in split) {
                    element.Parameters.Add(
                        p.IsNumeric() ?
                            new ParameterConfigurationElement() { Name = p, Value = p } :
                            new ParameterConfigurationElement() { Field = p });
                }
            }
            return element;
        }

        private static TransformConfigurationElement Parameters(string method, string arg, int skip) {
            var split = SplitComma(arg, skip);
            Guard.Against(split.Length == 0, "The {0} method requires parameters.", method);

            var element = new TransformConfigurationElement() { Method = method, IsShortHand = true };

            if (split.Length == 1) {
                element.Parameter = split[0];
            } else {
                foreach (var p in split) {
                    element.Parameters.Add(new ParameterConfigurationElement() { Field = p });
                }
            }

            // handle single parameter that is named parameter
            if (element.Parameter.Contains(":")) {
                var pair = Common.Split(element.Parameter, ":");
                element.Parameters.Insert(new ParameterConfigurationElement() {
                    Field = string.Empty,
                    Name = pair[0],
                    Value = pair[1]
                });
                element.Parameter = string.Empty;
            }

            // handle regular parameters
            foreach (ParameterConfigurationElement p in element.Parameters) {
                if (!p.Field.Contains(":"))
                    continue;
                var pair = Common.Split(p.Field, ":");
                p.Field = string.Empty;
                p.Name = pair[0];
                p.Value = pair[1];
            }

            return element;
        }

        private static TransformConfigurationElement Fields(string method, string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The {0} method requires a comma delimited list of fields.", method);

            var element = new TransformConfigurationElement() { Method = method, IsShortHand = true };
            foreach (var p in split) {
                element.Fields.Add(new FieldConfigurationElement() { Name = p, Length = "4000", Input = false });
            }
            return element;
        }


        private static TransformConfigurationElement Concat(string arg) {
            return Parameters("concat", arg, 0);
        }

        public static TransformConfigurationElement Interpret(string expression) {

            string method;
            var arg = string.Empty;
            Guard.Against(expression == null, "You may not pass a null expression.");
            // ReSharper disable once PossibleNullReferenceException
            if (expression.Contains("(")) {
                var index = expression.IndexOf('(');
                method = expression.Left(index).ToLower();
                arg = expression.Remove(0, index + 1).TrimEnd(new[] { ')' });
            } else {
                method = expression;
            }

            Guard.Against(!Methods.ContainsKey(method), "Sorry. Your expression '{0}' references an undefined method: '{1}'.", expression, method);

            return Functions[Methods[method]](arg);
        }

        private static TransformConfigurationElement RegexReplace(string arg) {

            Guard.Against(arg.Equals(string.Empty), "The regexreplace requires two parameters: a regular expression pattern, and replacement text.  You didn't pass in any parameters.");

            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The regexreplace method requires at least two parameters: the pattern, and the replacement text.  A third parameter, count (how many to replace) is optional. The argument '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = new TransformConfigurationElement() {
                Method = "regexreplace",
                Pattern = split[0],
                Replacement = split[1], IsShortHand = true
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
                Method = "replace", OldValue = oldValue, NewValue = newValue, IsShortHand = true
            };
        }

        private static TransformConfigurationElement Convert(string arg) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The convert and tostring methods require the first parameter reference another field's alias (or name).");

            var element = new TransformConfigurationElement() { Method = "convert", Parameter = split[0], IsShortHand = true };
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

        private static TransformConfigurationElement ToString(string arg) {
            var element = Convert(arg);
            element.Method = "tostring";
            element.To = "string";
            return element;
        }

        private static TransformConfigurationElement If(string arg) {
            var linked = new LinkedList<string>(SplitComma(arg));

            Guard.Against(linked.Count < 2, "The if method requires at least 2 arguments. Your argument '{0}' has {1}.", arg, linked.Count);

            // left is required first, assign and remove
            var element = new TransformConfigurationElement() {
                Method = "if",
                Left = linked.First.Value, IsShortHand = true
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
            return new TransformConfigurationElement() { Method = "right", Length = length, IsShortHand = true };
        }

        private static TransformConfigurationElement Left(string arg) {
            int length;
            Guard.Against(!int.TryParse(arg, out length), "The left method requires a single integer representing the length, or how many left-most characters you want. You passed in '{0}'.", arg);
            return new TransformConfigurationElement() { Method = "left", Length = length, IsShortHand = true };
        }

        private static string[] SplitComma(string arg, int skip = 0) {
            return Common.Split(arg, ",", skip);
        }

        private static TransformConfigurationElement Elipse(string arg) {
            var element = new TransformConfigurationElement() { Method = "elipse", IsShortHand = true };
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
            var element = new TransformConfigurationElement() { Method = "format", Format = split[0], IsShortHand = true };

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

        private static TransformConfigurationElement TimeZone(string arg) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The timezone method requires at least two parameters: the from-time-zone, and the to-time-zone.");

            var element = new TransformConfigurationElement() { Method = "timezone", IsShortHand = true };

            foreach (var p in split) {
                try {
                    TimeZoneInfo.FindSystemTimeZoneById(p);
                    if (string.IsNullOrEmpty(element.FromTimeZone)) {
                        element.FromTimeZone = p;
                    } else {
                        element.ToTimeZone = p;
                    }
                } catch (TimeZoneNotFoundException ex) {
                    if (string.IsNullOrEmpty(element.Parameter)) {
                        element.Parameter = p;
                    } else {
                        throw new TransformalizeException("The timezone method already has a parameter of {0}, and it can't interpret {1} as a valid time-zone identifer. {2}", element.Parameter, p, ex.Message);
                    }
                }
            }
            return element;
        }

        private static TransformConfigurationElement Tag(string arg) {
            var split = SplitComma(arg).ToList();
            Guard.Against(split.Count < 2, "The tag method requires at least 2 parameters: the tag (aka element name), and a parameter (which becomes an attribute of the tag/element).  With {0}, You passed in {1} parameter{2}.", arg, split.Count, split.Count.Plural());
            var element = Parameters("tag", arg, 1);
            element.Tag = split[0];
            return element;
        }
    }
}