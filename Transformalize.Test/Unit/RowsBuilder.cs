using System.Collections.Generic;
using Moq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Test.Unit
{
    public class RowsBuilder {
        private readonly IList<Row> _rows = new List<Row>();

        public RowBuilder Row() {
            var row = new Row();
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