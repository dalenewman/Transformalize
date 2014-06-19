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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Operations.Validate;
using System.Linq;

namespace Transformalize.Main {

    public class TransformOperationFactory {
        private const string DEFAULT = "[default]";
        private const string SPACE = " ";
        private const string COMMA = ",";

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly Process _process;
        private readonly Validator<TransformConfigurationElement> _validator = ValidationFactory.CreateValidator<TransformConfigurationElement>();
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

        public TransformOperationFactory(Process process) {
            _process = process;
        }

        public IOperation Create(Field field, TransformConfigurationElement element, IParameters parameters) {

            Func<Row, bool> shouldRun = row => true;
            var toTimeZone = string.IsNullOrEmpty(element.ToTimeZone) ? _process.TimeZone : element.ToTimeZone;
            var results = _validator.Validate(element);
            if (!results.IsValid) {
                _log.Error("There is a problem with the transform element for field {0}.", field.Alias);
                foreach (var result in results) {
                    _log.Error(result.Message);
                }
                throw new TransformalizeException("Transform validation failed. See error log.");
            }

            var hasParameters = parameters.Count > 0;
            var inKey = hasParameters ? parameters[0].Name : field.Alias;
            var inType = hasParameters ? parameters[0].SimpleType : field.SimpleType;
            var outKey = field.Alias;
            var outType = field.SimpleType;
            var resultKey = element.ResultField.Equals(DEFAULT) ? field.Alias + "Result" : element.ResultField;
            var messageKey = element.MessageField.Equals(DEFAULT) ? field.Alias + "Message" : element.MessageField;
            var scripts = new Dictionary<string, Script>();

            if (!hasParameters) {
                parameters.Add(field.Alias, field.Alias, null, field.SimpleType);
            }

            if (!element.RunField.Equals(string.Empty)) {
                var op = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.RunOperator, true);
                var simpleType = Common.ToSimpleType(element.RunType.Equals(DEFAULT) ? "boolean" : element.RunType);
                var value = Common.ConversionMap[simpleType](element.RunValue);
                shouldRun = row => Common.CompareMap[op](row[element.RunField], value);
            }

            switch (element.Method.ToLower()) {
                case "convert":
                    return new ConvertOperation(
                        inKey,
                        inType,
                        outKey,
                        Common.ToSimpleType(element.To),
                        element.Format
                    ) { ShouldRun = shouldRun };

                case "copy":
                    return new CopyOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "elipse":
                    return new ElipseOperation(inKey, outKey, element.Length, element.Elipse) { ShouldRun = shouldRun };

                case "replace":
                    return new ReplaceOperation(
                        inKey,
                        outKey,
                        element.OldValue,
                        element.NewValue
                    ) { ShouldRun = shouldRun };

                case "regexreplace":
                    return new RegexReplaceOperation(
                        inKey,
                        outKey,
                        element.Pattern,
                        element.Replacement,
                        element.Count
                    ) { ShouldRun = shouldRun };

                case "insert":
                    return new InsertOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Value
                    ) { ShouldRun = shouldRun };

                case "insertinterval":
                    return new InsertIntervalOperation(
                        inKey,
                        outKey,
                        element.Interval,
                        element.Value
                    ) { ShouldRun = shouldRun };

                case "append":
                    return new AppendOperation(
                        inKey,
                        outKey,
                        element.Value
                    ) { ShouldRun = shouldRun };

                case "if":
                    return new IfOperation(
                        GetParameter(field.Entity, element.Left, parameters),
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true),
                        GetParameter(field.Entity, element.Right, parameters),
                        GetParameter(field.Entity, element.Then, parameters),
                        GetParameter(field.Entity, element.Else, parameters),
                        outKey,
                        outType
                    ) { ShouldRun = shouldRun };

                case "distinctwords":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new DistinctWordsOperation(
                        inKey,
                        outKey,
                        element.Separator
                    ) { ShouldRun = shouldRun };

                case "newguid":
                    return new NewGuidOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "guid":
                    return new NewGuidOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "now":
                    return new PartialProcessOperation()
                        .Register(
                            new NowOperation(
                                inKey,
                                outKey
                                ) { ShouldRun = shouldRun })
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
                    ) { ShouldRun = shouldRun };

                case "trimstart":
                    return new TrimStartOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun };

                case "trimstartappend":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new TrimStartAppendOperation(
                        inKey,
                        outKey,
                        element.TrimChars,
                        element.Separator
                    ) { ShouldRun = shouldRun };

                case "trimend":
                    return new TrimEndOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun };

                case "trim":
                    return new TrimOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    ) { ShouldRun = shouldRun };

                case "substring":
                    return new SubstringOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Length
                    ) { ShouldRun = shouldRun };

                case "left":
                    return new LeftOperation(
                        inKey,
                        outKey,
                        element.Length
                    ) { ShouldRun = shouldRun };

                case "right":
                    return new RightOperation(
                        inKey,
                        outKey,
                        element.Length
                    ) { ShouldRun = shouldRun };

                case "gethashcode":
                    return new GetHashCodeOperation(
                        inKey,
                        outKey
                    );

                case "map":
                    var equals = _process.MapEquals.ContainsKey(element.Map) ? _process.MapEquals[element.Map] : new Map();
                    var startsWith = _process.MapStartsWith.ContainsKey(element.Map) ? _process.MapStartsWith[element.Map] : new Map();
                    var endsWith = _process.MapEndsWith.ContainsKey(element.Map) ? _process.MapEndsWith[element.Map] : new Map();

                    if (equals.Count == 0 && startsWith.Count == 0 && endsWith.Count == 0) {
                        throw new TransformalizeException("Map '{0}' is not defined.", element.Map);
                    }

                    return new MapOperation(
                        inKey,
                        outKey,
                        outType,
                        new[] { @equals, startsWith, endsWith }
                    ) { ShouldRun = shouldRun };

                case "padleft":
                    return new PadLeftOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    ) { ShouldRun = shouldRun };

                case "padright":
                    return new PadRightOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    ) { ShouldRun = shouldRun };

                case "tostring":
                    return new ToStringOperation(
                        inKey,
                        inType,
                        outKey,
                        element.Format
                    ) { ShouldRun = shouldRun };

                case "toupper":
                    return new ToUpperOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun };

                case "tolower":
                    return new ToLowerOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun };

                case "javascript":
                    foreach (TransformScriptConfigurationElement script in element.Scripts) {
                        if (!_process.Scripts.ContainsKey(script.Name)) {
                            throw new TransformalizeException("Invalid script reference: {0}.", script.Name);
                        }
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return new JavascriptOperation(
                        outKey,
                        element.Script,
                        scripts,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "csharp":
                    foreach (TransformScriptConfigurationElement script in element.Scripts) {
                        if (!_process.Scripts.ContainsKey(script.Name)) {
                            throw new TransformalizeException("Invalid script reference: {0}.", script.Name);
                        }
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return new CSharpOperation(
                        outKey,
                        outType,
                        (element.ReplaceSingleQuotes ? Regex.Replace(element.Script, @"(?<=[^'])'{1}(?=[^'])", "\"") : element.Script),
                        scripts,
                        parameters
                    ) { ShouldRun = shouldRun };


                case "template":

                    var templates = new Dictionary<string, Template>();
                    foreach (TransformTemplateConfigurationElement template in element.Templates) {
                        if (!_process.Templates.ContainsKey(template.Name)) {
                            throw new TransformalizeException("Invalid template reference: {0}", template.Name);
                        }
                        templates[template.Name] = _process.Templates[template.Name];
                        _process.Templates[template.Name].IsUsedInPipeline = true;
                    }

                    return new TemplateOperation(
                        outKey,
                        outType,
                        element.Template,
                        element.Model,
                        templates,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "format":
                    return new FormatOperation(
                        outKey,
                        element.Format,
                        parameters
                        ) { ShouldRun = shouldRun };

                case "concat":
                    return new ConcatOperation(
                        outKey,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "totitlecase":
                    return new ToTitleCaseOperation(
                        inKey,
                        outKey
                    ) { ShouldRun = shouldRun };

                case "join":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new JoinTransformOperation(
                        outKey,
                        element.Separator,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "tolocaltime":
                    return new ToLocalTimeOperation(
                        inKey,
                        outKey,
                        element.FromTimeZone,
                        element.ToTimeZone
                    ) { ShouldRun = shouldRun };

                case "timezone":
                    return new TimeZoneOperation(
                        inKey,
                        outKey,
                        element.FromTimeZone,
                        toTimeZone
                    ) { ShouldRun = shouldRun };

                case "tojson":
                    return new ToJsonOperation(
                        outKey,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "fromxml":
                    switch ((XmlMode)Enum.Parse(typeof(XmlMode), element.XmlMode)) {
                        case XmlMode.First:
                            return new FromFirstXmlOperation(
                                outKey,
                                new Fields(_process, parameters, field.Entity)
                            ) { ShouldRun = shouldRun };
                        case XmlMode.All:
                            return new FromXmlOperation(
                                outKey,
                                new Fields(_process, parameters, field.Entity)
                            ) { ShouldRun = shouldRun };
                        default:
                            return new FromNanoXmlOperation(
                                outKey,
                                new Fields(_process, parameters, field.Entity)
                            ) { ShouldRun = shouldRun };
                    }
                case "fromregex":
                    return new FromRegexOperation(
                        outKey,
                        element.Pattern,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "fromjson":
                    return new FromJsonOperation(
                        outKey,
                        parameters
                    ) { ShouldRun = shouldRun };

                case "distance":

                    return new DistanceOperation(
                        outKey,
                        element.Units,
                        GetParameter(field.Entity, element.FromLat),
                        GetParameter(field.Entity, element.FromLong),
                        GetParameter(field.Entity, element.ToLat),
                        GetParameter(field.Entity, element.ToLong)
                    ) { ShouldRun = shouldRun };

                case "length":
                    return new LengthOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "timeofday":
                    return new TimeOfDayOperation(inKey, inType, outKey, outType, element.TimeComponent) { ShouldRun = shouldRun };

                case "value":
                    return new ValueOperation(outKey, outType, element.Value, parameters) { ShouldRun = shouldRun };

                case "xpath":
                    return new XPathOperation(inKey, outKey, outType, element.XPath) { ShouldRun = shouldRun };

                case "xmlencode":
                    return new XmlEncodeOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "htmlencode":
                    return new HtmlEncodeOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "htmldecode":
                    return new HtmlDecodeOperation(inKey, outKey) { ShouldRun = shouldRun };

                case "filter":
                    return new FilterOperation(
                        inKey,
                        outKey,
                        outType,
                        element.Value,
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true)
                    ) { ShouldRun = shouldRun };

                case "splitindex":
                    return new SplitIndexOperation(
                        inKey,
                        outKey,
                        outType,
                        element.Separator,
                        element.Count,
                        element.Index
                        ) { ShouldRun = shouldRun };

                case "datepart":
                    return new DatePartOperation(
                        inKey,
                        outKey,
                        outType,
                        element.TimeComponent
                    ) { ShouldRun = shouldRun };

                // validators
                case "containscharacters":
                    return new ContainsCharactersValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        element.Characters,
                        (ContainsCharacters)Enum.Parse(typeof(ContainsCharacters), element.ContainsCharacters, true),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "datetimerange":
                    return new DateTimeRangeValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        (DateTime)_conversionMap[field.SimpleType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (DateTime)_conversionMap[field.SimpleType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "domain":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = COMMA;
                    }
                    var domain = element.Domain.Split(element.Separator.ToCharArray()).Select(s => _conversionMap[field.SimpleType](s));

                    return new DomainValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        domain,
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "isjson":
                    return new JsonValidatorOperation(inKey, resultKey, messageKey, element.MessageTemplate, element.Negated, element.MessageAppend);

                case "notnull":
                    return new NotNullValidatorOperation(inKey, resultKey, messageKey, element.MessageTemplate, element.Negated, element.MessageAppend);

                case "fieldcomparison":
                    return new PropertyComparisonValidatorOperation(inKey, element.TargetField, resultKey, messageKey, element.Operator, element.MessageTemplate, element.Negated, element.MessageAppend);

                case "range":
                    return new RangeValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        (IComparable)_conversionMap[field.SimpleType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (IComparable)_conversionMap[field.SimpleType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "regex":
                    return new RegexValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        element.Pattern,
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "relativedatetime":
                    return new RelativeDateTimeValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        Convert.ToInt32(element.LowerBound),
                        (DateTimeUnit)Enum.Parse(typeof(DateTimeUnit), element.LowerUnit, true),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        Convert.ToInt32(element.UpperBound),
                        (DateTimeUnit)Enum.Parse(typeof(DateTimeUnit), element.UpperUnit, true),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "startswith":
                    return new StartsWithValidatorOperation(
                        inKey,
                        element.Value,
                        resultKey,
                        messageKey,
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "stringlength":
                    return new StringLengthValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        Convert.ToInt32(element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        Convert.ToInt32(element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend
                    );

                case "typeconversion":
                    return new TypeConversionValidatorOperation(
                        inKey,
                        resultKey,
                        messageKey,
                        Common.ToSystemType(element.Type),
                        element.MessageTemplate,
                        element.Negated,
                        element.MessageAppend,
                        element.IgnoreEmpty
                    );

            }

            _log.Warn("{0} method is undefined.  It will not be used.", element.Method);
            return new EmptyOperation();
        }

        private IParameter GetParameter(string entity, string parameter) {
            Field f;
            return _process.TryGetField(parameter, entity, out f, false) ?
                f.ToParameter() :
                new Parameter(parameter, parameter);
        }

        private IParameter GetParameter(string entity, string parameter, IParameters parameters) {
            Field f;
            if (_process.TryGetField(parameter, entity, out f, false)) {
                return f.ToParameter();
            }
            if (parameters.ContainsKey(parameter)) {
                return parameters[parameter];
            }

            return new Parameter(parameter, parameter);
        }

    }
}