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
using Transformalize.Configuration;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations;
using Transformalize.Runner;

namespace Transformalize.Test.Unit
{
    [TestFixture]
    public class TestWithProcess : EtlProcessHelper
    {
        private readonly Mock<IOperation> _entityKeysExtract;
        private static readonly ProcessConfigurationElement Element = new ProcessConfigurationReader("Test").Read();
        private readonly Process _process = new ProcessReader(Element, new Options()).Read();

        public TestWithProcess()
        {
            _entityKeysExtract = new Mock<IOperation>();
            _entityKeysExtract.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                                   {
                                                                                                       new Row
                                                                                                           {
                                                                                                               {"OrderDetailKey", 1}
                                                                                                           },
                                                                                                       new Row
                                                                                                           {
                                                                                                               {"OrderDetailKey", 2}
                                                                                                           },
                                                                                                       new Row
                                                                                                           {
                                                                                                               {"OrderDetailKey", 3}
                                                                                                           },
                                                                                                       new Row
                                                                                                           {
                                                                                                               {"OrderDetailKey", 4}
                                                                                                           }
                                                                                                   });
        }

        [Test]
        public void TestEntityKeysToOperations()
        {
            Entity entity = _process.Entities.First();

            TestOperation(
                _entityKeysExtract.Object,
                new EntityInputKeysStore(_process, entity)
                );

            List<Row> operations = TestOperation(
                new EntityKeysToOperations(entity)
                );

            Assert.AreEqual(1, operations.Count);
        }

        [Test]
        public void TestKeyInserts()
        {
            Entity entity = _process.Entities.First();
            entity.OutputConnection.IsReady();

            List<Row> rows = TestOperation(_entityKeysExtract.Object);

            Assert.AreEqual(4, rows.Count);

            string actual = SqlTemplates.BatchInsertValues(2, "@KEYS", entity.PrimaryKey.ToEnumerable().ToArray(), rows, entity.OutputConnection);
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
        public void TestKeysTableVariable()
        {
            Entity entity = _process.Entities.First();

            string actual = _process.MasterEntity.OutputConnection.WriteTemporaryTable("@KEYS", entity.PrimaryKey.ToEnumerable().ToArray());
            const string expected = "DECLARE @KEYS AS TABLE([OrderDetailKey] INT);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSelectByKeysSql()
        {
            Entity entity = _process.Entities.First();

            string actual = SqlTemplates.Select(entity.All, entity.OutputName(), "@KEYS", entity.OutputConnection.Provider);

            const string expected = @"
SELECT
    l.[OrderDetailKey],
    l.[OrderKey],
    l.[Price],
    l.[ProductKey],
    l.[Properties],
    l.[Qty] AS [Quantity]
FROM [TestOrderDetail] l
INNER JOIN @KEYS r ON (l.[OrderDetailKey] = r.[OrderDetailKey])
OPTION (MAXDOP 2);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteSql()
        {
            string actual = new SqlServerViewWriter(_process).CreateSql();

            Assert.AreEqual(@"CREATE VIEW TestOrderDetailStar AS
SELECT
    TestOrderDetail.TflKey,
    TestOrderDetail.TflBatchId,
    b.TflUpdate,
    [TestOrderDetail].[Color],
    [TestOrderDetail].[Gender],
    [TestOrderDetail].[OrderDetailKey],
    [TestOrderDetail].[OrderKey],
    [TestOrderDetail].[Price],
    [TestOrderDetail].[ProductKey],
    [TestOrderDetail].[Properties],
    [TestOrderDetail].[Quantity],
    [TestOrderDetail].[Result],
    [TestOrderDetail].[Size],
    ISNULL([TestOrderDetail].[CustomerKey], 0) AS [CustomerKey],
    ISNULL([TestOrder].[OrderDate], '12/31/9999 12:00:00 AM') AS [OrderDate],
    ISNULL([TestCustomer].[Address], '') AS [Address],
    ISNULL([TestCustomer].[City], '') AS [City],
    ISNULL([TestCustomer].[Country], '') AS [Country],
    ISNULL([TestCustomer].[FirstName], '') AS [FirstName],
    ISNULL([TestCustomer].[LastName], '') AS [LastName],
    ISNULL([TestCustomer].[State], '') AS [State],
    ISNULL([TestProduct].[ProductName], 'None') AS [ProductName]
FROM TestOrderDetail
INNER JOIN TflBatch b ON (TestOrderDetail.TflBatchId = b.TflBatchId AND b.ProcessName = 'Test')
LEFT OUTER JOIN TestOrder ON (TestOrderDetail.[OrderKey] = TestOrder.[OrderKey])
LEFT OUTER JOIN TestCustomer ON (TestOrderDetail.[CustomerKey] = TestCustomer.[CustomerKey])
LEFT OUTER JOIN TestProduct ON (TestOrderDetail.[ProductKey] = TestProduct.[ProductKey])
;", actual);

            Console.Write(actual);
        }
    }
}