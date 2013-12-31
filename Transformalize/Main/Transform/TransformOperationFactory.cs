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
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Operations.Validate;
using System.Linq;

namespace Transformalize.Main {

    public class TransformOperationFactory {
        private const string DEFAULT = "[default]";
        private const string SPACE = " ";
        private const string COMMA = ",";

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly Validator<TransformConfigurationElement> _validator = ValidationFactory.CreateValidator<TransformConfigurationElement>();

        public TransformOperationFactory(Process process) {
            _process = process;
        }

        public AbstractOperation Create(Field field, TransformConfigurationElement element, IParameters parameters) {

            var results = _validator.Validate(element);
            if (!results.IsValid) {
                _log.Error("There is a problem with the transform element for field {0}.", field.Alias);
                foreach (var result in results) {
                    _log.Error(result.Message);
                }
                Environment.Exit(1);
            }

            var hasParameters = parameters.Count > 0;
            var inKey = hasParameters ? parameters[0].Name : field.Alias;
            var inType = hasParameters ? parameters[0].SimpleType : field.SimpleType;
            var append = !string.IsNullOrEmpty(element.AppendTo);
            var outKey = append ? element.AppendTo : field.Alias;
            var outType = append ? _process.GetField(outKey, field.Entity).SimpleType : field.SimpleType;

            if (!hasParameters) {
                parameters.Add(field.Alias, field.Alias, null, field.SimpleType);
            }

            switch (element.Method.ToLower()) {
                case "convert":
                    return new ConvertOperation(
                        inKey,
                        inType,
                        outKey,
                        Common.ToSimpleType(element.To),
                        element.Format
                    );

                case "replace":
                    return new ReplaceOperation(
                        inKey,
                        outKey,
                        element.OldValue,
                        element.NewValue
                    );

                case "regexreplace":
                    return new RegexReplaceOperation(
                        inKey,
                        outKey,
                        element.Pattern,
                        element.Replacement,
                        element.Count
                    );

                case "insert":
                    return new InsertOperation(
                        inKey,
                        outKey,
                        element.Index,
                        element.Value
                    );

                case "if":
                    return new IfOperation(
                        GetParameter(field.Entity, element.Left, parameters),
                        (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), element.Operator, true),
                        GetParameter(field.Entity, element.Right, parameters),
                        GetParameter(field.Entity, element.Then, parameters),
                        GetParameter(field.Entity, element.Else, parameters),
                        outKey,
                        outType
                    );

                case "distinctwords":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new DistinctWordsOperation(
                        inKey,
                        outKey,
                        element.Separator
                    );

                case "remove":
                    return new RemoveOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Length
                    );

                case "trimstart":
                    return new TrimStartOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    );

                case "trimstartappend":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new TrimStartAppendOperation(
                        inKey,
                        outKey,
                        element.TrimChars,
                        element.Separator
                    );

                case "trimend":
                    return new TrimEndOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    );

                case "trim":
                    return new TrimOperation(
                        inKey,
                        outKey,
                        element.TrimChars
                    );

                case "substring":
                    return new SubstringOperation(
                        inKey,
                        outKey,
                        element.StartIndex,
                        element.Length
                    );

                case "left":
                    return new LeftOperation(
                        inKey,
                        outKey,
                        element.Length
                    );

                case "right":
                    return new RightOperation(
                        inKey,
                        outKey,
                        element.Length
                    );

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
                        _log.Error("Map '{0}' is not defined.", element.Map);
                        Environment.Exit(1);
                    }

                    return new MapOperation(
                        inKey,
                        outKey,
                        outType,
                        new[] { @equals, startsWith, endsWith }
                    );

                case "padleft":
                    return new PadLeftOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    );

                case "padright":
                    return new PadRightOperation(
                        inKey,
                        outKey,
                        element.TotalWidth,
                        element.PaddingChar
                    );

                case "tostring":
                    return new ToStringOperation(
                        inKey,
                        inType,
                        outKey,
                        element.Format
                    );

                case "toupper":
                    return new ToUpperOperation(
                        inKey,
                        outKey
                    );

                case "tolower":
                    return new ToLowerOperation(
                        inKey,
                        outKey
                    );

                case "javascript":
                    var scripts = new Dictionary<string, Script>();
                    foreach (TransformScriptConfigurationElement script in element.Scripts) {
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return new JavascriptOperation(
                        outKey,
                        element.Script,
                        scripts,
                        parameters
                    );

                case "expression":
                    return new ExpressionOperation(
                        outKey,
                        element.Expression,
                        parameters
                    );

                case "template":

                    var templates = new Dictionary<string, Template>();
                    foreach (TransformTemplateConfigurationElement template in element.Templates) {
                        templates[template.Name] = _process.Templates[template.Name];
                    }

                    return new TemplateOperation(
                        outKey,
                        element.Template,
                        element.Model,
                        templates,
                        parameters
                    );

                case "format":
                    return new FormatOperation(
                        outKey,
                        element.Format,
                        parameters
                    );

                case "concat":
                    return new ConcatOperation(
                        outKey,
                        parameters
                    );

                case "totitlecase":
                    return new ToTitleCaseOperation(
                        inKey,
                        outKey
                    );

                case "join":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = SPACE;
                    }
                    return new JoinTransformOperation(
                        outKey,
                        element.Separator,
                        parameters
                    );

                case "tolocaltime":
                    return new ToLocalTimeOperation(
                        inKey,
                        outKey,
                        element.FromTimeZone,
                        element.ToTimeZone
                    );

                case "tojson":
                    return new ToJsonOperation(
                        outKey,
                        parameters
                    );

                case "fromxml":
                    return new FromXmlOperation(
                        outKey,
                        new Fields(_process, parameters, field.Entity)
                    );

                case "fromregex":
                    return new FromRegexOperation(
                        outKey,
                        element.Pattern,
                        parameters
                    );

                case "fromjson":
                    return new FromJsonOperation(
                        outKey,
                        element.Clean,
                        element.TryParse,
                        parameters
                    );

                case "defaultifequal":
                    return new DefaultIfEqualOperation(
                        inKey,
                        outKey,
                        field.Default,
                        new DefaultFactory().Convert(element.Value, field.SimpleType)
                    );

                case "distance":

                    return new DistanceOperation(
                        outKey,
                        element.Units,
                        GetParameter(field.Entity, element.FromLat),
                        GetParameter(field.Entity, element.FromLong),
                        GetParameter(field.Entity, element.ToLat),
                        GetParameter(field.Entity, element.ToLong)
                    );

                case "length":
                    return new LengthOperation(inKey, outKey);

                case "timeofday":
                    return new TimeOfDayOperation(inKey, inType, outKey, outType, element.TimeComponent);

                case "xpath":
                    return new XPathOperation(inKey, outKey, outType, element.XPath);

                // validators
                case "containscharacters":
                    return new ContainsCharactersValidatorOperation(
                        inKey,
                        outKey,
                        element.Characters,
                        (ContainsCharacters)Enum.Parse(typeof(ContainsCharacters), element.ContainsCharacters, true),
                        element.Message,
                        element.Negated,
                        append
                    );

                case "datetimerange":
                    return new DateTimeRangeValidatorOperation(
                        inKey,
                        outKey,
                        (DateTime)Common.ObjectConversionMap[field.SimpleType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (DateTime)Common.ObjectConversionMap[field.SimpleType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Message,
                        element.Negated,
                        append
                    );

                case "domain":
                    if (element.Separator.Equals(DEFAULT)) {
                        element.Separator = COMMA;
                    }
                    var domain = element.Domain.Split(element.Separator.ToCharArray()).Select(s => Common.ObjectConversionMap[field.SimpleType](s));

                    return new DomainValidatorOperation(
                        inKey,
                        outKey,
                        domain,
                        element.Message,
                        element.Negated,
                        append
                    );

                case "json":
                    return new JsonValidatorOperation(inKey, outKey, append);

                case "notnull":
                    return new NotNullValidatorOperation(inKey, outKey, element.Message, element.Negated, append);

                case "fieldcomparison":
                    return new PropertyComparisonValidatorOperation(inKey, element.TargetField, outKey, element.Operator, element.Message, element.Negated, append);

                case "range":
                    return new RangeValidatorOperation(
                        inKey,
                        outKey,
                        (IComparable)Common.ObjectConversionMap[field.SimpleType](element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        (IComparable)Common.ObjectConversionMap[field.SimpleType](element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Message,
                        element.Negated,
                        append
                    );

                case "regex":
                    return new RegexValidatorOperation(
                        inKey,
                        outKey,
                        element.Pattern,
                        element.Message,
                        element.Negated,
                        append
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
                        element.Message,
                        element.Negated,
                        append
                    );

                case "stringlength":
                    return new StringLengthValidatorOperation(
                        inKey,
                        outKey,
                        Convert.ToInt32(element.LowerBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.LowerBoundType, true),
                        Convert.ToInt32(element.UpperBound),
                        (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), element.UpperBoundType, true),
                        element.Message,
                        element.Negated,
                        append
                    );

                case "typeconversion":
                    return new TypeConversionValidatorOperation(
                        inKey,
                        outKey,
                        Common.ToSystemType(element.Type),
                        element.Message,
                        element.Negated,
                        append
                    );

            }

            _log.Warn("{0} method is undefined.  It will not be used.", element.Method);
            return new EmptyOperation();
        }

        private IParameter GetParameter(string entity, string parameter) {
            Field f;
            return _process.TryGetField(parameter, entity, out f) ?
                f.ToParameter() :
                new Parameter(parameter, parameter);
        }

        private IParameter GetParameter(string entity, string parameter, IParameters parameters) {
            Field f;
            if (_process.TryGetField(parameter, entity, out f)) {
                return f.ToParameter();
            }
            if (parameters.ContainsKey(parameter)) {
                return parameters[parameter];
            }
            return new Parameter(parameter, parameter);
        }

    }
}