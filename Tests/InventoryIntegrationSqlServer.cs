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
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging;
using Transformalize.Provider.SqlServer;

namespace Tests {

    [TestClass]
    public class InventoryIntegrationSqlServer {

        public string TestFile { get; set; } = @"c:\temp\Inventory.xml";

        public Connection InputConnection { get; set; } = new Connection {
            Name = "input",
            Provider = "sqlserver",
            ConnectionString = "server=localhost;database=Clevest10DUKE231003;trusted_connection=true;"
        };

        public Connection OutputConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "sqlserver",
            ConnectionString = "Server=localhost;Database=Tfl10DUKE231003;trusted_connection=true;"
        };

        public Process ResolveRoot(IContainer container, string file, bool init) {
            return container.Resolve<Process>(new NamedParameter("cfg", file + (init ? "?Mode=init" : string.Empty)));
        }


        [TestMethod]
        [Ignore]
        public void SqlServer_Integration() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(@"Shorthand.xml"));
            var container = builder.Build();

            // PUT INVENTORY BACK IN ORGINAL LOCATION
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.Execute(@"
                    UPDATE Inventory SET StorageLocationKey = '152C36A9-1C87-4841-BD73-801E4BB7097A' WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3';
                "));
            }

            // RUN INIT AND TEST
            var root = ResolveRoot(container, TestFile, true);
            var responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(4, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM InventoryStar;"));
                Assert.AreEqual("Scope-Indiana",cn.ExecuteScalar<string>("SELECT Warehouse FROM InventoryStar WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3'"));
                Assert.AreEqual(4, cn.ExecuteScalar<int>("SELECT TOP 1 Inserts FROM InventoryControl WHERE Entity = 'Inventory' AND BatchId = 1;"));
            }

            // FIRST DELTA, NO CHANGES
            root = ResolveRoot(container, TestFile, false);
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(4, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM InventoryStar;"));
                Assert.AreEqual(0, cn.ExecuteScalar<int>("SELECT TOP 1 Inserts+Updates+Deletes FROM InventoryControl WHERE Entity = 'Inventory' AND BatchId = 8;"));
                Assert.AreEqual("Scope-Indiana", cn.ExecuteScalar<string>("SELECT Warehouse FROM InventoryStar WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3'"));
            }


            // MOVE INVENTORY 
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                const string sql = @"UPDATE Inventory SET StorageLocationKey = '11111111-2222-3333-4444-555555555555' WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3';";
                Assert.AreEqual(1, cn.Execute(sql));
            }

            // RUN AND CHECK
            root = ResolveRoot(container, TestFile, false);
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual("Fitted", cn.ExecuteScalar<string>("SELECT Warehouse FROM InventoryStar WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3'"));
            }

            // MOVE INVENTORY AGAIN
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                const string sql = @"UPDATE Inventory SET StorageLocationKey = '6BAE6E58-356E-4639-8DC6-036B0A5DD529' WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3';";
                Assert.AreEqual(1, cn.Execute(sql));
            }

            // RUN AND CHECK
            root = ResolveRoot(container, TestFile, false);
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual("Scope-Indiana", cn.ExecuteScalar<string>("SELECT Warehouse FROM InventoryStar WHERE InventoryKey = '7C9D2F61-5BEE-4A79-85B9-3DFE01B55EC3'"));
            }


        }
    }
}
