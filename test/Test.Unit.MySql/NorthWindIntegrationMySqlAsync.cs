#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Console;
using Transformalize.Providers.MySql;
using Transformalize.Providers.MySql.Autofac;
using Transformalize.Transforms.Jint.Autofac;

namespace Test {

   /// <summary>
   /// The streaming counterpart to <see cref="NorthWindIntegrationMySql"/>.
   ///
   /// This shares the container's northwind/northwindstar databases with the synchronous test, so
   /// the delta mutations below deliberately use different values than that test does. MySQL's
   /// ON UPDATE CURRENT_TIMESTAMP only advances modified_at when a column actually changes value,
   /// and modified_at is this arrangement's version field. Re-applying the synchronous test's exact
   /// values would leave the version untouched, no delta would be detected, and the update
   /// assertions would fail depending on which class ran first.
   /// </summary>
   [TestClass]
   [DoNotParallelize]
   public class NorthWindIntegrationMySqlAsync {

      public string TestFile { get; set; } = "files/NorthWindMySqlToMySql.xml";

      private string CfgParams => $"MyServer={Tester.Server}&MyPort={Tester.Port}&MyUser={Tester.User}&MyPw={Tester.Pw}";

      private System.Data.IDbConnection InputCn() =>
         new MySqlConnectionFactory(new Connection { ConnectionString = Tester.GetConnectionString("northwind") }).GetConnection();

      private System.Data.IDbConnection OutputCn() =>
         new MySqlConnectionFactory(new Connection { ConnectionString = Tester.GetConnectionString("northwindstar") }).GetConnection();

      [TestMethod]
      [DoNotParallelize]
      public async Task Integration() {

         var logger = new ConsoleLogger(LogLevel.Info);

         using (var outer = new ConfigurationContainer().CreateScope(TestFile + $"?Mode=init&{CfgParams}", logger)) {
            var process = outer.Resolve<Process>();
            if (process.Errors().Any()) {
               foreach (var error in process.Errors()) {
                  Console.WriteLine("ERROR: " + error);
               }
            }
            using (var inner = new Container(new MySqlModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteStreamAsync();
            }
         }

         using (var cn = OutputCn()) {
            cn.Open();
            Assert.AreEqual(2155, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM NorthWindStar;"));
            Assert.AreEqual(2155, cn.ExecuteScalar<int>("SELECT Inserts FROM NorthWindControl WHERE Entity = 'OrderDetail' AND BatchId = 1 LIMIT 1;"));
         }

         // FIRST DELTA, NO CHANGES
         using (var outer = new ConfigurationContainer().CreateScope(TestFile + $"?{CfgParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new MySqlModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteStreamAsync();
            }
         }

         using (var cn = OutputCn()) {
            cn.Open();
            Assert.AreEqual(2155, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM NorthWindStar;"));
            Assert.AreEqual(0, cn.ExecuteScalar<int>("SELECT Inserts+Updates+Deletes FROM NorthWindControl WHERE Entity = 'OrderDetail' AND BatchId = 9 LIMIT 1;"));
         }

         // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO
         using (var cn = InputCn()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE OrderDetail SET unitPrice = 16, quantity = 41 WHERE orderId = 10253 AND productId = 39;"));
         }

         using (var outer = new ConfigurationContainer().CreateScope(TestFile + $"?{CfgParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new MySqlModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteStreamAsync();
            }
         }

         using (var cn = OutputCn()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'OrderDetail' AND BatchId = 17 LIMIT 1;"));
            Assert.AreEqual(16.0M, cn.ExecuteScalar<decimal>("SELECT OrderDetailsUnitPrice FROM NorthWindStar WHERE OrderDetailsOrderId = 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(41, cn.ExecuteScalar<int>("SELECT OrderDetailsQuantity FROM NorthWindStar WHERE OrderDetailsOrderId = 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(16.0 * 41, cn.ExecuteScalar<int>("SELECT OrderDetailsExtendedPrice FROM NorthWindStar WHERE OrderDetailsOrderId = 10253 AND OrderDetailsProductId = 39;"));
         }

         // CHANGE 1 RECORD'S CUSTID AND FREIGHT ON SALESORDER TABLE
         using (var cn = InputCn()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE SalesOrder SET custId = 76, freight = 21.11 WHERE orderId = 10254;"));
         }

         using (var outer = new ConfigurationContainer().CreateScope(TestFile + $"?{CfgParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new MySqlModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteStreamAsync();
            }
         }

         using (var cn = OutputCn()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Orders' AND BatchId = 26;"));
            Assert.AreEqual(76, cn.ExecuteScalar<int>("SELECT OrdersCustomerId FROM NorthWindStar WHERE OrderDetailsOrderId = 10254;"));
            Assert.AreEqual(21.11M, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindStar WHERE OrderDetailsOrderId = 10254;"));
         }
      }
   }
}
