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

using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Operations.Validate;
using System.Linq;

namespace Transformalize.Main {

    public class TransformOperationFactory {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;

        public TransformOperationFactory(Process process) {
            _process = process;
        }

        public AbstractOperation Create(Field field, TransformConfigurationElement element, IParameters parameters) {

            var hasParameters = parameters.Count > 0;
            var inKey = hasParameters ? parameters[0].Name : field.Alias;
            var inType = hasParameters ? parameters[0].SimpleType : field.SimpleType;
            var append = !string.IsNullOrEmpty(element.AppendTo);
            var outKey = append ? element.AppendTo : field.Alias;
            var outType = append ? _process.GetField(element.AppendTo, field.Entity).SimpleType : field.SimpleType;

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
                        element.Left,
                        element.Operator,
                        element.Right,
                        element.Then,
                        element.Else,
                        parameters,
                        outKey,
                        outType
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
                    var equals = _process.MapEquals[element.Map];
                    var startsWith = _process.MapStartsWith.ContainsKey(element.Map) ? _process.MapStartsWith[element.Map] : new Map();
                    var endsWith = _process.MapEndsWith.ContainsKey(element.Map) ? _process.MapEndsWith[element.Map] : new Map();
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

                // validators
                case "containscharacters":
                    return new ContainsCharactersOperation(
                        inKey,
                        outKey,
                        element.Characters,
                        element.ContainsCharacters,
                        element.Message,
                        element.Negated,
                        append
                    );

                case "datetimerange":
                    return new DateTimeRangeOperation(
                        inKey,
                        outKey,
                        element.LowerBound,
                        element.LowerBoundType,
                        element.UpperBound,
                        element.UpperBoundType,
                        element.Message,
                        element.Negated,
                        append
                    );

                case "domain":
                    var domain = element.Domain.Split(element.Separator.ToCharArray()).Select(s => Common.ObjectConversionMap[field.SimpleType](s));

                    return new DomainOperation(
                        inKey,
                        outKey,
                        domain,
                        element.Message,
                        element.Negated,
                        append
                    );

                case "parsejson":
                    return new ParseJsonOperation(inKey, outKey, append);

                case "notnull":
                    return new NotNullOperation(inKey, outKey, element.Message, element.Negated, append);

                case "fieldcomparison":
                    return new PropertyComparisonOperation(inKey, element.Field, outKey, element.Operator, element.Message, element.Negated, append);

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
    }
}