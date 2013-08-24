/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Operations;
using Transformalize.Providers;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestWithProcess : EtlProcessHelper {

        private readonly Mock<IOperation> _entityKeysExtract;
        private readonly Process _process = new ProcessReader("Test").Read();

        public TestWithProcess() {

            _entityKeysExtract = new Mock<IOperation>();
            _entityKeysExtract.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row {{"OrderDetailKey", 1} },
                new Row {{"OrderDetailKey", 2} },
                new Row {{"OrderDetailKey", 3} },
                new Row {{"OrderDetailKey", 4} }
            });

        }

        [Test]
        public void TestKeysTableVariable() {
            var entity = Process.Entities.First();
            
            var actual = SqlTemplates.CreateTableVariable("KEYS", entity.PrimaryKey.ToEnumerable().ToArray());
            const string expected = "DECLARE @KEYS AS TABLE([OrderDetailKey] INT);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestKeyInserts() {

            var entity = Process.Entities.First();

            var rows = TestOperation(_entityKeysExtract.Object);

            Assert.AreEqual(4, rows.Count);

            var actual = SqlTemplates.BatchInsertValues(2, "@KEYS", entity.PrimaryKey.ToEnumerable().ToArray(), rows, false);
            const string expected = @"
INSERT INTO @KEYS
SELECT 1
UNION ALL SELECT 2;
INSERT INTO @KEYS
SELECT 3
UNION ALL SELECT 4;";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSelectByKeysSql() {

            var entity = Process.Entities.First();

            var actual = SqlTemplates.Select(entity.All, entity.OutputName(), "@KEYS");
            const string expected = @"
SELECT
    [Color] = l.[Properties].value('(/Properties/Color)[1]', 'NVARCHAR(64)'),
    [Gender] = l.[Properties].value('(/Properties/Gender)[1]', 'NVARCHAR(64)'),
    l.[OrderDetailKey],
    l.[OrderKey],
    l.[Price],
    l.[ProductKey],
    [Quantity] = l.[Qty],
    [Size] = l.[Properties].value('(/Properties/Size)[1]', 'NVARCHAR(64)')
FROM [TestOrderDetail] l WITH (NOLOCK)
INNER JOIN @KEYS r ON (l.[OrderDetailKey] = r.[OrderDetailKey])
OPTION (MAXDOP 1);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestEntityKeysToOperations() {

            var entity = Process.Entities.First();
            var entityKeysToOperations = new EntityKeysToOperations(entity);

            var operations = TestOperation(_entityKeysExtract.Object, entityKeysToOperations);
            Assert.AreEqual(1, operations.Count);
        }

        [Test]
        public void TestWriteSql()
        {

            var actual = new SqlServerViewWriter(_process).CreateSql();

            Assert.AreEqual(@"CREATE VIEW [TestOrderDetailStar] AS
SELECT
    [TestOrderDetail].[TflKey],
    [TestOrderDetail].[TflBatchId],
    b.[TflUpdate],
    [TestOrderDetail].[Color],
    [TestOrderDetail].[Gender],
    [TestOrderDetail].[OrderDetailKey],
    [TestOrderDetail].[OrderKey],
    [TestOrderDetail].[Price],
    [TestOrderDetail].[ProductKey],
    [TestOrderDetail].[Quantity],
    [TestOrderDetail].[Result],
    [TestOrderDetail].[Size],
    [TestOrderDetail].[CustomerKey],
    [TestOrder].[OrderDate],
    [TestCustomer].[Address],
    [TestCustomer].[City],
    [TestCustomer].[Country],
    [TestCustomer].[FirstName],
    [TestCustomer].[LastName],
    [TestCustomer].[State],
    [TestProduct].[ProductName]
FROM [TestOrderDetail]
INNER JOIN [TflBatch] b ON ([TestOrderDetail].TflBatchId = b.TflBatchId)
LEFT OUTER JOIN [TestOrder] ON ([TestOrderDetail].[OrderKey] = [TestOrder].[OrderKey])
LEFT OUTER JOIN [TestCustomer] ON ([TestOrderDetail].[CustomerKey] = [TestCustomer].[CustomerKey])
LEFT OUTER JOIN [TestProduct] ON ([TestOrderDetail].[ProductKey] = [TestProduct].[ProductKey])
;", actual);

            Console.Write(actual);
        }


    }
}
