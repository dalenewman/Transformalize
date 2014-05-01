#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Linq;
using NUnit.Framework;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Libs.Dapper;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class NorthWindUpdates {
        private const string FILE = "NorthWind.xml";
        private readonly Logger _log = LogManager.GetLogger("tfl");

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Info);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Go() {

            _log.Info("***** RUN 00 * INITIALIZE ******");
            var options = new Options { Mode = "init" };
            var process = ProcessFactory.Create(FILE, options)[0];
            process.Run();

            _log.Info("***** RUN 01 * FIRST RUN ******");
            options = new Options { RenderTemplates = false, LogLevel = LogLevel.Info };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(2155, process["Order Details"].Inserts);
            Assert.AreEqual(830, process["Orders"].Inserts);
            Assert.AreEqual(91, process["Customers"].Inserts);
            Assert.AreEqual(9, process["Employees"].Inserts);
            Assert.AreEqual(77, process["Products"].Inserts);
            Assert.AreEqual(29, process["Suppliers"].Inserts);
            Assert.AreEqual(8, process["Categories"].Inserts);
            Assert.AreEqual(3, process["Shippers"].Inserts);

            _log.Info("***** RUN 02 * NO CHANGES ******");
            options = new Options { RenderTemplates = false, LogLevel = LogLevel.Info };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(0, process["Order Details"].Inserts);
            Assert.AreEqual(0, process["Orders"].Inserts);
            Assert.AreEqual(0, process["Customers"].Inserts);
            Assert.AreEqual(0, process["Employees"].Inserts);
            Assert.AreEqual(0, process["Products"].Inserts);
            Assert.AreEqual(0, process["Suppliers"].Inserts);
            Assert.AreEqual(0, process["Categories"].Inserts);
            Assert.AreEqual(0, process["Shippers"].Inserts);

            _log.Info("***** RUN 03 * ADD 1 ORDER DETAIL ******");
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity, Discount) values(10261,41,7.70,2,0);");
            }

            options = new Options { RenderTemplates = false, LogLevel = LogLevel.Info};
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(1, process["Order Details"].Inserts);



            _log.Info("***** RUN 04 * ADD 1 ORDER ******");
            using (var cn = process["Orders"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("insert into [Orders](CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry)values('HILAA',6,GETDATE(),GETDATE(),GETDATE(),3,1.00,'Test Name 1','Test Address 1','Test City 1',NULL,'11111','USA')");
            }

            options = new Options { RenderTemplates = false };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(1, process["Orders"].Inserts);

            _log.Info("***** RUN 05 * ADD 2 CUSTOMERS ******");
            using (var cn = process["Customers"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("insert into [Customers](CustomerId,CompanyName,ContactName,ContactTitle,[Address],City,PostalCode,Country,Phone) values ('AAAAA','Company A','A','A','A','A','AAAAA','USA','111-222-3333'), ('BBBBB','Company B','B','B','B','B','BBBBB','USB','111-222-3333');");
            }

            options = new Options { RenderTemplates = false };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(2, process["Customers"].Inserts);


            _log.Info("***** RUN 06 * INCREASE PRICE OF PRODUCT(23) ******");
            decimal inputSum;
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                inputSum = cn.Query<decimal>("SELECT SUM(UnitPrice) FROM [Order Details] WHERE ProductID = 57;").First();
            }

            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("update [Order Details] set UnitPrice = UnitPrice + .99 where ProductID = 57");
            }

            options = new Options { RenderTemplates = false };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            decimal outputSum;
            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                outputSum = cn.Query<decimal>("SELECT SUM(OrderDetailsUnitPrice) FROM NorthWindOrderDetails WHERE OrderDetailsProductID = 57;").First();
            }

            Assert.AreEqual(23, process["Order Details"].Updates);
            Assert.AreEqual(inputSum + (23 * .99M), outputSum);


            _log.Info("***** RUN 07 * UPDATE SHIP COUNTRY WHICH SHOULD AFFECT COUNTRY EXCHANGE CALCULATED FIELD ******");

            using (var cn = process["Orders"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("update Orders set ShipCountry = 'USA' where OrderID = 10250;");
            }
            string preUpdate;
            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                preUpdate = cn.Query<string>("SELECT TOP 1 CountryExchange FROM NorthWindOrderDetails WHERE OrderDetailsOrderID = 10250 ORDER BY OrderDetailsProductID ASC;").First();
            }

            options = new Options { RenderTemplates = false };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            string postUpdate;
            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                postUpdate = cn.Query<string>("SELECT TOP 1 CountryExchange FROM NorthWindOrderDetails WHERE OrderDetailsOrderID = 10250 ORDER BY OrderDetailsProductID ASC;").First();
            }

            Assert.AreNotEqual(preUpdate, postUpdate);

            _log.Info("***** RUN 08 * UPDATE PART OF ORDER DETAIL KEY, INTERPRETED AS AN INSERT, AND SUBSEQUENT DELETE ******");
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("UPDATE [Order Details] SET ProductID = 10 WHERE OrderId = 10248 AND ProductID = 11;");
            }

            options = new Options { RenderTemplates = false };
            process = ProcessFactory.Create(FILE, options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            process.Run();

            Assert.AreEqual(1, process["Order Details"].Inserts);

            _log.Info("***** RUN 09 * HANDLE DELETE ******");
            options = new Options { Mode = "delete" };
            process = ProcessFactory.Create(FILE, options)[0];
            process.Run();

            Assert.AreEqual(1, process["Order Details"].Deletes);

            Reset();
        }

        public void Reset() {
            _log.Info("***** RESET ******");
            var options = new Options { RenderTemplates = false };
            var process = ProcessFactory.Create(FILE, options)[0];
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                cn.Execute("delete from [Order Details] where OrderID = 10261 and ProductID = 41;");
                cn.Execute("delete from [Orders] where OrderID = (select top 1 OrderId from Orders order by OrderID desc)");
                cn.Execute("delete from [Customers]	WHERE CustomerId IN ('AAAAA','BBBBB')");
                cn.Execute("update [Order Details] set UnitPrice = UnitPrice - .99 where ProductID = 57;");
                cn.Execute("update Orders set ShipCountry = 'Brazil' where OrderID = 10250;");
                cn.Execute("update [Order Details] set ProductID = 11 where OrderId = 10248 and ProductID = 10;");
            }

            Assert.AreEqual(true, true);
        }
    }
}