using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Test.Unit.Builders
{
    public class RowBuilder {
        private readonly RowsBuilder _rowsBuilder;
        private readonly Row _row;

        public RowBuilder(ref RowsBuilder rowsBuilder, ref Row row) {
            _rowsBuilder = rowsBuilder;
            _row = row;
        }

        public RowBuilder Field(string key, object value) {
            _row[key] = value;
            return this;
        }

        public RowBuilder Row() {
            return _rowsBuilder.Row();
        }

        public RowBuilder Row(string key, object value)
        {
            return _rowsBuilder.Row(key, value);
        }

        public IEnumerable<Row> ToRows() {
            return _rowsBuilder.ToRows();
        }

        public IOperation ToOperation() {
            return _rowsBuilder.ToOperation();
        }

    }
}