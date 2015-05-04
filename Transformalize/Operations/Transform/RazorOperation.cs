using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;
using Template = Transformalize.Main.Template;

namespace Transformalize.Operations.Transform {

    public class RazorOperation : TemplateOperation {

        private readonly string _templateModelType;
        private readonly Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private readonly DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();

        public RazorOperation(string outKey, string outType, string template, string templateModelType, IEnumerable<KeyValuePair<string, Template>> templates, IParameters parameters, ILogger logger)
            : base(outKey, outType, template, templates, parameters) {

            _templateModelType = templateModelType;

            var type = templateModelType == "dynamic" ? typeof(DynamicViewBag) : typeof(Dictionary<string, object>);

            Razor.Compile(Template, type, outKey, Logger);
            logger.Debug("Compiled {0} template with key {1}.", templateModelType, outKey);
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