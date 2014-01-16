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

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Operations;
using Transformalize.Runner;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestWithProcess : EtlProcessHelper {

        private readonly Mock<IOperation> _entityKeysExtract;
        private static readonly ProcessConfigurationElement Element = new ProcessConfigurationReader("Test").Read();
        private readonly Process _process = new ProcessReader(Element, new Options()).Read();

        public TestWithProcess() {
            _entityKeysExtract = new Mock<IOperation>();
            _entityKeysExtract.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"OrderDetailKey", 1} },
                new Row { {"OrderDetailKey", 2} },
                new Row { {"OrderDetailKey", 3} },
                new Row { {"OrderDetailKey", 4} }
            });
        }

        [Test]
        public void TestEntityKeysToOperations() {
            var entity = _process.Entities.First();

            TestOperation(
                _entityKeysExtract.Object,
                new EntityInputKeysStore(entity)
                );

            var operations = TestOperation(
                new EntityKeysToOperations(entity)
                );

            Assert.AreEqual(1, operations.Count);
        }

        [Test]
        public void TestKeyInserts() {
            var entity = _process.Entities.First();
            _process.OutputConnection.IsReady();

            var rows = TestOperation(_entityKeysExtract.Object);

            Assert.AreEqual(4, rows.Count);

            var actual = SqlTemplates.BatchInsertValues(2, "@KEYS", entity.PrimaryKey.ToEnumerable().ToArray(), rows, _process.OutputConnection);
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
        public void TestKeysTableVariable() {
            var entity = _process.Entities.First();

            var actual = _process.OutputConnection.WriteTemporaryTable("@KEYS", entity.PrimaryKey.ToEnumerable().ToArray());
            const string expected = "DECLARE @KEYS AS TABLE([OrderDetailKey] INT);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSelectByKeysSql() {
            var entity = _process.Entities.First();

            var actual = SqlTemplates.Select(entity.Fields, entity.OutputName(), "@KEYS", _process.OutputConnection.Provider);

            const string expected = @"
SELECT
    l.[OrderDetailKey],
    l.[RowVersion] AS [OrderDetailRowVersion],
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
        public void TestWriteSql() {
            var actual = new SqlServerViewWriter().CreateSql(_process);

            Assert.AreEqual(@"CREATE VIEW [TestStar] AS
SELECT
    d.TflKey,
    d.TflBatchId,
    b.TflUpdate,
    [d].[Color],
    [d].[Gender],
    [d].[OrderDetailKey],
    [d].[OrderDetailRowVersion],
    [d].[OrderKey],
    [d].[Price],
    [d].[ProductKey],
    [d].[Properties],
    [d].[Quantity],
    [d].[Result],
    [d].[Size],
    ISNULL([d].[CustomerKey], 0) AS [CustomerKey],
    ISNULL([TestCustomer].[Address], '') AS [Address],
    ISNULL([TestCustomer].[City], '') AS [City],
    ISNULL([TestCustomer].[Country], '') AS [Country],
    ISNULL([TestCustomer].[CustomerValidation], '') AS [CustomerValidation],
    ISNULL([TestCustomer].[FirstName], '') AS [FirstName],
    ISNULL([TestCustomer].[LastName], '') AS [LastName],
    ISNULL([TestOrder].[OrderDate], '12/31/9999 12:00:00 AM') AS [OrderDate],
    ISNULL([TestOrder].[OrderValidation], '') AS [OrderValidation],
    ISNULL([TestProduct].[ProductName], 'None') AS [ProductName],
    ISNULL([TestCustomer].[State], '') AS [State]
FROM [TestOrderDetail] d
INNER JOIN TflBatch b ON (d.TflBatchId = b.TflBatchId AND b.ProcessName = 'Test')
LEFT OUTER JOIN TestOrder ON (d.[OrderKey] = TestOrder.[OrderKey])
LEFT OUTER JOIN TestCustomer ON (d.[CustomerKey] = TestCustomer.[CustomerKey])
LEFT OUTER JOIN TestProduct ON (d.[ProductKey] = TestProduct.[ProductKey])
;", actual);

            Console.Write(actual);
        }
    }
}