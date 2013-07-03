using NUnit.Framework;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class TestEntity : EtlProcessHelper {

        private Process _process;
        private Entity _entity;

        [SetUp]
        public void SetUp() {
            _process = new ProcessReader("Test").GetProcess();

            new OutputRepository(_process).Init();
            new TflTrackerRepository(_process).Init();

            _entity = _process.Entities["OrderDetail"];
        }

        [Test]
        public void TestKeysTableVariableWithKeys() {

            var rows = TestOperation(new EntityKeysExtract(_entity));

            Assert.AreEqual(10000, rows.Count);

            var sql = SqlTemplates.CreateTableVariable("@KEYS", _entity.PrimaryKey)
                + SqlTemplates.BatchInsertValues(5, "@KEYS", _entity.PrimaryKey, rows, 2005);

            Assert.AreEqual(248939, sql.Length);

            const string expected = @"DECLARE @KEYS AS TABLE([OrderDetailKey] INT);
INSERT INTO @KEYS
SELECT 1
UNION ALL SELECT 2
UNION ALL SELECT 3
UNION ALL SELECT 4
UNION ALL SELECT 5;
INSERT INTO @KEYS
SELECT 6
UNION ALL SELECT 7";
            Assert.AreEqual(expected, sql.Substring(0, expected.Length));
        }

        [Test]
        public void TestSelectByKeysSql() {
            var keys = TestOperation(new EntityKeysExtract(_entity));

            const string expectedStart = @"SET NOCOUNT ON;
DECLARE @KEYS AS TABLE([OrderDetailKey] INT);
INSERT INTO @KEYS
SELECT 1
UNION ALL SELECT 2
UNION ALL SELECT 3
UNION ALL SELECT 4
UNION ALL SELECT 5
UNION ALL SELECT 6
UNION ALL SELECT 7";

            const string expectedEnd = @"SELECT
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

            var actual = _entity.EntitySqlWriter.SelectByKeys(keys);

            Assert.AreEqual(expectedStart, actual.Substring(0, expectedStart.Length));
            Assert.AreEqual(expectedEnd, actual.Substring(actual.Length - expectedEnd.Length));
        }

        [Test]
        public void TestEntityKeysToOperations() {

            _entity.InputConnection.InputBatchSize = 200;

            var entityKeyExtract = new EntityKeysExtract(_entity);
            var entityKeysToOperations = new EntityKeysToOperations(_entity);

            var operations = TestOperation(entityKeyExtract, entityKeysToOperations);
            Assert.AreEqual(10000 / 200, operations.Count);
        }

    }
}
