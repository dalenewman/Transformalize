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
using System;
using System.IO;
using Autofac;
using Dapper;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Ioc.Autofac;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging;
using Transformalize.Providers.SqlServer;

namespace Tests {

    [TestClass]
    public class NorthWindIntegrationLucene : TestBase {

        public string TestFile { get; set; } = @"Files\NorthWindSqlServerToLucene.xml";
        public Connection InputConnection { get; set; } = new Connection {
            Name = "input",
            Provider = "sqlserver",
            Server = "localhost",
            Database = "NorthWind"
        };

        public Connection OutputConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "lucene",
            Folder = @"c:\temp\lucene_northwind"
        };
        
        [TestMethod]
        [Ignore]
        public void Lucene_Integration() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ShorthandModule("t"));
            builder.RegisterModule(new RootModule());
            var container = builder.Build();

            // CORRECT DATA AND INITIAL LOAD
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(2, cn.Execute(@"
                    UPDATE [Order Details] SET UnitPrice = 14.40, Quantity = 42 WHERE OrderId = 10253 AND ProductId = 39;
                    UPDATE Orders SET CustomerID = 'CHOPS', Freight = 22.98 WHERE OrderId = 10254;
                "));
            }

            var root = ResolveRoot(container, TestFile, InitMode());
            var response = Execute(root);

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual(string.Empty, response.Message);

            using (var reader = IndexReader.Open(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder,"Order Details"))), true)) {
                Assert.AreEqual(2155, reader.NumDocs());
            }

            // FIRST DELTA, NO CHANGES
            root = ResolveRoot(container, TestFile);
            response = Execute(root);

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual(string.Empty, response.Message);

            using (var reader = IndexReader.Open(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "Order Details"))), true)) {
                Assert.AreEqual(2155, reader.NumDocs());
            }

            // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO 
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                const string sql = @"UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;";
                Assert.AreEqual(1, cn.Execute(sql));
            }

            root = ResolveRoot(container, TestFile);
            response = Execute(root);

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual(string.Empty, response.Message);

            using (var searcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder,"Order Details"))), true)) {
                var hits = searcher.Search(new TermQuery(new Term("TflId", "1025339")),null, 1);
                Assert.AreEqual(1, hits.TotalHits);
                var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
                Assert.AreEqual(15.0M, Convert.ToDecimal(hit.Get("OrderDetailsUnitPrice")));
                Assert.AreEqual(40, Convert.ToInt32(hit.Get("OrderDetailsQuantity")));
                Assert.AreEqual(40 * 15.0M, Convert.ToDecimal(hit.Get("OrderDetailsExtendedPrice")));
            }

            // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
            }

            root = ResolveRoot(container, TestFile);
            response = Execute(root);

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual(string.Empty, response.Message);

            using (var searcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "Orders"))), true)) {
                var hits = searcher.Search(new TermQuery(new Term("OrdersOrderID", NumericUtils.IntToPrefixCoded(10254))), 1);
                Assert.AreEqual(1, hits.TotalHits);
                var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
                Assert.AreEqual("VICTE", hit.Get("OrdersCustomerID"));
                Assert.AreEqual(20.11M, Convert.ToDecimal(hit.Get("OrdersFreight")));
            }


        }
    }
}
