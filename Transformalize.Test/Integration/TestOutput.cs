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
    [LoadDate] DATETIME,
    [OrderDate] DATETIME,
    [OrderDetailKey] INT NOT NULL,
    [OrderKey] INT,
    [Price] DECIMAL(10,5),
    [ProductKey] INT,
    [ProductName] NVARCHAR(100),
    [Quantity] INT,
    [RowVersion] ROWVERSION,
    [Size] NVARCHAR(64),
    [State] NVARCHAR(2),
    [TimeKey] INT,
    CONSTRAINT [Pk_OrderDetail_OrderDetailKey_ASC] PRIMARY KEY CLUSTERED (
        [OrderDetailKey] ASC
    ) WITH (IGNORE_DUP_KEY = ON)
);
", actual);
        }

    }
}
