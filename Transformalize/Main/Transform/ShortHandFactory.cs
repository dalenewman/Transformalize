using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Elasticsearch.Net.Serialization;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.FileHelpers.DataLink;

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

        public static readonly Dictionary<string, Func<string, TflField, TflTransform>> Functions = new Dictionary<string, Func<string, TflField, TflTransform>> {
            {"replace", Replace},
            {"left", Left},
            {"right", Right},
            {"append", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{ t.Method="append"; t.Parameter = arg; t.IsShortHand = true;})}, 
            {"if", If},
            {"convert", Convert},
            {"copy", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method ="copy"; t.Parameter = arg; t.IsShortHand = true;})},
            {"concat", Concat},
            {"hashcode", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "gethashcode"; t.IsShortHand = true;})},
            {"compress", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "compress"; t.Parameter = arg; t.IsShortHand = true;})},
            {"decompress", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "decompress"; t.Parameter = arg; t.IsShortHand = true;})},
            {"elipse", Elipse},
            {"regexreplace", RegexReplace},
            {"striphtml", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "striphtml"; t.Parameter = arg; t.IsShortHand = true;})},
            {"join",Join},
            {"format", Format},
            {"insert", Insert},
            {"insertinterval", InsertInterval},
            {"transliterate", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method="transliterate"; t.Parameter = arg; t.IsShortHand = true;})},
            {"slug", Slug},
            {"cyrtolat", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "cyrtolat"; t.Parameter = arg; t.IsShortHand = true;})},
            {"distinctwords", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "distinctwords"; t.Separator = arg; t.IsShortHand = true;})},
            {"guid", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "guid"; t.IsShortHand = true;})},
            {"now", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "now"; t.IsShortHand = true;})},
            {"remove", Remove},
            {"trimstart", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trimstart"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"trimstartappend", TrimStartAppend},
            {"trimend", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trimend"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"trim", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trim"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"substring", Substring},
            {"map", Map},
            {"urlencode", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "urlencode"; t.Parameter = arg; t.IsShortHand = true;})},
            {"web", Web},
            {"add", Add},
            {"fromjson", (arg,root) => NotImplemented("fromjson", root)},
            {"padleft", PadLeft},
            {"padright", PadRight},
            {"tostring", ToString},
            {"tolower", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "tolower"; t.Parameter = arg; t.IsShortHand = true;})},
            {"toupper", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "toupper"; t.Parameter = arg; t.IsShortHand = true;})},
            {"javascript", JavaScript},
            {"csharp", CSharp},
            {"template", Template},
            {"totitlecase", (arg, root) => root.GetDefaultOf<TflTransform>(t=>{t.Method="totitlecase"; t.Parameter = arg; t.IsShortHand = true;})},
            {"timezone",TimeZone},
            {"tojson", ToJson},
            {"fromxml", (arg,root) => NotImplemented("fromxml", root)},
            {"fromregex", (arg,root) => NotImplemented("fromregex", root)},
            {"fromsplit", (arg,root) => NotImplemented("fromsplit", root)},
            {"velocity", Velocity},
            {"tag",Tag},
            {"htmlencode", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method="htmlencode"; t.Parameter= arg; t.IsShortHand = true;})},
            {"isdaylightsavings", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method="isdaylightsavings"; t.Parameter = arg; t.IsShortHand = true;})},
            {"collapse", (arg, root) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "collapse"; t.Parameter = arg; t.IsShortHand = true;})}
        };

        private static TflTransform NotImplemented(string arg, TflField field) {
            throw new TransformalizeException(string.Empty, string.Empty, "The {0} method is not implemented as a shorthand transform.", arg);
        }

        /// <summary>
        /// Converts t attribute to transforms.
        /// </summary>
        /// <param name="f">the field</param>
        public static void ExpandShortHandTransforms(TflField f) {
            if (f.T == string.Empty)
                return;
            var list = new List<TflTransform>(Common.Split(f.T, ").").Where(t => !string.IsNullOrEmpty(t)).Select(t => Interpret(t, f)));

            foreach (var transform in f.Transforms) {
                // some transforms are shorthand transforms that need expanding
                if (transform.Method.Equals("shorthand", StringComparison.OrdinalIgnoreCase)) {
                    list.AddRange(Common.Split(transform.T, ").").Where(t => !string.IsNullOrEmpty(t)).Select(t => Interpret(t, f)));
                }
                // some transforms define new fields that may have t attributes that need expanding as well
                foreach (var field in transform.Fields) {
                    ExpandShortHandTransforms(field);
                }
                list.Add(transform);
            }
            f.Transforms = list.Where(t => !t.Method.Equals("shorthand", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Converts t attribute to configuration items for the whole process
        /// </summary>
        /// <param name="process">the process</param>
        public static void ExpandShortHandTransforms(TflProcess process) {
            foreach (var entity in process.Entities) {
                foreach (var field in entity.Fields) {
                    ExpandShortHandTransforms(field);
                }
                foreach (var field in entity.CalculatedFields) {
                    ExpandShortHandTransforms(field);
                }
            }
            foreach (var field in process.CalculatedFields) {
                ExpandShortHandTransforms(field);
            }
        }

        private static TflTransform ToJson(string arg, TflField field) {
            var element = Parameters("tojson", arg, 0, field);
            return element;
        }

        private static TflTransform Template(string arg, TflField field) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The template/razor method requires at least two paramters: a template, and a parameter.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "template";
                t.Template = split[0];
                t.IsShortHand = true;
            });

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                var skipped = split.Skip(1).ToArray();
                for (var i = 0; i < skipped.Length; i++) {
                    var s = skipped[i];
                    element.Parameters.Add(field.GetDefaultOf<TflParameter>(p => p.Field = s));
                }
            }

            return element;
        }

        private static TflTransform Velocity(string arg, TflField field) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The velocity method requires at least two paramters: a template, and a parameter.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "velocity";
                t.Template = split[0];
                t.IsShortHand = true;
            });

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                var skipped = split.Skip(1).ToArray();
                for (int i = 0; i < skipped.Length; i++) {
                    var s = skipped[i];
                    element.Parameters.Add(field.GetDefaultOf<TflParameter>(p => p.Field = s));

                }
            }

            return element;
        }

        private static TflTransform Script(string method, string arg, TflField field) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The {0} method requires at least two paramters: a script, and a parameter.", method);

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.Script = split[0];
                t.IsShortHand = true;
            });

            if (split.Length == 2) {
                element.Parameter = split[1];
            } else {
                var skipped = split.Skip(1).ToArray();
                for (var i = 0; i < skipped.Length; i++) {
                    var s = skipped[i];
                    element.Parameters.Add(field.GetDefaultOf<TflParameter>(p => p.Field = s));
                }
            }

            return element;
        }

        private static TflTransform CSharp(string arg, TflField field) {
            return Script("csharp", arg, field);
        }

        private static TflTransform JavaScript(string arg, TflField field) {
            return Script("javascript", arg, field);
        }

        private static TflTransform PadLeft(string arg, TflField field) {
            return Pad("padleft", arg, field);
        }

        private static TflTransform PadRight(string arg, TflField field) {
            return Pad("padright", arg, field);
        }

        private static TflTransform Pad(string method, string arg, TflField field) {

            Guard.Against(arg.Equals(string.Empty), "The {0} method requires two pararmeters: the total width, and the padding character(s).", method);

            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The {0} method requires two pararmeters: the total width, and the padding character(s).  You've provided {1} parameter{2}.", method, split.Length, split.Length.Plural());

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.IsShortHand = true;
            });

            int totalWidth;
            if (int.TryParse(split[0], out totalWidth)) {
                element.TotalWidth = totalWidth;
            } else {
                throw new TransformalizeException("The {0} method requires the first parameter to be total width; an integer. {1} is not an integer", method, split[0]);
            }

            element.PaddingChar = split[1][0];
            Guard.Against(element.PaddingChar == default(char), "The {0} second parameter, the padding character, must be a character.  You can't pad something with nothing.", method);

            if (split.Length > 2) {
                element.Parameter = split[2];
            }
            return element;
        }

        private static TflTransform Web(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length > 2, "The web method takes two optional parameters: a parameter referencing a field, and an integer representing sleep ms in between web requests.  You have {0} parameter{1} in '{2}'.", split.Length, split.Length.Plural(), arg);

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "web";
                t.IsShortHand = true;
            });

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

        private static TflTransform Map(string arg, TflField field) {
            Guard.Against(arg.Equals(string.Empty), "The map method requires at least one parameter; the map name.  An additional parameter may reference another field to represent the value being mapped.");
            var split = SplitComma(arg);
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "map";
                t.IsShortHand = true;
            });

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

        private static TflTransform TrimStartAppend(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The trimstartappend method requires at least one parameter indicating the trim characters.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "trimstartappend";
                t.TrimChars = split[0];
                t.IsShortHand = true;
            });

            if (split.Length > 1) {
                element.Separator = split[1];
            }

            if (split.Length > 2) {
                element.Parameter = split[2];
            }

            return element;
        }

        private static TflTransform Substring(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The substring method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "substring";
                    t.StartIndex = startIndex;
                    t.Length = length;
                    t.IsShortHand = true;
                });
            }

            throw new TransformalizeException(string.Empty, string.Empty, "The substring method requires two integers indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TflTransform Remove(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The remove method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural());

            int startIndex;
            int length;
            if (int.TryParse(split[0], out startIndex) && int.TryParse(split[1], out length)) {
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "remove";
                    t.StartIndex = startIndex;
                    t.Length = length;
                    t.IsShortHand = true;
                });
            }

            throw new TransformalizeException(string.Empty, string.Empty, "The remove method requires two integer parameters indicating start index and length. '{0}' doesn't represent two integers.", arg);
        }

        private static TflTransform Slug(string arg, TflField field) {
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "slug";
                t.IsShortHand = true;
            });

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

        private static TflTransform InsertInterval(string arg, TflField field) {
            //interval, value
            var split = SplitComma(arg);

            Guard.Against(split.Length != 2, "The insertinterval method requires two parameters: the interval (e.g. every certain number of characters), and the value to insert. '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "insertinterval";
                t.IsShortHand = true;
            });

            int interval;
            if (int.TryParse(split[0], out interval)) {
                element.Interval = interval;
            } else {
                throw new TransformalizeException(string.Empty, string.Empty, "The insertinterval method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Value = split[1];
            return element;
        }

        private static TflTransform Insert(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length != 2, "The insert method requires two parameters; the start index, and the value (or field reference) you'd like to insert.  '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "insert";
                t.IsShortHand = true;
            });

            int startIndex;
            if (int.TryParse(split[0], out startIndex)) {
                element.StartIndex = startIndex;
            } else {
                throw new TransformalizeException(string.Empty, string.Empty, "The insert method's first parameter must be an integer.  {0} is not an integer.", split[0]);
            }

            element.Parameter = split[1];
            return element;
        }

        private static TflTransform Join(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The join method requires a separator, and then a * (for all fields) or a comma delimited list of parameters that reference fields.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "join";
                t.Separator = split[0];
                t.IsShortHand = true;
            });

            if (split.Length == 2) {
                element.Parameter = split[1];
                return element;
            }

            var skipped = split.Skip(1).ToArray();
            for (var i = 0; i < skipped.Length; i++) {
                var p = skipped[i];
                element.Parameters.Add(field.GetDefaultOf<TflParameter>(x => x.Field = p));
            }

            return element;
        }

        private static TflTransform Add(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length == 0, "The add method requires a * parameter, or a comma delimited list of parameters that reference numeric fields.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "add";
                t.IsShortHand = true;
            });

            if (split.Length == 1) {
                element.Parameter = split[0];
            } else {
                for (var i = 0; i < split.Length; i++) {
                    var p = split[i];
                    element.Parameters.Add(
                        p.IsNumeric()
                            ? field.GetDefaultOf<TflParameter>(x => {
                                x.Name = p;
                                x.Value = p;
                            })
                            : field.GetDefaultOf<TflParameter>(x => {
                                x.Field = p;
                            }));
                }
            }
            return element;
        }

        private static TflTransform Parameters(string method, string arg, int skip, TflField f) {
            var split = SplitComma(arg, skip);
            Guard.Against(split.Length == 0, "The {0} method requires parameters.", method);

            var element = f.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.IsShortHand = true;
            });

            if (split.Length == 1) {
                element.Parameter = split[0];
            } else {
                for (var i = 0; i < split.Length; i++) {
                    var p = split[i];
                    element.Parameters.Add(f.GetDefaultOf<TflParameter>(x => x.Field = p));
                }
            }

            // handle single parameter that is named parameter
            if (element.Parameter.Contains(":")) {
                var pair = Common.Split(element.Parameter, ":");
                element.Parameters.Insert(0, f.GetDefaultOf<TflParameter>(x => {
                    x.Field = string.Empty;
                    x.Name = pair[0];
                    x.Value = pair[1];
                }));
                element.Parameter = string.Empty;
            }

            // handle regular parameters
            foreach (var p in element.Parameters) {
                if (!p.Field.Contains(":"))
                    continue;
                var pair = Common.Split(p.Field, ":");
                p.Field = string.Empty;
                p.Name = pair[0];
                p.Value = pair[1];
            }

            return element;
        }

        private static TflTransform Concat(string arg, TflField field) {
            return Parameters("concat", arg, 0, field);
        }

        public static TflTransform Interpret(string expression, TflField field) {

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
            return Functions[Methods[method]](arg, field);
        }

        private static TflTransform RegexReplace(string arg, TflField field) {

            Guard.Against(arg.Equals(string.Empty), "The regexreplace requires two parameters: a regular expression pattern, and replacement text.  You didn't pass in any parameters.");

            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The regexreplace method requires at least two parameters: the pattern, and the replacement text.  A third parameter, count (how many to replace) is optional. The argument '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural());

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "regexreplace";
                t.Pattern = split[0];
                t.Replacement = split[1];
                t.IsShortHand = true;
            });

            if (split.Length <= 2)
                return element;

            int count;
            Guard.Against(!int.TryParse(split[2], out count), "The regexreplace's third parameter; count, must be an integer. The argument '{0}' contains '{1}'.", arg, split[2]);
            element.Count = count;

            return element;
        }

        private static TflTransform Replace(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 2, "The replace method requires two parameters: an old value, and a new value. Your arguments '{0}' resolve {1} parameter{2}.", arg, split.Length, split.Length.Plural());
            var oldValue = split[0];
            var newValue = split[1];

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "replace";
                t.OldValue = oldValue;
                t.NewValue = newValue;
                t.IsShortHand = true;
            });
        }

        private static TflTransform Convert(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The convert and tostring methods require the first parameter reference another field's alias (or name).");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "convert";
                t.Parameter = split[0];
                t.IsShortHand = true;
            });

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

        private static TflTransform ToString(string arg, TflField field) {
            var element = Convert(arg, field);
            element.Method = "tostring";
            element.To = "string";
            return element;
        }

        private static TflTransform If(string arg, TflField field) {
            var linked = new LinkedList<string>(SplitComma(arg));

            Guard.Against(linked.Count < 2, "The if method requires at least 2 arguments. Your argument '{0}' has {1}.", arg, linked.Count);

            // left is required first, assign and remove
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "if";
                t.Left = linked.First.Value;
                t.IsShortHand = true;
            });
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

        private static TflTransform Right(string arg, TflField field) {
            int length;
            Guard.Against(!int.TryParse(arg, out length), "The right method requires a single integer representing the length, or how many right-most characters you want. You passed in '{0}'.", arg);
            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "right";
                t.Length = length;
                t.IsShortHand = true;
            });
        }

        private static TflTransform Left(string arg, TflField field) {
            int length;
            Guard.Against(!int.TryParse(arg, out length), "The left method requires a single integer representing the length, or how many left-most characters you want. You passed in '{0}'.", arg);
            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "left";
                t.Length = length;
                t.IsShortHand = true;
            });
        }

        private static string[] SplitComma(string arg, int skip = 0) {
            return Common.Split(arg, ",", skip);
        }

        private static TflTransform Elipse(string arg, TflField field) {
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "elipse";
                t.IsShortHand = true;
            });

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

        private static TflTransform Format(string arg, TflField field) {
            var split = SplitComma(arg);
            Guard.Against(split.Length < 1, "The format method requires at least one parameter; the format with {{0}} style place-holders in it.  For each place-holder, add additional parameters that reference fields.  If no fields are referenced, the first parameter is assumed to be the field this transform is nested in.");
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "format";
                t.Format = split[0];
                t.IsShortHand = true;
            });

            if (split.Length <= 1)
                return element;

            if (split.Length == 2) {
                element.Parameter = split[1];
                return element;
            }

            var skipped = split.Skip(1).ToArray();
            for (int i = 0; i < skipped.Length; i++) {
                var s = skipped[i];
                element.Parameters.Add(field.GetDefaultOf<TflParameter>(p => p.Field = s));
            }
            return element;
        }

        private static TflTransform TimeZone(string arg, TflField field) {
            var split = SplitComma(arg);

            Guard.Against(split.Length < 2, "The timezone method requires at least two parameters: the from-time-zone, and the to-time-zone.");

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "timezone";
                t.IsShortHand = true;
            });

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

        private static TflTransform Tag(string arg, TflField field) {
            var split = SplitComma(arg).ToList();
            Guard.Against(split.Count < 2, "The tag method requires at least 2 parameters: the tag (aka element name), and a parameter (which becomes an attribute of the tag/element).  With {0}, You passed in {1} parameter{2}.", arg, split.Count, split.Count.Plural());
            var element = Parameters("tag", arg, 1, field);
            element.Tag = split[0];
            return element;
        }
    }
}