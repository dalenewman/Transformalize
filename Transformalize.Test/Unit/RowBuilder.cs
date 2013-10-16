using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Test.Unit
{
    public class RowBuilder {
        private readonly RowsBuilder _rowsBuilder;
        private readonly Row _row;

        public RowBuilder(ref RowsBuilder rowsBuilder, ref Row row) {
            _rowsBuilder = rowsBuilder;
            _row = row;
        }

        public RowBuilder Field(string field, object value) {
            _row[field] = value;
            return this;
        }

        public RowBuilder Row() {
            return _rowsBuilder.Row();
        }

        public IEnumerable<Row> ToRows() {
            return _rowsBuilder.ToRows();
        }

        public IOperation ToOperation() {
            return _rowsBuilder.ToOperation();
        }

    }
}