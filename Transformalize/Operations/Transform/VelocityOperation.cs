using System.Collections.Generic;
using Transformalize.Libs.NVelocity;
using Transformalize.Libs.NVelocity.App;
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
}