using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Transformalize.Readers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestEntity {
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
            var sql = SqlTemplates.CreateKeysTableVariable(entity.Keys);

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);", sql);
        }

        [Test]
        public void TestKeysTableVariableWithKeys() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var connection = entity.InputConnection;
            connection.BatchInsertSize = 2;
            var sql = SqlTemplates.CreateKeysTableVariable(entity.Keys) + SqlTemplates.BatchInsertKeyValues(entity.Keys, entity.GetKeys(), connection.Year, connection.BatchInsertSize);

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);
INSERT INTO @KEYS SELECT 1 UNION ALL SELECT 2;
INSERT INTO @KEYS SELECT 3 UNION ALL SELECT 4;", sql);
        }

        [Test]
        public void TestSelectByKeysSql() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var sql = SqlTemplates.SelectByKeys(entity);

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);
INSERT INTO @KEYS SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4;
SELECT t.[OrderDetailKey], t.[OrderKey], t.[ProductKey], [Quantity] = t.[Qty], t.[Price], t.[RowVersion], [Color] = t.[Properties].value('(/Properties/Color)[1]', 'NVARCHAR(64)'), [Size] = t.[Properties].value('(/Properties/Size)[1]', 'NVARCHAR(64)'), [Gender] = t.[Properties].value('(/Properties/Gender)[1]', 'NVARCHAR(64)')
FROM [dbo].[OrderDetail] t INNER JOIN @KEYS k ON (t.[OrderDetailKey] = k.[OrderDetailKey]);", sql);
        }
        
    }
}
