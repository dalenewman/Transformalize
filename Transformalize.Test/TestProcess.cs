using NUnit.Framework;
using Transformalize.Model;

namespace Transformalize.Test {
    [TestFixture]
    public class TestProcess {

        [Test]
        public void TestTruncateSql() {
            var output = new Process() { Name = "Test", Output = "Test" };

            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'Test'
                )	TRUNCATE TABLE [dbo].[Test];
            ", output.TruncateOutputSql());
        }

        [Test]
        public void TestDropSql() {
            var output = new Process() { Name = "TEST", Output = "TEST" };

            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'TEST'
                )	DROP TABLE [dbo].[TEST];
            ", output.DropOutputSql());
        }    

        [Test]
        public void RunProcess() {
            var process = new TransformalizeProcess("Test");
            process.Execute();
        }

    }
}
