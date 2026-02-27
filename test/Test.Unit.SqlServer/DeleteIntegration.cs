#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.SqlServer;
using Transformalize.Providers.SqlServer.Autofac;

namespace Test.Unit.SqlServer {

   [TestClass]
   public class DeleteIntegration {

      public Connection InputConnection { get; set; } = new Connection {
         Name = "input",
         Provider = "sqlserver",
         ConnectionString = Tester.GetConnectionString("NorthWind")
      };

      public Connection OutputConnection { get; set; } = new Connection {
         Name = "output",
         Provider = "sqlserver",
         ConnectionString = Tester.GetConnectionString("TflNorthWind")
      };

      [TestMethod]
      public void Delete_Integration() {

         var logger = new ConsoleLogger(LogLevel.Info);

         var cfg = $@"files/DeleteIntegration.xml?Server={Tester.Server},{Tester.Port}&User={Tester.User}&Pw={Tester.Pw}";

         // INITIALIZE INPUT
         using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.Execute(@"
                        IF OBJECT_ID('TestDeletes') IS NOT NULL
    	                    DROP TABLE [TestDeletes];

                        CREATE TABLE [TestDeletes](
    	                    [TextValue] NVARCHAR(64),
    	                    [Id] INT,
    	                    [NumericValue] INT,
                            [RowVersion] ROWVERSION,
    	                    CONSTRAINT PK_TestDeletes_Id_NumericValue PRIMARY KEY ([Id], [NumericValue])
                        )

                        INSERT INTO [TestDeletes]([TextValue],[Id],[NumericValue]) VALUES('One',1,1);
                        INSERT INTO [TestDeletes]([TextValue],[Id],[NumericValue]) VALUES('Two',2,2);
                        INSERT INTO [TestDeletes]([TextValue],[Id],[NumericValue]) VALUES('Three',3,3);
    "));
         }

         // RUN INIT AND TEST
         using (var outer = new ConfigurationContainer().CreateScope(cfg + "&Mode=init", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)3, process.Entities.First().Inserts);
               Assert.AreEqual((uint)0, process.Entities.First().Updates);
               Assert.AreEqual((uint)0, process.Entities.First().Deletes);
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
         }

         // FIRST DELTA, NO CHANGES
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)0, process.Entities.First().Inserts);
               Assert.AreEqual((uint)0, process.Entities.First().Updates);
               Assert.AreEqual((uint)0, process.Entities.First().Deletes);
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
         }

         // DELETE Row 2, Two
         using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            const string sql = @"DELETE FROM [TestDeletes] WHERE [Id] = 2 AND [NumericValue] = 2;";
            Assert.AreEqual(1, cn.Execute(sql));
         }

         // RUN AND CHECK, SHOULD STILL HAVE 3 RECORDS, but one marked TflDeleted = 1
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)0, process.Entities.First().Inserts);
               Assert.AreEqual((uint)0, process.Entities.First().Updates);
               Assert.AreEqual((uint)1, process.Entities.First().Deletes);
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
            Assert.AreEqual(1, cn.ExecuteScalar<decimal>("SELECT COUNT(*) FROM TestDeletesStar WHERE TflDeleted = 1;"));
         }

         // RUN AGAIN
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)0, process.Entities.First().Inserts);
               Assert.AreEqual((uint)0, process.Entities.First().Updates);
               Assert.AreEqual((uint)0, process.Entities.First().Deletes);
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
            Assert.AreEqual(1, cn.ExecuteScalar<decimal>("SELECT COUNT(*) FROM TestDeletesStar WHERE TflDeleted = 1;"));
         }

         // UN-DELETE Row 2, Two
         using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            const string sql = @"INSERT INTO [TestDeletes]([TextValue],[Id],[NumericValue]) VALUES('Two',2,2);";
            Assert.AreEqual(1, cn.Execute(sql));
         }

         // RUN AND CHECK
         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)0, process.Entities.First().Inserts);
               Assert.AreEqual((uint)1, process.Entities.First().Updates);
               Assert.AreEqual((uint)0, process.Entities.First().Deletes);
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar WHERE TflDeleted = 0;"));
         }


      }


   }
}

