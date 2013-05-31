using NUnit.Framework;

namespace Transformalize.Test {
    [TestFixture]
    public class TestOutput {

        [Test]
        public void TestTruncateSql()
        {
            var output = new Configuration.ProcessConfiguration() {Name="Test", Output="Test"};

            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'Test'
                )	TRUNCATE TABLE [Test];
            ", output.TruncateOutputSql());
        }

        [Test]
        public void TestDropSql() {
            var output = new Configuration.ProcessConfiguration() { Name = "TEST", Output="TEST" };

            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'TEST'
                )	DROP TABLE [TEST];
            ", output.DropOutputSql());
        }    
    }
}
