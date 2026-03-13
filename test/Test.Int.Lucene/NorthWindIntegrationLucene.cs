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
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.Lucene;
using Transformalize.Providers.Lucene.Autofac;
using Transformalize.Providers.Sqlite.Autofac;
using Transformalize.Providers.SQLite;
using Transformalize.Transforms.Jint.Autofac;

namespace IntegrationTests {

   [TestClass]
   [DoNotParallelize]
   public class NorthWindIntegrationLucene {

      public static string LuceneFolder { get; } = Path.Combine(Path.GetTempPath(), "northwind-lucene");
      public static string SqliteFile { get; } = "files/northwind.sqlite3";

      public string TestFile { get; } = "files/NorthWindSqliteToLucene.xml";
      public string FileParams => $"SqliteFile={SqliteFile}&LuceneFolder={LuceneFolder}";

      public Connection InputConnection => new Connection {
         Name = "input",
         Provider = "sqlite",
         File = SqliteFile
      };

      // The master entity (first/fact entity) drives the doc count
      public string MasterEntityAlias => "Order Details";

      [ClassInitialize]
      public static void ClassInit(TestContext context) {
         File.Copy("files/northwind-sqlite.db", SqliteFile, overwrite: true);
         if (System.IO.Directory.Exists(LuceneFolder)) {
            System.IO.Directory.Delete(LuceneFolder, recursive: true);
         }
      }

      private static DirectoryInfo EntityIndex(string entityAlias) =>
         new DirectoryInfo(Path.Combine(LuceneFolder, entityAlias));

      [TestMethod]
      public void Lucene_01_Init() {

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{TestFile}?Mode=init&{FileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new LuceneModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         using var reader = DirectoryReader.Open(FSDirectory.Open(EntityIndex(MasterEntityAlias)));
         Assert.AreEqual(2155, reader.NumDocs);
      }

      [TestMethod]
      public void Lucene_02_NoChanges() {

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{TestFile}?{FileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new LuceneModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         using var reader = DirectoryReader.Open(FSDirectory.Open(EntityIndex(MasterEntityAlias)));
         Assert.AreEqual(2155, reader.NumDocs);
      }

      [TestMethod]
      public void Lucene_03_ChangeOrderDetails() {

         // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE
         using (var cn = new SqliteConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;"));
         }

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{TestFile}?{FileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new LuceneModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         var indexDir = EntityIndex(MasterEntityAlias);
         var searcher = new IndexSearcher(DirectoryReader.Open(FSDirectory.Open(indexDir)));
         var query = new BooleanQuery {
            { NumericRangeQuery.NewInt32Range("OrderDetailsOrderID", 10253, 10253, true, true), Occur.MUST },
            { NumericRangeQuery.NewInt32Range("OrderDetailsProductID", 39, 39, true, true), Occur.MUST }
         };
         var hits = searcher.Search(query, 1);
         Assert.AreEqual(1, hits.TotalHits);
         var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
         Assert.AreEqual(15.0M, Convert.ToDecimal(hit.Get("OrderDetailsUnitPrice")));
         Assert.AreEqual(40, Convert.ToInt32(hit.Get("OrderDetailsQuantity")));
         Assert.AreEqual(40 * 15.0M, Convert.ToDecimal(hit.Get("OrderDetailsExtendedPrice")));
      }

      [TestMethod]
      public void Lucene_04_ChangeOrders() {

         // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
         using (var cn = new SqliteConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
         }

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{TestFile}?{FileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new LuceneModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         // Changes to Orders are tracked in the Orders entity index (no cross-entity denormalization for Lucene)
         var indexDir = EntityIndex("Orders");
         var searcher = new IndexSearcher(DirectoryReader.Open(FSDirectory.Open(indexDir)));
         var hits = searcher.Search(NumericRangeQuery.NewInt32Range("OrdersOrderID", 10254, 10254, true, true), 1);
         Assert.AreEqual(1, hits.TotalHits);
         var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
         Assert.AreEqual("VICTE", hit.Get("OrdersCustomerID"));
         Assert.AreEqual(20.11M, Convert.ToDecimal(hit.Get("OrdersFreight")));
      }

   }
}
