using NUnit.Framework;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Readers;
using Transformalize.Rhino.Etl.Core.Operations;
using Transformalize.Writers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestEntity : EtlProcessHelper {

        [Test]
        public void TestKeysTableVariable() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var sql = string.Format(@"DECLARE @KEYS AS TABLE({0});", new FieldSqlWriter(entity.Keys).Name().DataType().NotNull());

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);", sql);
        }

        [Test]
        public void TestKeysTableVariableWithKeys() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 2;

            var keys = TestOperation(new EntityKeysExtract(entity));

            var writer = new FieldSqlWriter(entity.Keys).Name().DataType().NotNull();
            var sql = string.Format(@"DECLARE @KEYS AS TABLE({0});", writer )
                + entity.EntitySqlWriter.BatchInsertValues("@KEYS", entity.Keys, keys);

            Assert.AreEqual(
@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);
INSERT INTO @KEYS([OrderDetailKey])
SELECT 1 UNION ALL SELECT 2;
INSERT INTO @KEYS([OrderDetailKey])
SELECT 3 UNION ALL SELECT 4;", sql);
        }

        [Test]
        public void TestSelectByKeysSql() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var keys = TestOperation(new EntityKeysExtract(entity));
            var sql = entity.EntitySqlWriter.SelectByKeys(keys);

            Assert.AreEqual(
@"SET NOCOUNT ON;
DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);
INSERT INTO @KEYS([OrderDetailKey])
SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4;
SELECT t.[OrderDetailKey], t.[OrderKey], t.[ProductKey], [Quantity] = t.[Qty], t.[Price], t.[RowVersion], [Color] = t.[Properties].value('(/Properties/Color)[1]', 'NVARCHAR(64)'), [Size] = t.[Properties].value('(/Properties/Size)[1]', 'NVARCHAR(64)'), [Gender] = t.[Properties].value('(/Properties/Gender)[1]', 'NVARCHAR(64)')
FROM [dbo].[OrderDetail] t
INNER JOIN @KEYS k ON (t.[OrderDetailKey] = k.[OrderDetailKey]);", sql);
        }

        [Test]
        public void TestEntityKeysExtract() {
            var process = new ProcessReader("Test").GetProcess();
            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 1;
            entity.InputConnection.BatchSelectSize = 2;

            var operation = new EntityKeysExtract(entity);
            var results = TestOperation(operation);
            Assert.AreEqual(4, results.Count);
        }

        [Test]
        public void TestEntityKeysToOperations() {

            var process = new ProcessReader("Test").GetProcess();
            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 1;
            entity.InputConnection.BatchSelectSize = 2;

            var entityKeyExtract = new EntityKeysExtract(entity);
            var entityKeysToOperations = new EntityKeysToOperations(entity);

            var operations = TestOperation(entityKeyExtract, entityKeysToOperations);
            Assert.AreEqual(2, operations.Count);
        }

        [Test]
        public void TestEntityExtract() {

            var process = new ProcessReader("Test").GetProcess();
            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 1;
            entity.InputConnection.BatchSelectSize = 2;

            var entityKeyExtract = new EntityKeysExtract(entity);
            var entityKeysToOperations = new EntityKeysToOperations(entity);
            var entityExtract = new ConventionSerialUnionAllOperation();

            var rows = TestOperation(entityKeyExtract, entityKeysToOperations, entityExtract);
            Assert.AreEqual(4, rows.Count);
        }

        [Test]
        public void TestEntityToOutput() {

            var process = new ProcessReader("Test").GetProcess();
            var entity = process.Entities["OrderDetail"];

            var entityKeyExtract = new EntityKeysExtract(entity);
            var entityKeysToOperations = new EntityKeysToOperations(entity);
            var entityExtract = new ConventionSerialUnionAllOperation();
            var entityToOutput = new EntityToOutput(entity);

            var rows = TestOperation(
                entityKeyExtract,
                entityKeysToOperations,
                entityExtract,
                entityToOutput
            );
            
            var writer = new VersionWriter(entity);
            writer.WriteEndVersion(entityKeyExtract.End);

            Assert.AreEqual(0, rows.Count);
        }

    }
}
