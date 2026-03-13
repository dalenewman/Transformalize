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
using Lucene.Net.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.Lucene;
using Transformalize.Providers.Lucene.Autofac;
using Transformalize.Providers.SqlServer;

namespace IntegrationTests {

   [TestClass]
   public class NorthWindIntegrationLucene {

      //public string TestFile { get; set; } = @"Files\NorthWindSqlServerToLucene.xml";
      //public Connection InputConnection { get; set; } = new Connection {
      //   Name = "input",
      //   Provider = "sqlserver",
      //   Server = "localhost",
      //   Database = "NorthWind"
      //};

      //public Connection OutputConnection { get; set; } = new Connection {
      //   Name = "output",
      //   Provider = "lucene",
      //   Folder = @"c:\temp\lucene_northwind"
      //};

      //[TestMethod]
      //[Ignore] // not tested yet
      //public void Lucene_Integration() {

      //   var logger = new ConsoleLogger(LogLevel.Debug);

      //   // CORRECT DATA AND INITIAL LOAD
      //   using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
      //      cn.Open();
      //      Assert.AreEqual(2, cn.Execute(@"
      //              UPDATE [Order Details] SET UnitPrice = 14.40, Quantity = 42 WHERE OrderId = 10253 AND ProductId = 39;
      //              UPDATE Orders SET CustomerID = 'CHOPS', Freight = 22.98 WHERE OrderId = 10254;
      //          "));
      //   }


      //   using (var outer = new ConfigurationContainer().CreateScope(TestFile + "?Mode=init", logger)) {
      //      var process = outer.Resolve<Process>();
      //      using (var inner = new Container(new LuceneModule()).CreateScope(process, logger)) {
      //         var controller = inner.Resolve<IProcessController>();
      //         controller.Execute();
      //      }
      //   }

      //   using (var reader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "NorthWindStar"))))) {
      //      Assert.AreEqual(2155, reader.NumDocs);
      //   }

      //   // FIRST DELTA, NO CHANGES
      //   using (var outer = new ConfigurationContainer().CreateScope(TestFile, logger)) {
      //      var process = outer.Resolve<Process>();
      //      using (var inner = new Container().CreateScope(process, logger)) {
      //         var controller = inner.Resolve<IProcessController>();
      //         controller.Execute();
      //      }
      //   }

      //   using (var reader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "NorthWindStar"))))) {
      //      Assert.AreEqual(2155, reader.NumDocs);
      //   }

      //   // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO 
      //   using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
      //      cn.Open();
      //      const string sql = @"UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;";
      //      Assert.AreEqual(1, cn.Execute(sql));
      //   }

      //   using (var outer = new ConfigurationContainer().CreateScope(TestFile, logger)) {
      //      var process = outer.Resolve<Process>();
      //      using (var inner = new Container().CreateScope(process, logger)) {
      //         var controller = inner.Resolve<IProcessController>();
      //         controller.Execute();
      //      }
      //   }

      //   var searcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "NorthWindStar"))));
      //   var booleanQuery = new BooleanQuery {
      //      {new TermQuery(new Term("OrderDetailsOrderID", NumericUtils.Int32ToPrefixCoded(10253))), Occur.MUST},
      //      {new TermQuery(new Term("OrderDetailsProductID", NumericUtils.Int32ToPrefixCoded(39))), Occur.MUST}
      //   };

      //   var hits = searcher.Search(booleanQuery, null, 1);
      //   Assert.AreEqual(1, hits.TotalHits);
      //   var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
      //   Assert.AreEqual(15.0M, Convert.ToDecimal(hit.Get("OrderDetailsUnitPrice")));
      //   Assert.AreEqual(40, Convert.ToInt32(hit.Get("OrderDetailsQuantity")));
      //   Assert.AreEqual(40 * 15.0M, Convert.ToDecimal(hit.Get("OrderDetailsExtendedPrice")));


      //   // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
      //   using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
      //      cn.Open();
      //      Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
      //   }

      //   using (var outer = new ConfigurationContainer().CreateScope(TestFile, logger)) {
      //      var process = outer.Resolve<Process>();
      //      using (var inner = new Container().CreateScope(process, logger)) {
      //         var controller = inner.Resolve<IProcessController>();
      //         controller.Execute();
      //      }
      //   }

      //   using (var searcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(Path.Combine(OutputConnection.Folder, "NorthWindStar"))), true)) {
      //      var hits = searcher.Search(new TermQuery(new Term("OrderDetailsOrderID", NumericUtils.Int32ToPrefixCoded(10254))), 1);
      //      Assert.AreNotEqual(0, hits.TotalHits);
      //      var hit = searcher.Doc(hits.ScoreDocs[0].Doc);
      //      Assert.AreEqual("VICTE", hit.Get("OrdersCustomerID"));
      //      Assert.AreEqual(20.11M, Convert.ToDecimal(hit.Get("OrdersFreight")));
      //   }


      //}
   }
}
