using NUnit.Framework;
using Transformalize.Data;
using Transformalize.Readers;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestViewWriter {

        [Test]
        public void TestWriteSql() {
            var process = new ProcessReader("Test").Read();
            var actual = new SqlServerViewWriter(ref process).CreateSql();
            Assert.AreEqual(@"CREATE VIEW [TestOrderDetailStar] AS
SELECT
    [TestOrderDetail].[TflKey],
    [TestOrderDetail].[TflBatchId],
    [TestOrderDetail].[Color],
    [TestOrderDetail].[Gender],
    [TestOrderDetail].[OrderDetailKey],
    [TestOrderDetail].[OrderKey],
    [TestOrderDetail].[Price],
    [TestOrderDetail].[ProductKey],
    [TestOrderDetail].[Quantity],
    [TestOrderDetail].[Result],
    [TestOrderDetail].[Size],
    [TestOrderDetail].[CustomerKey],
    [TestOrder].[OrderDate],
    [TestCustomer].[Address],
    [TestCustomer].[City],
    [TestCustomer].[Country],
    [TestCustomer].[FirstName],
    [TestCustomer].[LastName],
    [TestCustomer].[State],
    [TestProduct].[ProductName]
FROM [TestOrderDetail]
INNER JOIN [TestOrder] ON ([TestOrderDetail].[OrderKey] = [TestOrder].[OrderKey])
INNER JOIN [TestCustomer] ON ([TestOrderDetail].[CustomerKey] = [TestCustomer].[CustomerKey])
INNER JOIN [TestProduct] ON ([TestOrderDetail].[ProductKey] = [TestProduct].[ProductKey])
;", actual);
        }
   
    }
}
