using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Processes {
    public class HtmlRowOperation : EntityOutputOperation {
        private readonly string _htmlField;
        private readonly StringBuilder _htmlRow = new StringBuilder();

        public HtmlRowOperation(Entity entity, string htmlField)
            : base(entity) {
            _htmlField = htmlField;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                _htmlRow.Clear();
                _htmlRow.Append("\t\t\t<tr>");
                foreach (var field in OutputFields) {
                    _htmlRow.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(row[field].ToString()));
                    row.Remove(field);
                }
                _htmlRow.Append("</tr>");
                row[_htmlField] = _htmlRow.ToString();
                yield return row;
            }
        }
    }

}