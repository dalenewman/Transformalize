using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Transform {

    public class ShortHandFactory {

        private readonly char[] _codeCharaters = { ' ', '(', ')', ';', ',', '{', '}', '+', '*', '-' };
        private readonly TflProcess _process;
        private readonly Dictionary<string, string> _methods;
        private readonly Dictionary<string, Func<string, TflField, TflTransform, TflTransform>> _functions;
        private readonly Dictionary<string, byte> _maps;
        private readonly List<string> _problems = new List<string>();
        private readonly TflTransform _guard;

        public ShortHandFactory(TflProcess process) {
            _process = process;
            _maps = process.Maps.ToDictionary(m => m.Name, m => default(byte));
            _guard = process.GetDefaultOf<TflTransform>(t => { t.Method = "guard"; });
            _methods = new Dictionary<string, string> {
            {"add", "add"},
            {"append","append"},
            {"collapse","collapse"},
            {"compress","compress"},
            {"concat","concat"},
            {"convert","convert"},
            {"copy","copy"},
            {"cs","csharp"},
            {"csharp","csharp"},
            {"cyrtolat","cyrtolat"},
            {"decompress","decompress"},
            {"datepart","datepart"},
            {"distinctwords","distinctwords"},
            {"elipse","elipse"},
            {"format","format"},
            {"formatphone","formatphone"},
            {"fromjson","fromjson"},
            {"fromregex", "fromregex"},
            {"fromsplit","fromsplit"},
            {"fromxml","fromxml"},
            {"gethashcode","hashcode"},
            {"guid","guid"},
            {"hashcode","hashcode"},
            {"htmlencode","htmlencode"},
            {"if","if"},
            {"iif","if"},
            {"insert","insert"},
            {"insertinterval","insertinterval"},
            {"isdaylightsavings","isdaylightsavings"},
            {"javascript","javascript"},
            {"join","join"},
            {"js","javascript"},
            {"left","left"},
            {"lower","tolower"},
            {"map","map"},
            {"now","now"},
            {"utcnow","utcnow"},
            {"padleft","padleft"},
            {"padright","padright"},
            {"phone","formatphone"},
            {"razor","template"},
            {"regexreplace","regexreplace"},
            {"remove","remove"},
            {"replace","replace"},
            {"right","right"},
            {"slug","slug"},
            {"slugify","slug"},
            {"striphtml","striphtml"},
            {"substr","substring"},
            {"substring","substring"},
            {"sum", "add"},
            {"tag","tag"},
            {"template","template"},
            {"timezone","timezone"},
            {"titlecase","totitlecase"},
            {"tojson","tojson"},
            {"toint","toint"},
            {"toint32","toint"},
            {"tolower","tolower"},
            {"tostring","tostring"},
            {"totitlecase","totitlecase"},
            {"toupper","toupper"},
            {"transliterate","transliterate"},
            {"trim","trim"},
            {"trimend","trimend"},
            {"trimstart","trimstart"},
            {"trimstartappend","trimstartappend"},
            {"upper","toupper"},
            {"urlencode","urlencode"},
            {"velocity","velocity"},
            {"web","web"},
            {"timespan","timespan"},
            {"isempty","isempty"},
            {"isblank","isempty"},
            {"equals","equals"},
            {"isequalto", "equals"},
            {"xmlencode","xmlencode"},
            {"xmldecode","htmldecode"},
            {"htmldecode", "htmldecode"},
            {"timeago","timeago"}
        };
            _functions = new Dictionary<string, Func<string, TflField, TflTransform, TflTransform>> {
            {"replace", Replace},
            {"left", Left},
            {"right", Right},
            {"append", Append}, 
            {"if", If},
            {"convert", Convert},
            {"copy", Copy},
            {"concat", Concat},
            {"hashcode", GetHashCode},
            {"compress", Compress},
            {"datepart", DatePart},
            {"decompress", Decompress},
            {"elipse", Elipse},
            {"regexreplace", RegexReplace},
            {"striphtml", StripHtml},
            {"join", Join},
            {"format", Format},
            {"insert", Insert},
            {"insertinterval", InsertInterval},
            {"transliterate", Transliterate},
            {"slug", Slug},
            {"cyrtolat", CyrToLat},
            {"distinctwords", DistinctWords},
            {"guid", (arg, root, last) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "guid"; t.IsShortHand = true;})},
            {"now", (arg, root, last) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "now"; t.IsShortHand = true;})},
            {"utcnow", (arg, root, last) =>root.GetDefaultOf<TflTransform>(t=>{t.Method = "utcnow"; t.IsShortHand = true;})},
            {"remove", Remove},
            {"trimstart", (arg, root, last) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trimstart"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"trimstartappend", TrimStartAppend},
            {"trimend", (arg, root, last) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trimend"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"trim", (arg, root, last) => root.GetDefaultOf<TflTransform>(t=>{t.Method = "trim"; t.TrimChars = arg; t.IsShortHand = true;})},
            {"substring", Substring},
            {"map", Map},
            {"urlencode", UrlEncode},
            {"xmlencode", XmlEncode},
            {"htmldecode", HtmlDecode},
            {"web", Web},
            {"add", Add},
            {"fromjson", (arg,root, last) => NotImplemented("fromjson", root, last)},
            {"padleft", PadLeft},
            {"padright", PadRight},
            {"tostring", ToString},
            {"formatphone", FormatPhone},
            {"tolower", ToLower},
            {"toupper", ToUpper},
            {"javascript", JavaScript},
            {"csharp", CSharp},
            {"template", Template},
            {"totitlecase", ToTitleCase},
            {"timezone", TimeZone},
            {"toint", ToInt},
            {"tojson", ToJson},
            {"fromxml", (arg,root, last) => NotImplemented("fromxml", root, last)},
            {"fromregex", (arg,root, last) => NotImplemented("fromregex", root, last)},
            {"fromsplit", (arg,root, last) => NotImplemented("fromsplit", root, last)},
            {"velocity", Velocity},
            {"tag", Tag},
            {"htmlencode", HtmlEncode},
            {"isdaylightsavings", IsDaylightSavings},
            {"collapse", Collapse},
            {"timespan", Timespan},
            {"isempty", IsEmpty},
            {"equals", Equals},
            {"timeago", TimeAgo}
        };
        }

        private TflTransform TimeAgo(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);
            if (split.Length == 0) {
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "timeago";
                    t.FromTimeZone = "UTC";
                    t.IsShortHand = true;
                });
            }

            var p = split[0];
            try {
                TimeZoneInfo.FindSystemTimeZoneById(p);
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "timeago";
                    t.FromTimeZone = p;
                    t.IsShortHand = true;
                });
            } catch (Exception ex) {
                var message = string.Format("TimeAgo from time zone is {0} is invalid. {1}", p, ex.Message);
                _problems.Add(message);
                return _guard;
            }
        }

        private TflTransform IsEmpty(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("isempty", "(true if empty, false if not empty)", arg, field, lastTransform);
        }

        private TflTransform Equals(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);
            if (Guard.Against(_problems, split.Length == 0 && lastTransform.Method != "copy" || split.Length > 1,
                "The equals method requires a parameter representing a value or field that you want compared with this field's value OR should be proceeded by a copy transform that supplies multiple parameters.")) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "equals";
                t.Parameter = arg;
                t.IsShortHand = true;
            });
        }

        private TflTransform Timespan(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("timespan", "timespan", arg, field, lastTransform);
        }

        private TflTransform Append(string arg, TflField field, TflTransform lastTransform) {
            if (Guard.Against(_problems, string.IsNullOrEmpty(arg),
                "The append method requires a single argument representing a field or a value that you want to append to this field.")) {
                return _guard;
            }
            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "append";
                t.Parameter = arg;
                t.IsShortHand = true;
            });
        }

        private TflTransform Copy(string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, string.IsNullOrEmpty(arg),
                "The copy method requires one or more parameters representing fields that you want to copy to this field.  A single field produces a single value, multiple fields produces an array or values.")) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "copy";
                t.IsShortHand = true;
            });

            var split = SplitComma(arg);

            if (split.Length == 1) {
                element.Parameter = split[0];
                return element;
            }

            foreach (var p in split) {
                var parameter = field.GetDefaultOf<TflParameter>();
                if (p.Contains(":")) {  //named values
                    var named = p.Split(':');
                    parameter.Name = named[0];
                    parameter.Value = named[1];
                } else if (p.Contains(".")) { // entity, field combinations
                    var dotted = p.Split('.');
                    parameter.Entity = dotted[0];
                    parameter.Field = dotted[1];
                } else {
                    parameter.Field = p; // just fields
                }
                element.Parameters.Add(parameter);
            }

            return element;
        }

        private TflTransform NotImplemented(string arg, TflField field, TflTransform lastTransform) {
            _problems.Add(string.Format("The {0} method is not implemented as a shorthand transform.", arg));
            return _guard;
        }

        /// <summary>
        /// Converts t attribute to transforms.
        /// </summary>
        /// <param name="f">the field</param>
        public void ExpandShortHandTransforms(TflField f) {

            var list = new LinkedList<TflTransform>();
            var lastTransform = new TflTransform();
            var methods = f.T == string.Empty ? new String[0] : Common.Split(f.T, ").").Where(t => !string.IsNullOrEmpty(t));

            foreach (var method in methods) {
                // translate previous copy to parameters, making them compatible with verbose xml transforms
                if (lastTransform.Method == "copy") {
                    var tempParameter = lastTransform.Parameter;
                    var tempParameters = lastTransform.Parameters;
                    var transform = lastTransform = Interpret(method, f, lastTransform);
                    transform.Parameter = tempParameter;
                    transform.Parameters = tempParameters;
                    list.RemoveLast(); //remove previous copy
                    list.AddLast(transform);
                } else {
                    lastTransform = Interpret(method, f, lastTransform);
                    list.AddLast(lastTransform);
                }
            }

            foreach (var transform in f.Transforms) {
                // expand shorthand transforms
                if (transform.Method.Equals("t") || transform.Method.Equals("shorthand")) {
                    var tempField = new TflField { T = transform.T };
                    ExpandShortHandTransforms(tempField);
                    foreach (var t in tempField.Transforms) {
                        list.AddLast(t);
                    }
                } else {
                    list.AddLast(transform);
                }
            }

            f.Transforms = list.Where(t => t.Method != "guard").ToList();
        }

        /// <summary>
        /// Converts t attribute to configuration items for the whole process, and returns any problems found
        /// </summary>
        public string[] ExpandShortHandTransforms() {
            foreach (var entity in _process.Entities) {
                foreach (var field in entity.Fields) {
                    ExpandShortHandTransforms(field);
                }
                foreach (var field in entity.CalculatedFields) {
                    ExpandShortHandTransforms(field);
                }
            }
            foreach (var field in _process.CalculatedFields) {
                ExpandShortHandTransforms(field);
            }
            return _problems.ToArray();
        }

        private TflTransform ToJson(string arg, TflField field, TflTransform lastTransform) {
            var element = Parameters("tojson", arg, 0, field, lastTransform);
            return element;
        }

        private TflTransform Template(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 1 || split.Length > 2,
                "The template/razor method takes between 1 and 2 parameters: the template name and a snippet (of template). Use the copy method to inject other data into the template.")) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "template";
                t.IsShortHand = true;
            });

            foreach (var parameter in split) {
                if (parameter.IndexOfAny(_codeCharaters) > -1) {
                    element.Template = parameter;
                } else {
                    element.Templates = new List<TflNameReference> { new TflNameReference { Name = parameter } };
                }
            }

            return element;
        }

        private TflTransform Velocity(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 1 || split.Length > 2,
                "The velocity method takes between 1 and 2 parameters: the template name, and a snippet (of template). Use the copy method to inject other data into the template.")) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "velocity";
                t.IsShortHand = true;
            });

            foreach (var parameter in split) {
                if (parameter.IndexOfAny(_codeCharaters) > -1) {
                    element.Template = parameter;
                } else {
                    element.Templates = new List<TflNameReference> { new TflNameReference { Name = parameter } };
                }
            }

            return element;
        }

        private TflTransform Script(string method, string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 1 || split.Length > 2,
                "The {0} method takes between 1 and 2 parameters: a script name and/or a snippet (e.g. a function call).",
                method)) {
                return _guard;
            };

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.IsShortHand = true;
            });

            foreach (var parameter in split) {
                if (parameter.IndexOfAny(_codeCharaters) > -1) {
                    element.Script = parameter;
                } else {
                    element.Scripts = new List<TflNameReference> { new TflNameReference { Name = parameter } };
                }
            }

            return element;
        }

        private TflTransform CSharp(string arg, TflField field, TflTransform lastTransform) {
            return Script("csharp", arg, field, lastTransform);
        }

        private TflTransform JavaScript(string arg, TflField field, TflTransform lastTransform) {
            return Script("javascript", arg, field, lastTransform);
        }

        private TflTransform PadLeft(string arg, TflField field, TflTransform lastTransform) {
            return Pad("padleft", arg, field, lastTransform);
        }

        private TflTransform PadRight(string arg, TflField field, TflTransform lastTransform) {
            return Pad("padright", arg, field, lastTransform);
        }

        private TflTransform Pad(string method, string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, arg.Equals(string.Empty),
                "The {0} method requires two pararmeters: the total width, and the padding character(s).", method)) {
                return _guard;
            }

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 2,
                "The {0} method requires two pararmeters: the total width, and the padding character(s).  You've provided {1} parameter{2}.",
                method, split.Length, split.Length.Plural())) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.IsShortHand = true;
            });

            int totalWidth;
            if (int.TryParse(split[0], out totalWidth)) {
                element.TotalWidth = totalWidth;
            } else {
                _problems.Add(string.Format("The {0} method requires the first parameter to be total width; an integer. {1} is not an integer", method, split[0]));
                return _guard;
            }

            element.PaddingChar = split[1][0];

            if (Guard.Against(_problems, element.PaddingChar == default(char),
                "The {0} second parameter, the padding character, must be a character.  You can't pad something with nothing.", method)) {
                return _guard;
            }

            if (split.Length > 2) {
                element.Parameter = split[2];
            }
            return element;
        }

        private TflTransform Web(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);
            if (Guard.Against(_problems, split.Length > 2,
                "The web method takes two optional parameters: a parameter referencing a field, and an integer representing sleep ms in between web requests.  You have {0} parameter{1} in '{2}'.",
                split.Length, split.Length.Plural(), arg)) {
                return _guard;
            }

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

        private TflTransform Map(string arg, TflField field, TflTransform lastTransform) {

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length == 0,
                "The map method requires a map name (e.g. 'map'), or a set of parameters that represent an inline map (e.g. 'a=1,b=2,c=3').")) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "map";
                t.IsShortHand = true;
            });

            if (_maps.ContainsKey(split[0])) {
                element.Map = split[0];
                Guard.Against(_problems, split.Length > 1, "If you reference a map name in a map method, that's the only parameter you can have.  It will map what is in the current field.");
                return element;
            }

            element.Map = string.Join(",", split);

            return element;
        }

        private TflTransform TrimStartAppend(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 1,
                "The trimstartappend method requires at least one parameter indicating the trim characters.")) {
                return _guard;
            }

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

        private TflTransform Substring(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 1, "The substring method requires a start index.")) {
                return _guard;
            }

            int startIndex;
            if (int.TryParse(split[0], out startIndex)) {
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "substring";
                    t.StartIndex = startIndex;
                    int length;
                    t.Length = split.Length > 1 && int.TryParse(split[1], out length) ? length : 0;
                    t.IsShortHand = true;
                });
            }

            _problems.Add(string.Format("The substring method requires two integers indicating start index and length. '{0}' doesn't represent two integers.", arg));
            return _guard;
        }

        private TflTransform Remove(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 2, "The remove method requires start index and length. You have {0} parameter{1}.", split.Length, split.Length.Plural())) {
                return _guard;
            }

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

            _problems.Add(string.Format("The remove method requires two integer parameters indicating start index and length. '{0}' doesn't represent two integers.", arg));
            return _guard;
        }

        private TflTransform Slug(string arg, TflField field, TflTransform lastTransform) {
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "slug";
                t.IsShortHand = true;
            });

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length > 1,
                "The slug method takes 1 argument; an integer representing the maximum length of the slug. You passed in '{0}' arguments.",
                split.Length)) {
                return _guard;
            }

            int length;
            if (int.TryParse(split[0], out length)) {
                element.Length = length;
            }

            return Guard.Against(_problems, length == 0,
                "The slug method takes 1 argument; an integer greater than zero, representing the maximum length of the slug. You passed in '{0}'.",
                arg) ? _guard : element;
        }

        private TflTransform InsertInterval(string arg, TflField field, TflTransform lastTransform) {
            //interval, value
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length != 2, "The insertinterval method requires two parameters: the interval (e.g. every certain number of characters), and the value to insert. '{0}' has {1} parameter{2}.", arg, split.Length, split.Length.Plural())) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "insertinterval";
                t.IsShortHand = true;
            });

            int interval;
            if (int.TryParse(split[0], out interval)) {
                element.Interval = interval;
            } else {
                _problems.Add(string.Format("The insertinterval method's first parameter must be an integer.  {0} is not an integer.", split[0]));
                return _guard;
            }

            element.Value = split[1];
            return element;
        }

        private TflTransform Insert(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length != 2,
                "The insert method requires two parameters; the start index, and the value (or field reference) you'd like to insert.  '{0}' has {1} parameter{2}.",
                arg, split.Length, split.Length.Plural())) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "insert";
                t.IsShortHand = true;
            });

            int startIndex;
            if (int.TryParse(split[0], out startIndex)) {
                element.StartIndex = startIndex;
            } else {
                _problems.Add(string.Format("The insert method's first parameter must be an integer.  {0} is not an integer.", split[0]));
                return _guard;
            }

            element.Parameter = split[1];
            return element;
        }

        private TflTransform Join(string arg, TflField field, TflTransform lastTransform) {

            if (string.IsNullOrEmpty(arg)) {
                return field.GetDefaultOf<TflTransform>(t => {
                    t.Method = "concat";
                    t.IsShortHand = true;
                });
            }

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length != 1,
                "The join method requires one parameter; the separator. To get fields for join, use the copy() method.")) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "join";
                t.Separator = split[0];
                t.IsShortHand = true;
            });

        }

        private TflTransform Add(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length == 0,
                "The add method requires a * parameter, or a comma delimited list of parameters that reference numeric fields.")) {
                return _guard;
            }

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

        private TflTransform Parameters(string method, string arg, int skip, TflField f, TflTransform lastTransform) {
            var split = SplitComma(arg, skip);

            if (Guard.Against(_problems, split.Length == 0, "The {0} method requires parameters.", method)) {
                return _guard;
            }

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

        private TflTransform Concat(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("concat", "concatenated", arg, field, lastTransform);
        }

        public TflTransform Interpret(string expression, TflField field, TflTransform lastTransform = null) {

            string method;
            var arg = string.Empty;

            if (Guard.Against(_problems, expression == null, "You may not pass a null expression.")) {
                return _guard;
            }

            // ReSharper disable once PossibleNullReferenceException
            if (expression.Contains("(")) {
                var index = expression.IndexOf('(');
                method = expression.Left(index).ToLower();
                arg = expression.Remove(0, index + 1).TrimEnd(new[] { ')' });
            } else {
                method = expression;
            }

            if (Guard.Against(_problems, !_methods.ContainsKey(method),
                "Sorry. Your expression '{0}' references an undefined method: '{1}'.", expression, method)) {
                return _guard;
            }

            return _functions[_methods[method]](arg, field, lastTransform);
        }

        private TflTransform RegexReplace(string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, arg.Equals(string.Empty),
                "The regexreplace requires two parameters: a regular expression pattern, and replacement text.  You didn't pass in any parameters.")) {
                return _guard;
            };

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 2,
                "The regexreplace method requires at least two parameters: the pattern, and the replacement text.  A third parameter, count (how many to replace) is optional. The argument '{0}' has {1} parameter{2}.",
                arg, split.Length, split.Length.Plural())) {
                return _guard;
            }

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "regexreplace";
                t.Pattern = split[0];
                t.Replacement = split[1];
                t.IsShortHand = true;
            });

            if (split.Length <= 2)
                return element;

            int count;
            if (Guard.Against(_problems, !int.TryParse(split[2], out count),
                "The regexreplace's third parameter; count, must be an integer. The argument '{0}' contains '{1}'.", arg,
                split[2])) {
                return _guard;
            }
            element.Count = count;

            return element;
        }

        private TflTransform Replace(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 2,
                "The replace method requires two parameters: an old value, and a new value. Your arguments '{0}' resolve {1} parameter{2}.",
                arg, split.Length, split.Length.Plural())) {
                return _guard;
            }

            var oldValue = split[0];
            var newValue = split[1];

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "replace";
                t.OldValue = oldValue;
                t.NewValue = newValue;
                t.IsShortHand = true;
            });
        }

        private TflTransform Convert(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "convert";
                t.IsShortHand = true;
            });

            if (split.Length == 0)
                return element;

            foreach (var p in split) {
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

        private TflTransform ToString(string arg, TflField field, TflTransform lastTransform) {
            var element = Convert(arg, field, lastTransform);
            element.Method = "tostring";
            element.To = "string";
            return element;
        }

        private TflTransform ToInt(string arg, TflField field, TflTransform lastTransform) {
            var element = Convert(arg, field, lastTransform);
            element.Method = "convert";
            element.To = "int";
            return element;
        }

        private TflTransform If(string arg, TflField field, TflTransform lastTransform) {
            var linked = new LinkedList<string>(SplitComma(arg));

            if (Guard.Against(_problems, linked.Count < 2,
                "The if method requires at least 2 arguments. Your argument '{0}' has {1}.", arg, linked.Count)) {
                return _guard;
            }

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

        private TflTransform Right(string arg, TflField field, TflTransform lastTransform) {
            int length;

            if (Guard.Against(_problems, !int.TryParse(arg, out length),
                "The right method requires a single integer representing the length, or how many right-most characters you want. You passed in '{0}'.",
                arg)) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "right";
                t.Length = length;
                t.IsShortHand = true;
            });
        }

        private TflTransform Left(string arg, TflField field, TflTransform lastTransform) {
            int length;
            if (Guard.Against(_problems, !int.TryParse(arg, out length),
                "The left method requires a single integer representing the length, or how many left-most characters you want. You passed in '{0}'.",
                arg)) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "left";
                t.Length = length;
                t.IsShortHand = true;
            });
        }

        private TflTransform GetHashCode(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("hashcode", "integer hash code", arg, field, lastTransform);
        }

        private TflTransform Compress(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("compress", "compressed", arg, field, lastTransform);
        }

        private TflTransform UrlEncode(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("urlencode", "url-encoded", arg, field, lastTransform);
        }

        private TflTransform XmlEncode(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("xmlencode", "xml-encoded", arg, field, lastTransform);
        }

        private TflTransform HtmlDecode(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("htmldecode", "html-decoded", arg, field, lastTransform);
        }

        private TflTransform HtmlEncode(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("htmlencode", "html-encoded", arg, field, lastTransform);
        }

        private TflTransform Collapse(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("collapse", "white-space collapsed", arg, field, lastTransform);
        }

        private TflTransform IsDaylightSavings(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("isdaylightsavings", "boolean indicating whether or not it is daylight savings time", arg, field, lastTransform);
        }

        private TflTransform StripHtml(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("striphtml", "html-less", arg, field, lastTransform);
        }

        private TflTransform Transliterate(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("transliterate", "transliterated", arg, field, lastTransform);
        }

        private TflTransform ToUpper(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("toupper", "upper-cased", arg, field, lastTransform);
        }

        private TflTransform ToTitleCase(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("totitlecase", "title-cased", arg, field, lastTransform);
        }

        private TflTransform ToLower(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("tolower", "lower-cased", arg, field, lastTransform);
        }

        private TflTransform FormatPhone(string arg, TflField field, TflTransform lastTransform) {
            return Parameterless("formatphone", "phone number formatted", arg, field, lastTransform);
        }

        private TflTransform Parameterless(string method, string result, string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, !string.IsNullOrEmpty(arg),
                "{0}() does not take parameters.  It returns a {1} version of the value or values in the field. To get data into the field, proceed {0}() with copy(f1) or copy(f1,f2,etc) short-hand method.",
                method, result)) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = method;
                t.IsShortHand = true;
            });
        }
        private TflTransform CyrToLat(string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, !string.IsNullOrEmpty(arg),
                "The cyrtolat method does not take parameters.  It will return a latin version of whatever is currently in the field.")) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "cyrtolat";
                t.IsShortHand = true;
            });
        }

        private TflTransform DistinctWords(string arg, TflField field, TflTransform lastTransform) {

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length != 1,
                "The distinctwords method takes 1 parameter.  You provided {0}.", split.Length)) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "distinctwords";
                t.Separator = split[0];
                t.IsShortHand = true;
            });
        }

        private TflTransform Decompress(string arg, TflField field, TflTransform lastTransform) {

            if (Guard.Against(_problems, !string.IsNullOrEmpty(arg),
                "The decompress method does not take parameters.  It will return a decompressed version of whatever is currently in the field.")) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "decompress";
                t.IsShortHand = true;
            });
        }

        private static string[] SplitComma(string arg, int skip = 0) {
            return Common.Split(arg, ",", skip);
        }

        private TflTransform DatePart(string arg, TflField field, TflTransform lastTransform) {

            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "datepart";
                t.IsShortHand = true;
            });

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length == 0 || split.Length > 1,
                "The datepart method requires one parameter representing a time component.")) {
                return _guard;
            }

            var timeComponent = split[0].ToLower();

            if (Guard.Against(_problems, !DatePartOperation.Parts.ContainsKey(timeComponent),
                "The datepart method requires one of these time components: {0}",
                string.Join(", ", DatePartOperation.Parts.Select(kv => kv.Key)))) {
                return _guard;
            }

            element.TimeComponent = timeComponent;

            return element;
        }

        private TflTransform Elipse(string arg, TflField field, TflTransform lastTransform) {
            var element = field.GetDefaultOf<TflTransform>(t => {
                t.Method = "elipse";
                t.IsShortHand = true;
            });

            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length == 0,
                "The elipse method requires a an integer representing the number of characters allowed before the elipse.")) {
                return _guard;
            }

            int length;

            if (Guard.Against(_problems, !int.TryParse(split[0], out length),
                "The elipse method requires a an integer representing the number of characters allowed before the elipse. You passed in '{0}'.",
                split[0])) {
                return _guard;
            }
            element.Length = length;

            if (split.Length > 1) {
                element.Elipse = split[1];
            }
            return element;
        }

        private TflTransform Format(string arg, TflField field, TflTransform lastTransform) {

            var split = SplitComma(arg);
            if (Guard.Against(_problems, split.Length != 1,
                "format() takes one parameter: the format with {{index}} style place-holders in it. To get data for format, use the copy() method.")) {
                return _guard;
            }

            if (Guard.Against(_problems, !split[0].Contains("{"),
                "A format place-holder is required.  There are not left brackets {.")) {
                return _guard;
            }

            if (Guard.Against(_problems, !split[0].Contains("}"),
                "A format place-holder is required.  There are not right brackets }.")) {
                return _guard;
            }

            return field.GetDefaultOf<TflTransform>(t => {
                t.Method = "format";
                t.Format = split[0];
                t.IsShortHand = true;
            });

        }

        private TflTransform TimeZone(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg);

            if (Guard.Against(_problems, split.Length < 2,
                "The timezone method requires at least two parameters: the from-time-zone, and the to-time-zone.")) {
                return _guard;
            }

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
                        _problems.Add(string.Format("The timezone method already has a parameter of {0}, and it can't interpret {1} as a valid time-zone identifer. {2}", element.Parameter, p, ex.Message));
                        return _guard;
                    }
                }
            }
            return element;
        }

        private TflTransform Tag(string arg, TflField field, TflTransform lastTransform) {
            var split = SplitComma(arg).ToList();

            if (Guard.Against(_problems, split.Count < 2,
                "The tag method requires at least 2 parameters: the tag (aka element name), and a parameter (which becomes an attribute of the tag/element).  With {0}, You passed in {1} parameter{2}.",
                arg, split.Count, split.Count.Plural())) {
                return _guard;
            }

            var element = Parameters("tag", arg, 1, field, lastTransform);
            element.Tag = split[0];
            return element;
        }
    }
}