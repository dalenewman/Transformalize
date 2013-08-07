using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Transform_
{
    public class TransformFactory
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly TransformConfigurationElement _transform;
        private readonly IFields _results;
        private readonly IParameters _parameters;

        public TransformFactory(Process process, TransformConfigurationElement transform, ITransformParametersReader transformParametersReader, IFieldsReader resultsReader)
        {
            _process = process;
            _transform = transform;
            _parameters = transformParametersReader.Read() ?? new Parameters();
            _results = resultsReader.Read() ?? new Fields();
        }

        public AbstractTransform Create(string fieldName = "")
        {

            switch (_transform.Method.ToLower())
            {
                case "replace":
                    return new ReplaceTransform(_transform.OldValue, _transform.NewValue);

                case "regexreplace":
                    return new RegexReplaceTransform(_transform.Pattern, _transform.Replacement, _transform.Count);

                case "insert":
                    return new InsertTransform(_transform.Index, _transform.Value);

                case "remove":
                    return new RemoveTransform(_transform.StartIndex, _transform.Length);

                case "trimstart":
                    return new TrimStartTransform(_transform.TrimChars);

                case "trimend":
                    return new TrimEndTransform(_transform.TrimChars);

                case "trim":
                    return new TrimTransform(_transform.TrimChars);

                case "substring":
                    return new SubstringTransform(_transform.StartIndex, _transform.Length);

                case "left":
                    return new LeftTransform(_transform.Length);

                case "right":
                    return new RightTransform(_transform.Length);

                case "map":
                    var equals = _process.MapEquals[_transform.Map];
                    var startsWith = _process.MapStartsWith.ContainsKey(_transform.Map)
                                         ? _process.MapStartsWith[_transform.Map]
                                         : new Dictionary<string, object>();
                    var endsWith = _process.MapEndsWith.ContainsKey(_transform.Map)
                                       ? _process.MapEndsWith[_transform.Map]
                                       : new Dictionary<string, object>();
                    return new MapTransform(new[] {@equals, startsWith, endsWith});

                case "javascript":
                    var scripts = new Dictionary<string, Script>();
                    foreach (TransformScriptConfigurationElement script in _transform.Scripts)
                    {
                        scripts[script.Name] = _process.Scripts[script.Name];
                    }

                    return
                        _parameters.Any()
                            ? new JavascriptTransform(_transform.Script, _parameters, _results, scripts)
                            : new JavascriptTransform(_transform.Script, fieldName, scripts);

                case "template":
                    return
                        _parameters.Any()
                            ? new TemplateTransform(_transform.Template, _transform.Model, _parameters, _results)
                            : new TemplateTransform(_transform.Template, fieldName);

                case "padleft":
                    return new PadLeftTransform(_transform.TotalWidth, _transform.PaddingChar[0]);

                case "padright":
                    return new PadRightTransform(_transform.TotalWidth, _transform.PaddingChar[0]);

                case "format":
                    return new FormatTransform(_transform.Format, _parameters, _results);

                case "dateformat":
                    return new DateFormatTransform(_transform.Format, _parameters, _results);

                case "toupper":
                    return new ToUpperTransform();

                case "tolower":
                    return new ToLowerTransform();

                case "concat":
                    return new ConcatTransform(_parameters, _results);

                case "join":
                    return new JoinTransform(_transform.Separator, _parameters, _results);

                case "split":
                    return new SplitTransform(_transform.Separator, _parameters, _results);

                case "copy":
                    return new CopyTransform(_parameters, _results);

            }

            _log.Warn("{0} | {1} method is undefined.  It will not be used.");
            return new EmptyTransform();

        }

    }
}