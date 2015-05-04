using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations {

    public class HtmlRowOperation : EntityOutputOperation {
        private readonly Entity _entity;
        private readonly string _htmlField;
        private readonly StringBuilder _htmlRow = new StringBuilder();

        public HtmlRowOperation(Entity entity, string htmlField)
            : base(entity) {
            _entity = entity;
            _htmlField = htmlField;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                _htmlRow.Clear();
                _htmlRow.Append("\t\t\t<tr>");

                foreach (var alias in _entity.Fields.WithFileOutput().Aliases()) {
                    _htmlRow.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(row[alias].ToString()));

                }
                foreach (var alias in _entity.CalculatedFields.WithFileOutput().Aliases()) {
                    _htmlRow.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(row[alias].ToString()));
                }

                _htmlRow.Append("</tr>");
                row[_htmlField] = _htmlRow.ToString();
                yield return row;
            }
        }
    }

}