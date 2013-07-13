/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
    b.[TflUpdate],
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
INNER JOIN [TflBatch] b ON ([TestOrderDetail].TflBatchId = b.TflBatchId)
INNER JOIN [TestOrder] ON ([TestOrderDetail].[OrderKey] = [TestOrder].[OrderKey])
INNER JOIN [TestCustomer] ON ([TestOrderDetail].[CustomerKey] = [TestCustomer].[CustomerKey])
INNER JOIN [TestProduct] ON ([TestOrderDetail].[ProductKey] = [TestProduct].[ProductKey])
;", actual);
        }
   
    }
}
