using System.Collections.Generic;
using Moq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Test.Unit {
    public class RowsBuilder {

        private readonly List<Row> _rows = new List<Row>();
        public RowsBuilder() { }

        public RowsBuilder(IEnumerable<Row> rows) {
            _rows.AddRange(rows);
        }

        public RowBuilder Row() {
            return Consume(new Row());
        }

        public RowBuilder Row(string key, object value) {
            return Consume(new Row() { { key, value } });
        }

        private RowBuilder Consume(Row row) {
            _rows.Add(row);
            var rowsBuilder = this;
            return new RowBuilder(ref rowsBuilder, ref row);
        }

        public IEnumerable<Row> ToRows() {
            return _rows;
        }

        public IOperation ToOperation() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(ToRows());
            return mock.Object;
        }
    }
}