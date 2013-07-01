using NUnit.Framework;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestSqlTemplates {

        [Test]
        public void TestTruncateSql() {
            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'Test'
                )	TRUNCATE TABLE [dbo].[Test];
            ", SqlTemplates.TruncateTable("Test"));
        }

        [Test]
        public void TestDropSql() {
            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'TEST'
                )	DROP TABLE [dbo].[TEST];
            ", SqlTemplates.DropTable("TEST"));
        }
   
    }
}
