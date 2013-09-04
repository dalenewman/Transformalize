using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Transform_
{
    public class TransformFactory
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly TransformConfigurationElement _transform;
        private readonly IParametersReader _parametersReader;
        private readonly IParameters _parameters;
        
        public TransformFactory(TransformConfigurationElement transform, ITransformParametersReader transformParametersReader, IParametersReader parametersReader)
        {
            _transform = transform;
            _parametersReader = parametersReader;
            _parameters = transformParametersReader.Read(transform);
        }

        public AbstractTransform Create(string fieldName = "")
        {
            AbstractTransform transform = new EmptyTransform();
            switch (_transform.Method.ToLower())
            {
                case "convert":
                    transform = new ConvertTransform(_transform.To, _parameters);
                    break;

                case "replace":
                    transform = new ReplaceTransform(_transform.OldValue, _transform.NewValue, _parameters);
                    break;

                case "regexreplace":
                    transform = new RegexReplaceTransform(_transform.Pattern, _transform.Replacement, _transform.Count, _parameters);
                    break;

                case "insert":
                    transform = new InsertTransform(_transform.Index, _transform.Value, _parameters);
                    break;

                case "remove":
                    transform = new RemoveTransform(_transform.StartIndex, _transform.Length, _parameters);
                    break;

                case "trimstart":
                    transform = new TrimStartTransform(_transform.TrimChars, _parameters);
                    break;

                case "trimend":
                    transform = new TrimEndTransform(_transform.TrimChars, _parameters);
                    break;

                case "trim":
                    transform = new TrimTransform(_transform.TrimChars, _parameters);
                    break;

                case "substring":
                    transform = new SubstringTransform(_transform.StartIndex, _transform.Length, _parameters);
                    break;

                case "left":
                    transform = new LeftTransform(_transform.Length, _parameters);
                    break;

                case "right":
                    transform = new RightTransform(_transform.Length, _parameters);
                    break;

                case "map":
                    var equals = Process.MapEquals[_transform.Map];
                    var startsWith = Process.MapStartsWith.ContainsKey(_transform.Map)
                        ? Process.MapStartsWith[_transform.Map]
                        : new Map();
                    var endsWith = Process.MapEndsWith.ContainsKey(_transform.Map)
                        ? Process.MapEndsWith[_transform.Map]
                        : new Map();
                    transform = new MapTransform(new[] { @equals, startsWith, endsWith }, _parameters);
                    break;

                case "javascript":
                    var scripts = new Dictionary<string, Script>();
                    foreach (TransformScriptConfigurationElement script in _transform.Scripts)
                    {
                        scripts[script.Name] = Process.Scripts[script.Name];
                    }

                    transform = 
                        _parameters.Any()
                            ? new JavascriptTransform(_transform.Script, _parameters, scripts)
                            : new JavascriptTransform(_transform.Script, fieldName, scripts);
                    break;

                case "expression":
                    transform = _parameters.Any()
                        ? new ExpressionTransform(_transform.Expression, _parameters)
                        : new ExpressionTransform(fieldName, _transform.Expression, _parameters);
                    break;

                case "template":

                    var templates = new Dictionary<string, Template>();
                    foreach (TransformTemplateConfigurationElement template in _transform.Templates)
                    {
                        templates[template.Name] = Process.Templates[template.Name];
                    }

                    transform = 
                        _parameters.Any()
                            ? new TemplateTransform(_transform.Template, _transform.Model, _parameters, templates)
                            : new TemplateTransform(_transform.Template, fieldName, templates);
                    break;

                case "padleft":
                    transform = new PadLeftTransform(_transform.TotalWidth, _transform.PaddingChar[0], _parameters);
                    break;

                case "padright":
                    transform = new PadRightTransform(_transform.TotalWidth, _transform.PaddingChar[0], _parameters);
                    break;

                case "format":
                    transform = new FormatTransform(_transform.Format, _parameters);
                    break;

                case "dateformat":
                    transform = new DateFormatTransform(_transform.Format, _parameters);
                    break;

                case "toupper":
                    transform = new ToUpperTransform(_parameters);
                    break;

                case "tolower":
                    transform = new ToLowerTransform(_parameters);
                    break;

                case "concat":
                    transform = new ConcatTransform(_parameters);
                    break;

                case "join":
                    transform = new JoinTransform(_transform.Separator, _parameters);
                    break;

                case "tolocaltime":
                    transform = new ToLocalTimeTransform(_parameters);
                    break;

                case "tojson":
                    transform = new ToJsonTransform(_parameters);
                    break;

                case "fromxml":
                    transform = new FromXmlTransform(fieldName, _parameters);
                    break;
            }

            if (transform.RequiresParameters && !transform.Parameters.Any() || _transform.Parameter.Equals("*"))
            {
                transform.Parameters = _parametersReader.Read();
            }

            if(transform.Name == "Empty Transform")
                _log.Warn("{0} | {1} method is undefined.  It will not be used.", Process.Name, _transform.Method );
            return transform;

        }

    }
}