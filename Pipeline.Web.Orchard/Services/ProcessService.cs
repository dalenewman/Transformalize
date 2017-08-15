using Cfg.Net.Environment;
using Cfg.Net.Parsers;
using Cfg.Net.Serializers;
using Cfg.Net.Shorthand;
using Orchard;
using Transformalize.Configuration;
using Pipeline.Web.Orchard.Models;
using Transformalize.Transform.DateMath;
using Transformalize.Transform.Jint;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Pipeline.Web.Orchard.Services {

    public interface IProcessService : IDependency {
        Process Resolve(PipelineConfigurationPart part, string input, string output);
        Process Resolve(PipelineConfigurationPart part);
    }

    public class ProcessService : IProcessService {

        public ILogger Logger { get; set; }
        private readonly object _lock = new object();
        public static Cfg.Net.Contracts.IDependency _shortHandCustomizer;

        public ProcessService() {
            Logger = NullLogger.Instance;
            lock (_lock) {
                if (_shortHandCustomizer == null) {
                    _shortHandCustomizer = BuildShortHandCustomizer();
                }
            }
        }

        public Process Resolve(PipelineConfigurationPart part, string input, string output) {
            var marker = part.PlaceHolderStyle[0];
            var prefix = part.PlaceHolderStyle[1];
            var suffix = part.PlaceHolderStyle[2];
            var modifier = new EnvironmentModifier(new PlaceHolderReplacer(marker, prefix, suffix), "environments", "environment", "name", "parameters", "name", "value");

            switch (input) {
                case "json":
                    switch (output) {
                        case "json":
                            return new Process(
                                new DateMathModifier(),
                                new FastJsonParser(),
                                new JsonSerializer(),
                                new JintValidator(),
                                _shortHandCustomizer,
                                modifier
                            );
                        default:  // xml
                            return new Process(
                                new DateMathModifier(),
                                new FastJsonParser(),
                                new XmlSerializer(),
                                new JintValidator(),
                                _shortHandCustomizer,
                                modifier
                            );
                    }
                default:
                    switch (output) {
                        case "json":
                            return new Process(
                                new DateMathModifier(),
                                new NanoXmlParser(),
                                new JsonSerializer(),
                                new JintValidator(),
                                _shortHandCustomizer,
                                modifier
                            );
                        default: // xml
                            return new Process(
                                new DateMathModifier(),
                                new NanoXmlParser(),
                                new XmlSerializer(),
                                new JintValidator(),
                                _shortHandCustomizer,
                                modifier
                            );
                    }
            }
        }

        public Process Resolve(PipelineConfigurationPart part) {
            return Resolve(part, part.EditorMode, part.EditorMode);
        }

        private ShorthandCustomizer BuildShortHandCustomizer() {
            var root = new ShorthandRoot();
            root.Signatures.Add(new Signature() { Name = "none" });
            root.Signatures.Add(Simple("format"));
            root.Signatures.Add(Simple("length"));
            root.Signatures.Add(Simple("separator", ","));
            root.Signatures.Add(new Signature() {
                Name = "separator-space",
                Parameters = new List<Parameter> { new Parameter { Name = "separator", Value = " " } }
            });
            root.Signatures.Add(new Signature() {
                Name = "padding",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "total-width"},
                    new Parameter {Name = "padding-char", Value = "0"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "timezone",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "from-time-zone"},
                    new Parameter {Name = "to-time-zone"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "fromtimezone",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "from-time-zone", Value = "UTC"}
                }
            });
            root.Signatures.Add(Simple("value", "[default]"));
            root.Signatures.Add(Simple("type", "[default]"));
            root.Signatures.Add(new Signature() {
                Name = "trim",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "trim-chars", Value = " "}
                }
            });
            root.Signatures.Add(Simple("script"));
            root.Signatures.Add(Simple("map"));
            root.Signatures.Add(Simple("dayofweek"));
            root.Signatures.Add(new Signature() {
                Name = "substring",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "start-index"},
                    new Parameter {Name = "length", Value = "0"}
                }
            });
            root.Signatures.Add(Simple("timecomponent"));
            root.Signatures.Add(new Signature {
                Name = "dateadd",
                Parameters = new List<Cfg.Net.Shorthand.Parameter>
                {
                    new Cfg.Net.Shorthand.Parameter { Name = "value" },
                    new Cfg.Net.Shorthand.Parameter { Name = "timecomponent", Value="days" }
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "replace",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "old-value"},
                    new Parameter {Name = "new-value", Value = ""}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "regexreplace",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "pattern"},
                    new Parameter {Name = "new-value"},
                    new Parameter {Name = "count", Value = "0"}
                }
            });
            root.Signatures.Add(Simple("pattern"));
            root.Signatures.Add(new Signature() {
                Name = "insert",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "start-index"},
                    new Parameter {Name = "value"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "remove",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "start-index"},
                    new Parameter {Name = "count", Value = "0"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "template",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "template"},
                    new Parameter {Name = "content-type", Value = "raw"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "any",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "value"},
                    new Parameter {Name = "operator", Value = "equal"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "property",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "name"},
                    new Parameter {Name = "property"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "file",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "extension", Value = "true"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "xpath",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "xpath"},
                    new Parameter {Name = "name-space", Value = ""},
                    new Parameter {Name = "url", Value = ""}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "datediff",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "time-component"},
                    new Parameter {Name = "from-time-zone", Value = "UTC"}
                }
            });
            root.Signatures.Add(Simple("domain"));
            root.Signatures.Add(new Signature() {
                Name = "tag",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "tag"},
                    new Parameter {Name = "class", Value = ""},
                    new Parameter {Name = "style", Value = ""},
                    new Parameter {Name = "title", Value = ""},
                    new Parameter {Name = "href", Value = ""},
                    new Parameter {Name = "role", Value = ""},
                    new Parameter {Name = "target", Value = ""},
                    new Parameter {Name = "body", Value = ""},
                    new Parameter {Name = "encode", Value = "true"}
                }
            });
            root.Signatures.Add(new Signature() {
                Name = "decimals",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "decimals", Value = "0"}
                }
            });
            root.Signatures.Add(new Signature {
                Name = "iif",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "expression"},
                    new Parameter {Name = "true-field"},
                    new Parameter {Name = "false-field"}
                }
            });
            root.Signatures.Add(new Signature {
                Name = "geohash",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "latitude"},
                    new Parameter {Name = "longitude"},
                    new Parameter {Name = "length", Value = "6"}
                }
            });
            root.Signatures.Add(Simple("direction"));
            root.Signatures.Add(new Signature {
                Name = "distance",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "fromlat"},
                    new Parameter {Name = "fromlon"},
                    new Parameter {Name = "tolat"},
                    new Parameter {Name = "tolon"}
                }
            });
            root.Signatures.Add(new Signature {
                Name = "humanize",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "format", Value = "[default]"}
                }
            });
            root.Signatures.Add(new Signature {
                Name = "web",
                Parameters = new List<Parameter> {
                    new Parameter {Name = "url", Value = ""},
                    new Parameter {Name = "web-method", Value = "GET"},
                    new Parameter {Name = "body", Value = ""}
                }
            });
            root.Signatures.Add(Simple("expression"));
            root.Signatures.Add(Simple("key"));
            root.Signatures.Add(new Signature {
                Name = "slice",
                NamedParameterIndicator = string.Empty,
                Parameters = new List<Parameter> {
                    new Parameter { Name = "expression" },
                    new Parameter { Name = "separator", Value="" },
                }
            });
            root.Signatures.Add(Simple("units"));

            root.Methods.Add(new Method { Name = "add", Signature = "none" });
            root.Methods.Add(new Method { Name = "abs", Signature = "none" });
            root.Methods.Add(new Method { Name = "any", Signature = "any" });
            root.Methods.Add(new Method { Name = "ceiling", Signature = "none" });
            root.Methods.Add(new Method { Name = "concat", Signature = "none" });
            root.Methods.Add(new Method { Name = "connection", Signature = "property" });
            root.Methods.Add(new Method { Name = "contains", Signature = "value" });
            root.Methods.Add(new Method { Name = "convert", Signature = "type" });
            root.Methods.Add(new Method { Name = "copy", Signature = "none", Ignore = true });
            root.Methods.Add(new Method { Name = "cs", Signature = "script" });
            root.Methods.Add(new Method { Name = "csharp", Signature = "script" });
            root.Methods.Add(new Method { Name = "datediff", Signature = "datediff" });
            root.Methods.Add(new Method { Name = "datepart", Signature = "timecomponent" });
            root.Methods.Add(new Method { Name = "decompress", Signature = "none" });
            root.Methods.Add(new Method { Name = "compress", Signature = "none" });
            root.Methods.Add(new Method { Name = "equal", Signature = "value" });
            root.Methods.Add(new Method { Name = "equals", Signature = "value" });
            root.Methods.Add(new Method { Name = "fileext", Signature = "none" });
            root.Methods.Add(new Method { Name = "filename", Signature = "file" });
            root.Methods.Add(new Method { Name = "filepath", Signature = "file" });
            root.Methods.Add(new Method { Name = "floor", Signature = "none" });
            root.Methods.Add(new Method { Name = "format", Signature = "format" });
            root.Methods.Add(new Method { Name = "formatphone", Signature = "none" });
            root.Methods.Add(new Method { Name = "formatxml", Signature = "none" });
            root.Methods.Add(new Method { Name = "hashcode", Signature = "none" });
            root.Methods.Add(new Method { Name = "htmldecode", Signature = "none" });
            root.Methods.Add(new Method { Name = "insert", Signature = "insert" });
            root.Methods.Add(new Method { Name = "is", Signature = "type" });
            root.Methods.Add(new Method { Name = "javascript", Signature = "script" });
            root.Methods.Add(new Method { Name = "join", Signature = "separator" });
            root.Methods.Add(new Method { Name = "js", Signature = "script" });
            root.Methods.Add(new Method { Name = "last", Signature = "dayofweek" });
            root.Methods.Add(new Method { Name = "left", Signature = "length" });
            root.Methods.Add(new Method { Name = "lower", Signature = "none" });
            root.Methods.Add(new Method { Name = "map", Signature = "map" });
            root.Methods.Add(new Method { Name = "multiply", Signature = "none" });
            root.Methods.Add(new Method { Name = "next", Signature = "dayofweek" });
            root.Methods.Add(new Method { Name = "now", Signature = "none" });
            root.Methods.Add(new Method { Name = "padleft", Signature = "padding" });
            root.Methods.Add(new Method { Name = "padright", Signature = "padding" });
            root.Methods.Add(new Method { Name = "razor", Signature = "template" });
            root.Methods.Add(new Method { Name = "regexreplace", Signature = "regexreplace" });
            root.Methods.Add(new Method { Name = "remove", Signature = "remove" });
            root.Methods.Add(new Method { Name = "replace", Signature = "replace" });
            root.Methods.Add(new Method { Name = "right", Signature = "length" });
            root.Methods.Add(new Method { Name = "round", Signature = "decimals" });
            root.Methods.Add(new Method { Name = "roundto", Signature = "value" });
            root.Methods.Add(new Method { Name = "roundupto", Signature = "value" });
            root.Methods.Add(new Method { Name = "rounddownto", Signature = "value" });
            root.Methods.Add(new Method { Name = "splitlength", Signature = "separator" });
            root.Methods.Add(new Method { Name = "substring", Signature = "substring" });
            root.Methods.Add(new Method { Name = "sum", Signature = "none" });
            root.Methods.Add(new Method { Name = "timeago", Signature = "fromtimezone" });
            root.Methods.Add(new Method { Name = "timeahead", Signature = "fromtimezone" });
            root.Methods.Add(new Method { Name = "timezone", Signature = "timezone" });
            root.Methods.Add(new Method { Name = "tolower", Signature = "none" });
            root.Methods.Add(new Method { Name = "tostring", Signature = "format" });
            root.Methods.Add(new Method { Name = "totime", Signature = "timecomponent" });
            root.Methods.Add(new Method { Name = "toupper", Signature = "none" });
            root.Methods.Add(new Method { Name = "toyesno", Signature = "none" });
            root.Methods.Add(new Method { Name = "trim", Signature = "trim" });
            root.Methods.Add(new Method { Name = "trimend", Signature = "trim" });
            root.Methods.Add(new Method { Name = "trimstart", Signature = "trim" });
            root.Methods.Add(new Method { Name = "upper", Signature = "none" });
            root.Methods.Add(new Method { Name = "utcnow", Signature = "none" });
            root.Methods.Add(new Method { Name = "velocity", Signature = "template" });
            root.Methods.Add(new Method { Name = "xmldecode", Signature = "none" });
            root.Methods.Add(new Method { Name = "xpath", Signature = "xpath" });
            root.Methods.Add(new Method { Name = "in", Signature = "domain" });
            root.Methods.Add(new Method { Name = "match", Signature = "pattern" });
            root.Methods.Add(new Method { Name = "coalesce", Signature = "none" });
            root.Methods.Add(new Method { Name = "startswith", Signature = "value" });
            root.Methods.Add(new Method { Name = "endswith", Signature = "value" });
            root.Methods.Add(new Method { Name = "invert", Signature = "none" });
            root.Methods.Add(new Method { Name = "isdefault", Signature = "none" });
            root.Methods.Add(new Method { Name = "isempty", Signature = "none" });
            root.Methods.Add(new Method { Name = "tag", Signature = "tag" });
            root.Methods.Add(new Method { Name = "include", Signature = "any" });
            root.Methods.Add(new Method { Name = "exclude", Signature = "any" });
            root.Methods.Add(new Method { Name = "slugify", Signature = "none" });
            root.Methods.Add(new Method { Name = "camelize", Signature = "none" });
            root.Methods.Add(new Method { Name = "dasherize", Signature = "none" });
            root.Methods.Add(new Method { Name = "hyphenate", Signature = "none" });
            root.Methods.Add(new Method { Name = "frommetric", Signature = "none" });
            root.Methods.Add(new Method { Name = "fromroman", Signature = "none" });
            root.Methods.Add(new Method { Name = "humanize", Signature = "humanize" });
            root.Methods.Add(new Method { Name = "dehumanize", Signature = "none" });
            root.Methods.Add(new Method { Name = "ordinalize", Signature = "none" });
            root.Methods.Add(new Method { Name = "pascalize", Signature = "none" });
            root.Methods.Add(new Method { Name = "pluralize", Signature = "none" });
            root.Methods.Add(new Method { Name = "singularize", Signature = "none" });
            root.Methods.Add(new Method { Name = "titleize", Signature = "none" });
            root.Methods.Add(new Method { Name = "tometric", Signature = "none" });
            root.Methods.Add(new Method { Name = "toordinalwords", Signature = "none" });
            root.Methods.Add(new Method { Name = "toroman", Signature = "none" });
            root.Methods.Add(new Method { Name = "towords", Signature = "none" });
            root.Methods.Add(new Method { Name = "underscore", Signature = "none" });
            root.Methods.Add(new Method { Name = "bytes", Signature = "none" });
            root.Methods.Add(new Method { Name = "dateadd", Signature = "dateadd" });
            root.Methods.Add(new Method { Name = "iif", Signature = "iif" });
            root.Methods.Add(new Method { Name = "geohashencode", Signature = "geohash" });
            root.Methods.Add(new Method { Name = "isnumeric", Signature = "none" });
            root.Methods.Add(new Method { Name = "geohashneighbor", Signature = "direction" });
            root.Methods.Add(new Method { Name = "commonprefix", Signature = "none" });
            root.Methods.Add(new Method { Name = "commonprefixes", Signature = "separator" });
            root.Methods.Add(new Method { Name = "distance", Signature = "distance" });

            root.Methods.Add(new Method { Name = "web", Signature = "web" });
            root.Methods.Add(new Method { Name = "urlencode", Signature = "none" });
            root.Methods.Add(new Method { Name = "fromjson", Signature = "none" });
            root.Methods.Add(new Method { Name = "datemath", Signature = "expression" });
            root.Methods.Add(new Method { Name = "eval", Signature = "expression" });

            // VIN, Vehicle Identification Number
            root.Methods.Add(new Method { Name = "isvin", Signature = "none" });
            root.Methods.Add(new Method { Name = "vinisvalid", Signature = "none" });
            root.Methods.Add(new Method { Name = "vingetworldmanufacturer", Signature = "none" });
            root.Methods.Add(new Method { Name = "vingetmodelyear", Signature = "none" });

            root.Methods.Add(new Method { Name = "isdaylightsavings", Signature = "none" });
            root.Methods.Add(new Method { Name = "htmlencode", Signature = "none" });
            root.Methods.Add(new Method { Name = "fromaddress", Signature = "key" });
            root.Methods.Add(new Method { Name = "distinct", Signature = "separator-space" });
            root.Methods.Add(new Method { Name = "ismatch", Signature = "pattern" });
            root.Methods.Add(new Method { Name = "matchcount", Signature = "pattern" });
            root.Methods.Add(new Method { Name = "slice", Signature = "slice" });
            root.Methods.Add(new Method { Name = "bytesize", Signature = "units" });
            root.Methods.Add(new Method { Name = "append", Signature = "value" });
            root.Check();

            if (!root.Errors().Any()) {
                return new ShorthandCustomizer(root, new[] { "fields", "calculated-fields" }, "t", "transforms", "method");
            }

            foreach (var error in root.Errors()) {
                Logger.Error(error);
            }
            Logger.Error("Please fix you shorthand configuration.  No short-hand is being processed.");

            return new ShorthandCustomizer(new ShorthandRoot(), new[] { "fields", "calculated-fields" }, "t", "transforms", "method");
        }

        private static Signature Simple(string name, string value = null) {
            if (value == null) {
                return new Signature {
                    Name = name,
                    Parameters = new List<Parameter> {
                            new Parameter { Name = name }
                        }
                };
            }
            return new Signature {
                Name = name,
                Parameters = new List<Parameter> {
                    new Parameter { Name = name, Value = value }
                }
            };
        }
    }
}
