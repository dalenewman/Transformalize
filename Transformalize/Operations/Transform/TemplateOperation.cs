using System.Collections.Generic;
using System.Text;
using System.Threading;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class TemplateOperation : ShouldRunOperation {

        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly string _templateModelType;
        private readonly IParameters _parameters;
        private readonly Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private readonly DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();

        public TemplateOperation(string outKey, string template, string templateModelType, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters)
            : base(string.Empty, outKey) {
            _templateModelType = templateModelType;
            _parameters = parameters;

            //if (_parameters.ContainsKey(outKey)) {
            //    _parameters.Remove(outKey);
            //}

            CombineTemplates(templates, ref _builder);
            _builder.Append(template);

            var type = templateModelType == "dynamic" ? typeof(DynamicViewBag) : typeof(Dictionary<string, object>);

            Razor.Compile(_builder.ToString(), type, outKey);
            _log.Debug("Compiled {0} template with key {1}.", templateModelType, outKey);
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

        private static void CombineTemplates(IEnumerable<KeyValuePair<string, Template>> templates, ref StringBuilder builder) {
            foreach (var pair in templates) {
                builder.AppendLine(pair.Value.Content);
            }
        }

        private void RunWithDictionary(Row row) {
            foreach (var pair in _parameters) {
                _dictionaryContext[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[OutKey] = Razor.Run(OutKey, _dictionaryContext);
        }

        private void RunWithDynamic(Row row) {
            foreach (var pair in _parameters) {
                _dynamicViewBagContext.SetValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[OutKey] = Razor.Run(OutKey, _dynamicViewBagContext);
        }

    }
}