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
        public void TestGetKeysSql() {
            var process = new ProcessReader("Test").GetProcess();
            
            var entity = process.Entities["OrderDetail"];
            entity.InputConnection.BatchInsertSize = 2;

            var sql = entity.CreateKeysSql(entity.GetKeys());

            Assert.AreEqual(@"DECLARE @KEYS AS TABLE([OrderDetailKey] INT NOT NULL);
INSERT INTO @KEYS SELECT 1 UNION ALL SELECT 2;
INSERT INTO @KEYS SELECT 3 UNION ALL SELECT 4;", sql);
        }
    }
}
