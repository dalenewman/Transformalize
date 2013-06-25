using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Transformalize.Operations;
using Transformalize.Readers;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestEntity : EtlProcessHelper {

        private readonly Mock<IOperation> _entityKeysExtract;

        public TestEntity() {

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
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            
            var actual = SqlTemplates.CreateTableVariable("KEYS", entity.PrimaryKey);
            const string expected = "DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestKeyInserts() {

            var entity = new ProcessReader("Test").GetProcess().Entities["OrderDetail"];

            var rows = TestOperation(_entityKeysExtract.Object);

            Assert.AreEqual(4, rows.Count);

            var actual = SqlTemplates.BatchInsertValues(2, "@KEYS", entity.PrimaryKey, rows, 2005);
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

            var entity = new ProcessReader("Test").GetProcess().Entities["OrderDetail"];

            var actual = SqlTemplates.Select(entity.All, entity.Output, "@KEYS");
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
FROM [OrderDetail] l
INNER JOIN @KEYS r ON (l.[OrderDetailKey] = r.[OrderDetailKey])
OPTION (MAXDOP 1);";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestEntityKeysToOperations() {

            var process = new ProcessReader("Test").GetProcess();
            var entity = process.Entities["OrderDetail"];
            var entityKeysToOperations = new EntityKeysToOperations(entity);

            var operations = TestOperation(_entityKeysExtract.Object, entityKeysToOperations);
            Assert.AreEqual(1, operations.Count);
        }

    }
}
