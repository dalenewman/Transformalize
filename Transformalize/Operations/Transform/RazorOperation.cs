using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NVelocity;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Template = Transformalize.Main.Template;

namespace Transformalize.Operations.Transform {

    public class VelocityOperation : TemplateOperation {
        private readonly VelocityContext _context;

        public VelocityOperation(string outKey, string outType, string template, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters)
            : base(outKey, outType, template, templates, parameters) {
            _context = new VelocityContext();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    foreach (var pair in Parameters) {
                        _context.Put(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
                    }
                    var sw = StringWriters.GetObject();
                    Velocity.Evaluate(_context, sw, string.Empty, Template);
                    var sb = sw.GetStringBuilder();
                    row[OutKey] = Common.ConversionMap[OutType](sb.ToString());
                    sb.Clear();
                    StringWriters.PutObject(sw);
                    foreach (var pair in Parameters) {
                        _context.Remove(pair.Value.Name);
                    }
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }

    public abstract class TemplateOperation : ShouldRunOperation {

        private readonly StringBuilder _builder = new StringBuilder();
        protected readonly string OutType;
        protected readonly IParameters Parameters;
        protected readonly string Template;

        protected TemplateOperation(string outKey, string outType, string template, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters)
            : base(string.Empty, outKey) {
            OutType = Common.ToSimpleType(outType);
            Parameters = parameters;

            CombineTemplates(templates, ref _builder);
            _builder.Append(template);
            Template = _builder.ToString();
        }

        private static void CombineTemplates(IEnumerable<KeyValuePair<string, Template>> templates, ref StringBuilder builder) {
            foreach (var pair in templates) {
                builder.AppendLine(pair.Value.Contents.Content);
            }
        }

    }

    public class RazorOperation : TemplateOperation {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly string _templateModelType;
        private readonly Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private readonly DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();

        public RazorOperation(string outKey, string outType, string template, string templateModelType, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters)
            : base(outKey, outType, template, templates, parameters) {

            _templateModelType = templateModelType;

            var type = templateModelType == "dynamic" ? typeof(DynamicViewBag) : typeof(Dictionary<string, object>);

            Razor.Compile(Template, type, outKey);
            _log.Debug("Compiled {0} template with key {1}.", templateModelType, outKey);
            Name = string.Format("RazorOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_templateModelType == "dynamic")
                        RunWithDynamic(row);
                    else
                        RunWithDictionary(row);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }

        private void RunWithDictionary(Row row) {
            foreach (var pair in Parameters) {
                _dictionaryContext[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[OutKey] = Common.ConversionMap[OutType](Razor.Run(OutKey, _dictionaryContext).Trim());
        }

        private void RunWithDynamic(Row row) {
            foreach (var pair in Parameters) {
                _dynamicViewBagContext.SetValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[OutKey] = Common.ConversionMap[OutType](Razor.Run(OutKey, _dynamicViewBagContext).Trim());
        }

    }
}