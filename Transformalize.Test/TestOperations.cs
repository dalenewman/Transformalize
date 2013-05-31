using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Transformalize.Test {
    [TestFixture]
    public class TestOperations : EtlProcessHelper {


        [Test]
        public void TestOrdersExtract() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"OrderKey", 1}, {"CustomerKey", 1}, {"OrderDate",DateTime.Now}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} },
                new Row { {"OrderKey", 2}, {"CustomerKey", 2}, {"OrderDate",DateTime.Now}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} }
            });

            var results = TestOperation(mock.Object);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(4, results[0].Columns.Count());
        }

        [Test]
        public void TestCustomerExtract() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"CustomerKey", 1}, {"FirstName", "Dale"}, {"LastName","Newman"}, {"Address","306 Jones St."},{"City","Dowagiac"},{"State","MI"},{"Country","US"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} },
                new Row { {"CustomerKey", 2}, {"FirstName", "Eddie"}, {"LastName","Yerington"}, {"Address","222 Smith Ave."},{"City","Saint Joseph"},{"State","MI"},{"Country","US"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} }
            });

            var results = TestOperation(mock.Object);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(8, results[0].Columns.Count());
        }

        [Test]
        public void TestProductExtract() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"ProductKey", 1}, {"Name", "ReSharper"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} },
                new Row { {"ProductKey", 2}, {"Name", "PyCharm"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} }
            });

            var results = TestOperation(mock.Object);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(3, results[0].Columns.Count());
        }

        [Test]
        public void TestOrderDetailExtract() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row {{"OrderDetailKey", 1}, {"OrderKey", 1}, {"ProductKey", 1}, {"Price",1.0} , {"Quantity",1}, {"Color", "Red"}, {"Size", "Large"}, {"Gender","Female"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} },
                new Row {{"OrderDetailKey", 2}, {"OrderKey", 2}, {"ProductKey", 1}, {"Price",1.0} , {"Quantity",1}, {"Color", "Blue"}, {"Size", "Small"}, {"Gender", "Male"}, {"RowVersion", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }} }
            });

            var results = TestOperation(mock.Object);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(9, results[0].Columns.Count());
        }

    }
}
