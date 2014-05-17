using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Processes {
    public class HtmlRowOperation : EntityOutputOperation {
        private readonly Entity _entity;
        private readonly string _htmlField;
        private readonly StringBuilder _htmlRow = new StringBuilder();

        public HtmlRowOperation(Entity entity, string htmlField)
            : base(entity)
        {
            _entity = entity;
            _htmlField = htmlField;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                _htmlRow.Clear();
                _htmlRow.Append("\t\t\t<tr>");

                foreach (var pair in _entity.Fields.Where(f => f.Value.FileOutput)) {
                    _htmlRow.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(row[pair.Value.Alias].ToString()));

                }
                foreach (var pair in _entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                    _htmlRow.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(row[pair.Value.Alias].ToString()));
                }

                _htmlRow.Append("</tr>");
                row[_htmlField] = _htmlRow.ToString();
                yield return row;
            }
        }
    }

}