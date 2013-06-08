using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Transformalize.Readers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestEntity : EtlProcessHelper {
        [Test]
        public void TestGetKeys() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var orderDetailKeys = entity.GetKeys();

            Assert.AreEqual(4, orderDetailKeys.Count());
        }

        [Test]
        public void TestKeysTableVariable() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var sql = SqlTemplates.CreateTableVariable("@KEYS", entity);

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);", sql);
        }

        [Test]
        public void TestKeysTableVariableWithKeys() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 2;
            var sql = SqlTemplates.CreateTableVariable("@KEYS", entity) + SqlTemplates.BatchInsertValues("@KEYS", entity, entity.GetKeys());

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
            var sql = SqlTemplates.SelectByKeys(entity, entity.GetKeys());

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
        public void TestGetInputOperations() {
            var process = new ProcessReader("Test").GetProcess();
            
            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 1;
            entity.InputConnection.BatchSelectSize = 2;

            var operations = process.Entities["OrderDetail"].GetInputOperations();

            Assert.AreEqual(2, operations.Count());
        }

        [Test]
        public void TestGetInputOperation() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 1;
            entity.InputConnection.BatchSelectSize = 2;

            var operation = process.Entities["OrderDetail"].GetInputOperation();

            var results = TestOperation(operation);

            Assert.AreEqual(4, results.Count);


        }

    }
}
