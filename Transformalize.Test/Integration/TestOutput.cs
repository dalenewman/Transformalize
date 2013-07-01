using NUnit.Framework;
using Transformalize.Readers;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestOutput {

        [Test]
        public void TestCreateSql() {
            var process = new ProcessReader("Test").GetProcess();
            var actual = process.CreateOutputSql();

            Assert.AreEqual(@"
CREATE TABLE [dbo].[OrderDetail](
    [Address] NVARCHAR(100),
    [City] NVARCHAR(50),
    [Color] NVARCHAR(64),
    [Country] NVARCHAR(2),
    [CustomerKey] INT,
    [FirstName] NVARCHAR(50),
    [Gender] NVARCHAR(64),
    [LastName] NVARCHAR(50),
    [OrderDate] DATETIME,
    [OrderDetailKey] INT NOT NULL,
    [OrderKey] INT,
    [Price] DECIMAL(10,5),
    [ProductKey] INT,
    [ProductName] NVARCHAR(100),
    [Quantity] INT,
    [Result] TINYINT,
    [Size] NVARCHAR(64),
    [State] NVARCHAR(2),
    [TflId] INT NOT NULL,
    [TflKey] INT NOT NULL IDENTITY(1,1) UNIQUE CLUSTERED,
    CONSTRAINT [Pk_OrderDetail_OrderDetailKey_ASC] PRIMARY KEY (
        [OrderDetailKey] ASC
    ) WITH (IGNORE_DUP_KEY = ON)
);
", actual);
        }

    }
}
