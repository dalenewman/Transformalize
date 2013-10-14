#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestTransforms : EtlProcessHelper
    {
        
    }

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