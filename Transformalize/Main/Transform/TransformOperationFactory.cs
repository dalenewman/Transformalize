#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main.Parameters;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.Mail;
using Transformalize.Operations.Transform;
using Transformalize.Operations.Validate;
using System.Linq;
using Parameter = Transformalize.Main.Parameters.Parameter;

namespace Transformalize.Main {

    public class TransformOperationFactory {
        private const string SPACE = " ";
        private const string COMMA = ",";

        private readonly Process _process;
        private readonly Validator<TflTransform> _validator = ValidationFactory.CreateValidator<TflTransform>();
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();
        private readonly bool _isInitMode;
        private readonly string _entityName;

        public TransformOperationFactory(Process process, string entityName) {
            _process = process;
            _entityName = entityName;
            _isInitMode = process.IsInitMode();
        }

        public IOperation Create(Field field, TflTransform element, IParameters parameters) {

            if (_isInitMode)
                return new EmptyOperation();

            Func<Row, bool> shouldRun = row => true;
            var toTimeZone = string.IsNullOrEmpty(element.ToTimeZone) ? _process.TimeZone : element.ToTimeZone;
            var results = _validator.Validate(element);
            if (!results.IsValid) {
                TflLogger.Error(_process.Name, _entityName, "There is a problem with the transform element for field {0}.", field.Alias);
                foreach (var result in results) {
                    TflLogger.Error(_process.Name, _entityName, result.Message);
                }
                throw new TransformalizeException(_process.Name, _entityName, "Transform validation failed. See error log.");
            }

            var hasParameters = parameters.Count > 0;
            var inKey = hasParameters ? parameters[0].Name : field.Alias;
            var inType = hasParameters ? parameters[0].SimpleType : field.SimpleType;
            var outKey = field.Alias;
            var outType = field.SimpleType;
            var scripts = new Dictionary<string, Script>();

            if (!hasParameters) {
                parameters.Add(field.Alias, field.Alias, null, field.SimpleType);
            }

            if (!element.RunField.Equals(string.Empty)) {
                var op = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.RunOperator, true);
                var simpleType = Common.ToSimpleType(element.RunType.Equals(Common.DefaultValue) ? "boolean" : element.RunType);
                var runValue = simpleType.StartsWith("bool") && element.RunValue.Equals(Common.DefaultValue) ? "true" : element.RunValue;
                var value = Common.ConversionMap[simpleType](runValue);
                shouldRun = row => Common.CompareMap[op](row[element.RunField], value);
            }

            var templates = ComposeTemplates(field, ref element);

            switch (element.Method.ToLower()) {
                case "convert":
                    return new ConvertOperation(
                        inKey,
                        inType,
                        outKey,
                        element.To.Equals(string.Empty) ? outType : element.To,
                        element.Encoding,
                        element.Format
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "copy":
                    if (!hasParameters) {
                        throw new TransformalizeException(_process.Name, _entityName, "The copy transform requires a parameter.  It copies the parameter value into the calculated field.");
                    }
                    if(parameters.Count > 1)
                        return new CopyMultipleOperation(outKey, parameters) { ShouldRun = shouldRun, EntityName = _entityName };

                    return new CopyOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "collapse":
                    var partial = new PartialProcessOperation(_process);
                    partial.Register(new RegexReplaceOperation(inKey, outKey, "[\r\n]{2,}", "\r\n", 0) { ShouldRun = shouldRun, EntityName = _entityName });
                    partial.Register(new RegexReplaceOperation(inKey, outKey, " {2,}", " ", 0) { ShouldRun = shouldRun, EntityName = _entityName });
                    partial.Register(new TrimOperation(inKey, outKey, " ") { ShouldRun = shouldRun, EntityName = _entityName });
                    return partial;

                case "compress":
                    return new CompressOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "decompress":
                    return new DecompressOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "elipse":
                    return new ElipseOperation(inKey, outKey, element.Length, element.Elipse) { ShouldRun = shouldRun, EntityName = _entityName };

                case "replace":
                    return new ReplaceOperation(
                        inKey,
                        outKey,
                        element.OldValue,
                        element.NewValue
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "regexreplace":
                    return new RegexReplaceOperation(
                        inKey,
                        outKey,
                        element.Pattern,
                        element.Replacement,
                        element.Count
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "striphtml":
                    return new RegexReplaceOperation(
                        inKey,
                        outKey,
                        @"<[^>]+>|&nbsp;",
                        string.Empty,
                        0
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "insert":
                    return new InsertOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Value,
                        parameters[0].Name.Equals(outKey) ? null : GetParameter(_entityName, parameters[0].Name)
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "insertinterval":
                    return new InsertIntervalOperation(
                        inKey,
                        outKey,
                        element.Interval,
                        element.Value
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "append":
                    return new AppendOperation(
                        inKey,
                        outKey,
                        element.Value,
                        parameters[0].Name.Equals(outKey) ? null : GetParameter(_entityName, parameters[0].Name)
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "if":
                    var leftParameter = GetParameter(_entityName, element.Left, parameters);
                    return new IfOperation(
                        leftParameter,
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true),
                        GetParameter(_entityName, element.Right, parameters, leftParameter.SimpleType),
                        GetParameter(_entityName, element.Then, parameters),
                        GetParameter(_entityName, element.Else, parameters),
                        outKey,
                        outType
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "distinctwords":
                    if (element.Separator.Equals(Common.DefaultValue)) {
                        element.Separator = SPACE;
                    }
                    return new DistinctWordsOperation(
                        inKey,
                        outKey,
                        element.Separator
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "guid":
                    return new GuidOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "now":
                    toTimeZone = TimeZoneOperation.GuardTimeZone(field.Process, _entityName, toTimeZone, TimeZoneInfo.Local.Id);
                    return new PartialProcessOperation(_process)
                        .Register(
                            new NowOperation(
                                inKey,
                                outKey
                                ) { ShouldRun = shouldRun, EntityName = _entityName })
                        .Register(
                            new TimeZoneOperation(
                                outKey,
                                outKey,
                                "UTC",
                                toTimeZone)
                            );
                case "remove":
                    return new RemoveOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Length
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "trimstart":
                    return new TrimStartOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "trimstartappend":
                    if (element.Separator.Equals(Common.DefaultValue)) {
                        element.Separator = SPACE;
                    }
                    return new TrimStartAppendOperation(
                        inKey,
                        outKey,
                        element.TrimChars,
                        element.Separator
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "trimend":
                    return new TrimEndOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "trim":
                    return new TrimOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "substring":
                    return new SubstringOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Length
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "left":
                    return new LeftOperation(
                        inKey,
                        outKey,
                        element.Length
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "right":
                    return new RightOperation(
                        inKey,
                        outKey,
                        element.Length
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "gethashcode":
                    return new GetHashCodeOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "hashcode":
                    goto case "gethashcode";

                case "mail":
                    Guard.Against(string.IsNullOrEmpty(element.Connection), "The mail transform operations requires the connection attribute be set.");

                    var connection = _process.Connections[element.Connection];

                    Guard.Against(connection.Type != ProviderType.Mail, "The mail transform requires a valid mail connection.  The connection you referenced is {0}", connection.Type);

                    if (!parameters.ContainsName("body")) {
                        parameters.Add(field.Alias, "body", null, "string");
                    }

                    if (_process.Connections.ContainsKey(element.Connection)) {
                        return new MailOperation(
                            (MailConnection)connection,
                            parameters
                            ) { ShouldRun = shouldRun, EntityName = _entityName };
                    }

                    throw new TransformalizeException(_process.Name, _entityName, "Mail operation references invalid connection {0}", element.Connection);

                case "map":
                    var equals = _process.MapEquals.ContainsKey(element.Map) ? _process.MapEquals[element.Map] : new Map();
                    var startsWith = _process.MapStartsWith.ContainsKey(element.Map) ? _process.MapStartsWith[element.Map] : new Map();
                    var endsWith = _process.MapEndsWith.ContainsKey(element.Map) ? _process.MapEndsWith[element.Map] : new Map();

                    if (equals.Count == 0 && startsWith.Count == 0 && endsWith.Count == 0) {
                        if (element.Map.Contains("=")) {
                            foreach (var item in element.Map.Split(new[] { ',' })) {
                                var split = item.Split(new[] { '=' });
                                if (split.Length == 2) {
                                    var left = split[0];
                                    var right = split[1];
                                    Field tryField;
                                    if (_process.TryGetField(_entityName, right, out tryField, false)) {
                                        equals.Add(left, new Item(tryField.Alias, right));
                                    } else {
                                        equals.Add(left, new Item(right));
                                    }
                                }
                            }
                            if (equals.Count == 0) {
                                TflLogger.Warn(_process.Name, _entityName, "Map '{0}' is empty.", element.Map);
                            }
                        } else {
                            TflLogger.Warn(_process.Name, _entityName, "Map '{0}' is empty.", element.Map);
                        }
                    }

                    return new MapOperation(
                        inKey,
                        outKey,
                        outType,
                        new[] { @equals, startsWith, endsWith }
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "markdown":
                    return new MarkDownOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "padleft":
                    return new PadLeftOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "padright":
                    return new PadRightOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "tostring":
                    return new ToStringOperation(
                        inKey,
                        inType,
                        outKey,
                        element.Format
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "toupper":
                    return new ToUpperOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "tolower":
                    return new ToLowerOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "javascript":
                    foreach (var script in element.Scripts) {
                        if (!_process.Scripts.ContainsKey(script.Name)) {
                            throw new TransformalizeException(_process.Name, _entityName, "Invalid script reference: {0}.", script.Name);
                        }
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return new JavascriptOperation(
                        outKey,
                        element.Script,
                        scripts,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "csharp":
                    foreach (var script in element.Scripts) {
                        if (!_process.Scripts.ContainsKey(script.Name)) {
                            throw new TransformalizeException(_process.Name, _entityName, "Invalid script reference: {0}.", script.Name);
                        }
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return new CSharpOperation(
                        outKey,
                        outType,
                        (element.ReplaceSingleQuotes ? Regex.Replace(element.Script, @"(?<=[^'])'{1}(?=[^'])", "\"") : element.Script),
                        scripts,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "template":
                    return new RazorOperation(
                        outKey,
                        outType,
                        element.Template,
                        element.Model,
                        templates,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "razor":
                    goto case "template";

                case "velocity":
                    if (!_process.VelocityInitialized) {
                        Velocity.Init();
                    }
                    return new VelocityOperation(
                        outKey,
                        outType,
                        element.Template,
                        templates,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "tag":
                    return new TagOperation(
                        outKey,
                        element.Tag,
                        parameters,
                        element.Decode,
                        element.Encode
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "format":
                    return new FormatOperation(
                        outKey,
                        element.Format,
                        parameters
                        ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "concat":
                    return new ConcatOperation(
                        outKey,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "totitlecase":
                    return new ToTitleCaseOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "formatphone":
                    return new FormatPhoneOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName};

                case "join":
                    if (element.Separator.Equals(Common.DefaultValue)) {
                        element.Separator = SPACE;
                    }
                    return new JoinTransformOperation(
                        outKey,
                        element.Separator,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "tolocaltime":
                    return new ToLocalTimeOperation(
                        inKey,
                        outKey,
                        element.FromTimeZone,
                        element.ToTimeZone
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "timezone":
                    element.FromTimeZone = TimeZoneOperation.GuardTimeZone(field.Process, _entityName, element.FromTimeZone, "UTC");
                    toTimeZone = TimeZoneOperation.GuardTimeZone(field.Process, _entityName, toTimeZone, TimeZoneInfo.Local.Id);
                    return new TimeZoneOperation(
                        inKey,
                        outKey,
                        element.FromTimeZone,
                        toTimeZone
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "tojson":
                    return new ToJsonOperation(
                        outKey,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "fromxml":
                    switch ((XmlMode)Enum.Parse(typeof(XmlMode), element.XmlMode)) {
                        case XmlMode.First:
                            return new FromFirstXmlOperation(
                                outKey,
                                new Fields(_process, parameters, _entityName)
                            ) { ShouldRun = shouldRun, EntityName = _entityName };
                        case XmlMode.All:
                            return new FromXmlOperation(
                                outKey,
                                element.Root,
                                new Fields(_process, parameters, _entityName)
                            ) { ShouldRun = shouldRun, EntityName = _entityName };
                        default:
                            return new FromNanoXmlOperation(
                                outKey,
                                new Fields(_process, parameters, _entityName)
                            ) { ShouldRun = shouldRun, EntityName = _entityName };
                    }

                case "fromregex":
                    return new FromRegexOperation(
                        outKey,
                        element.Pattern,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "fromjson":
                    return new FromJsonOperation(
                        TryRemoveInputParameters(element, parameters) ? inKey : outKey,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "fromsplit":
                    return new FromSplitOperation(outKey, element.Separator, parameters) { ShouldRun = shouldRun, EntityName = _entityName };

                case "distance":

                    return new DistanceOperation(
                        outKey,
                        element.Units,
                        GetParameter(_entityName, element.FromLat),
                        GetParameter(_entityName, element.FromLong),
                        GetParameter(_entityName, element.ToLat),
                        GetParameter(_entityName, element.ToLong)
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "length":
                    return new LengthOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "timeofday":
                    return new TimeOfDayOperation(inKey, inType, outKey, outType, element.TimeComponent) { ShouldRun = shouldRun, EntityName = _entityName };

                case "value":
                    return new ValueOperation(outKey, outType, element.Value, parameters) { ShouldRun = shouldRun, EntityName = _entityName };

                case "xpath":
                    return new XPathOperation(inKey, outKey, outType, element.Xpath) { ShouldRun = shouldRun, EntityName = _entityName };

                case "xmlencode":
                    return new XmlEncodeOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "urlencode":
                    return new UrlEncodeOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "htmlencode":
                    return new HtmlEncodeOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "htmldecode":
                    return new HtmlDecodeOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "filter":
                    return new FilterOperation(
                        inKey,
                        outKey,
                        outType,
                        element.Value,
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true)
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "splitindex":
                    return new SplitIndexOperation(
                        inKey,
                        outKey,
                        outType,
                        element.Separator,
                        element.Count,
                        element.Index
                        ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "datepart":
                    return new DatePartOperation(
                        inKey,
                        outKey,
                        outType,
                        element.TimeComponent
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "average":
                    return new AverageOperation(
                        outKey,
                        outType,
                        parameters
                        ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "add":
                    return new AddOperation(
                        outKey,
                        outType,
                        parameters
                        ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "geocode":
                    return new GeoCodeOperation(
                        inKey,
                        outKey,
                        element.Sleep,
                        element.UseHttps,
                        parameters
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "transliterate":
                    return new TransliterateOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "slug":
                    return new SlugOperation(inKey, outKey, element.Length) { ShouldRun = shouldRun, EntityName = _entityName };

                case "cyrtolat":
                    return new CyrToLatOperation(inKey, outKey) { ShouldRun = shouldRun, EntityName = _entityName };

                case "web":
                    return new WebOperation(
                        element.Url.Equals(string.Empty) ? parameters[0] : GetParameter(_entityName, element.Url, parameters),
                        outKey,
                        element.Sleep,
                        element.WebMethod,
                        GetParameter(_entityName, element.Data, parameters),
                        element.ContentType
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "run":
                    Guard.Against(string.IsNullOrEmpty(element.Connection), "Run transform requires a connection.");
                    Guard.Against(!_process.Connections.ContainsKey(element.Connection), "Run transform requires a connection defined in <connections/>. {0} is not defined.", element.Connection);
                    return new RunOperation(
                        inKey,
                        _process.Connections[element.Connection],
                        element.TimeOut
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                // validators
                case "isdaylightsavings":
                    return new IsDaylightSavingsOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "any":
                    return new AnyOperation(
                        string.IsNullOrEmpty(element.Value) ? GetParameter(_entityName, element.Left, parameters) : new Parameter(element.Value, element.Value),
                        outKey,
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true),
                        parameters,
                        element.Negated
                    ) { ShouldRun = shouldRun, EntityName = _entityName };

                case "containscharacters":
                    return new ContainsCharactersValidatorOperation(
                        inKey,
                        outKey,
                        element.Characters,
                        (ContainsCharacters)Enum.Parse(typeof(ContainsCharacters), element.ContainsCharacters, true),
                        element.Negated
                    );

                case "datetimerange":
                    return new DateTimeRangeValidatorOperation(
                        inKey,
                        outKey,
                        (DateTime)_conversionMap[inType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (DateTime)_conversionMap[inType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Negated
                    );

                case "domain":
                    if (element.Separator.Equals(Common.DefaultValue)) {
                        element.Separator = COMMA;
                    }
                    var domain = element.Domain.Split(element.Separator.ToCharArray()).Select(s => _conversionMap[inType](s));

                    return new DomainValidatorOperation(
                        inKey,
                        outKey,
                        domain,
                        element.Negated
                    );

                case "isjson":
                    return new JsonValidatorOperation(inKey, outKey, element.Negated);

                case "notnull":
                    return new NotNullValidatorOperation(inKey, outKey, element.Negated);

                case "fieldcomparison":
                    return new PropertyComparisonValidatorOperation(inKey, element.TargetField, outKey, element.Operator, element.Negated);

                case "range":
                    return new RangeValidatorOperation(
                        inKey,
                        outKey,
                        (IComparable)_conversionMap[field.SimpleType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (IComparable)_conversionMap[field.SimpleType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Negated
                    );

                case "regex":
                    return new RegexValidatorOperation(
                        inKey,
                        outKey,
                        element.Pattern,
                        element.Negated
                    );

                case "relativedatetime":
                    return new RelativeDateTimeValidatorOperation(
                        inKey,
                        outKey,
                        Convert.ToInt32(element.LowerBound),
                        (DateTimeUnit)Enum.Parse(typeof(DateTimeUnit), element.LowerUnit, true),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        Convert.ToInt32(element.UpperBound),
                        (DateTimeUnit)Enum.Parse(typeof(DateTimeUnit), element.UpperUnit, true),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Negated
                    );

                case "startswith":
                    return new StartsWithValidatorOperation(
                        inKey,
                        element.Value,
                        outKey,
                        element.Negated
                    );

                case "stringlength":
                    return new StringLengthValidatorOperation(
                        inKey,
                        outKey,
                        Convert.ToInt32(element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        Convert.ToInt32(element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Negated
                    );

                case "typeconversion":
                    return new TypeConversionValidatorOperation(
                        inKey,
                        outKey,
                        Common.ToSystemType(element.Type),
                        element.Negated,
                        element.IgnoreEmpty
                    );

            }

            TflLogger.Warn(field.Process, _entityName, "{0} method is undefined.  It will not be used.", element.Method);
            return new EmptyOperation();
        }

        private Dictionary<string, Template> ComposeTemplates(Field field, ref TflTransform element) {
            var templates = new Dictionary<string, Template>();
            var method = element.Method.ToLower();
            if (new[] { "razor", "template", "velocity" }.All(n => n != method))
                return templates;

            foreach (var template in element.Templates) {
                if (!_process.Templates.ContainsKey(template.Name)) {
                    throw new TransformalizeException(_process.Name, _entityName, "Invalid template reference: {0}", template.Name);
                }
                templates[template.Name] = _process.Templates[template.Name];
                _process.Templates[template.Name].IsUsedInPipeline = true;
            }
            if (_process.Templates.ContainsKey(element.Template)) {
                templates[element.Template] = _process.Templates[element.Template];
                _process.Templates[element.Template].IsUsedInPipeline = true;
                element.Template = string.Empty;
            }
            if (!templates.Any() && string.IsNullOrEmpty(element.Template) && _process.Templates.ContainsKey(field.Alias)) {
                templates[field.Alias] = _process.Templates[field.Alias];
                _process.Templates[field.Alias].IsUsedInPipeline = true;
                element.Template = string.Empty;
            }
            return templates;
        }

        private IParameter GetParameter(string entity, string parameter) {
            Field f;
            return _process.TryGetField(entity, parameter, out f, false) ?
                f.ToParameter() :
                new Parameter(parameter, parameter);
        }

        private IParameter GetParameter(string entity, string parameter, IParameters parameters, string newParameterType = "string") {
            Field f;
            if (_process.TryGetField(entity, parameter, out f, false)) {
                return f.ToParameter();
            }
            if (parameters.ContainsKey(parameter)) {
                return parameters[parameter];
            }

            return new Parameter(parameter, parameter) { SimpleType = newParameterType };
        }

        private static bool TryRemoveInputParameters(TflTransform element, IParameters parameters) {
            //if inKey (the field, or first parameter) is not in fields,
            //then it is an input parameter, not an output parameter (or field)
            var parameterElements = element.Parameters;
            if (!parameterElements.Any(f => f.Input))
                return false;
            var key = parameterElements.First(f => f.Input).Field;
            if (!parameters.ContainsKey(key))
                return false;
            parameters.Remove(key);
            return true;
        }

    }
}