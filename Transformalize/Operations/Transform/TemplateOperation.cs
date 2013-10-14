using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class TemplateOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _templateModelType;
        private readonly IParameters _parameters;
        private Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();

        public TemplateOperation(string outKey, string template, string templateModelType, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters) {
            _outKey = outKey;
            _templateModelType = templateModelType;
            _parameters = parameters;

            CombineTemplates(templates, ref _builder);
            _builder.Append(template);

            var type = templateModelType == "dynamic" ? typeof(DynamicViewBag) : typeof(Dictionary<string, object>);

            Razor.Compile(_builder.ToString(), type, _outKey);
            _log.Debug("Compiled {0} template with key {1}.", templateModelType, _outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (_templateModelType == "dynamic")
                    RunWithDynamic(row);
                else
                    RunWithDictionary(row);

                yield return row;
            }
        }

        private static void CombineTemplates(IEnumerable<KeyValuePair<string, Template>> templates, ref StringBuilder builder) {
            foreach (var pair in templates) {
                builder.AppendLine(pair.Value.Content);
            }
        }

        private void RunWithDictionary(Row row) {
            _dictionaryContext.Add(_outKey, row[_outKey]);
            foreach (var pair in _parameters) {
                _dictionaryContext[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[_outKey] = Razor.Run(_outKey, _dictionaryContext);
        }

        private void RunWithDynamic(Row row) {
            _dynamicViewBagContext.SetValue(_outKey, row[_outKey]);
            foreach (var pair in _parameters) {
                _dynamicViewBagContext.SetValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[_outKey] = Razor.Run(_outKey, _dynamicViewBagContext);
        }

        public override void Dispose() {
            _dictionaryContext = null;
            _dynamicViewBagContext = null;
            base.Dispose();
        }
    }
}