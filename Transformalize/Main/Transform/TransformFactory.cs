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

namespace Transformalize.Main {
    public class TransformFactory {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;

        public TransformFactory(Process process) {
            _process = process;
        }

        public AbstractTransform Create(TransformConfigurationElement element,
                                        ITransformParametersReader transformParametersReader,
                                        IParametersReader parametersReader, string fieldName = "") {
            var parameters = transformParametersReader.Read(element);
            AbstractTransform transform = new EmptyTransform();
            switch (element.Method.ToLower()) {
                case "convert":
                    transform = new ConvertTransform(element.To, parameters);
                    break;

                case "replace":
                    transform = new ReplaceTransform(element.OldValue, element.NewValue, parameters);
                    break;

                case "regexreplace":
                    transform = new RegexReplaceTransform(element.Pattern, element.Replacement, element.Count,
                                                          parameters);
                    break;

                case "insert":
                    transform = new InsertTransform(element.Index, element.Value, parameters);
                    break;

                case "remove":
                    transform = new RemoveTransform(element.StartIndex, element.Length, parameters);
                    break;

                case "trimstart":
                    transform = new TrimStartTransform(element.TrimChars, parameters);
                    break;

                case "trimend":
                    transform = new TrimEndTransform(element.TrimChars, parameters);
                    break;

                case "trim":
                    transform = new TrimTransform(element.TrimChars, parameters);
                    break;

                case "substring":
                    transform = new SubstringTransform(element.StartIndex, element.Length, parameters);
                    break;

                case "left":
                    transform = new LeftTransform(element.Length, parameters);
                    break;

                case "right":
                    transform = new RightTransform(element.Length, parameters);
                    break;

                case "gethashcode":
                    transform = new GetHashCodeTransform(parameters);
                    break;

                case "map":
                    var equals = _process.MapEquals[element.Map];
                    var startsWith = _process.MapStartsWith.ContainsKey(element.Map)
                                         ? _process.MapStartsWith[element.Map]
                                         : new Map();
                    var endsWith = _process.MapEndsWith.ContainsKey(element.Map)
                                       ? _process.MapEndsWith[element.Map]
                                       : new Map();
                    transform = new MapTransform(new[] { @equals, startsWith, endsWith }, parameters);
                    break;

                case "javascript":
                    var scripts = new Dictionary<string, Script>();
                    foreach (TransformScriptConfigurationElement script in element.Scripts) {
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    transform =
                        parameters.Any()
                            ? new JavascriptTransform(element.Script, parameters, scripts)
                            : new JavascriptTransform(element.Script, fieldName, scripts);
                    break;

                case "expression":
                    transform = parameters.Any()
                        ? new ExpressionTransform(element.Expression, parameters)
                        : new ExpressionTransform(fieldName, element.Expression, parameters);
                    break;

                case "template":

                    var templates = new Dictionary<string, Template>();
                    foreach (TransformTemplateConfigurationElement template in element.Templates) {
                        templates[template.Name] = _process.Templates[template.Name];
                    }

                    transform =
                        parameters.Any()
                            ? new TemplateTransform(element.Template, fieldName, element.Model, parameters, templates)
                            : new TemplateTransform(element.Template, fieldName, templates);
                    break;

                case "padleft":
                    transform = new PadLeftTransform(element.TotalWidth, element.PaddingChar[0], parameters);
                    break;

                case "padright":
                    transform = new PadRightTransform(element.TotalWidth, element.PaddingChar[0], parameters);
                    break;

                case "format":
                    transform = new FormatTransform(element.Format, parameters);
                    break;

                case "dateformat":
                    transform = new DateFormatTransform(element.Format, parameters);
                    break;

                case "toupper":
                    transform = new ToUpperTransform(parameters);
                    break;

                case "tolower":
                    transform = new ToLowerTransform(parameters);
                    break;

                case "concat":
                    transform = new ConcatTransform(parameters);
                    break;

                case "join":
                    transform = new JoinTransform(element.Separator, parameters);
                    break;

                case "tolocaltime":
                    transform = new ToLocalTimeTransform(parameters);
                    break;

                case "tojson":
                    transform = new ToJsonTransform(parameters);
                    break;

                case "fromxml":
                    transform = new FromXmlTransform(fieldName, parameters);
                    break;

                case "fromregex":
                    transform = new FromRegexTransform(fieldName, element.Pattern, parameters);
                    break;
            }

            if (transform.RequiresParameters && !transform.Parameters.Any() || element.Parameter.Equals("*")) {
                transform.Parameters = parametersReader.Read();
            }

            if (transform.Name == "Empty Transform")
                _log.Warn("{0} method is undefined.  It will not be used.", element.Method);
            return transform;
        }
    }
}