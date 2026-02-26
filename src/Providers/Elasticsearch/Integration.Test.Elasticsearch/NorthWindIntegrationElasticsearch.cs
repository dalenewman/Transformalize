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
using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Logging;
using Transformalize.Providers.Console;
using Transformalize.Providers.Elasticsearch;
using Transformalize.Providers.Elasticsearch.Autofac;
using Transformalize.Providers.Elasticsearch.Ext;
using Transformalize.Providers.SqlServer;
using Transformalize.Providers.SqlServer.Autofac;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Transforms.CSharp.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   [DoNotParallelize]
   public class NorthWindIntegrationSqlServerThenElastic {

      public string SqlTestFile { get; set; } = @"files/NorthWind.xml";
      public string ElasticTestFile { get; set; } = @"files/NorthWindSqlServerToElasticsearch.xml";

      public Connection InputConnection => new Connection {
         Name = "input",
         Provider = "sqlserver",
         ConnectionString = Tester.GetSqlConnectionString("NorthWind")
      };

      public Connection OutputConnection => new Connection {
         Name = "output",
         Provider = "sqlserver",
         ConnectionString = Tester.GetSqlConnectionString("TflNorthWind")
      };

      private string SqlFileParams =>
         $"Server={Tester.SqlServer}&User={Tester.SqlUser}&Pw={Tester.SqlPw}";

      private string ElasticFileParams =>
         $"SqlServer={Tester.SqlServer}&SqlUser={Tester.SqlUser}&SqlPw={Tester.SqlPw}" +
         $"&ElasticServer={Tester.ElasticServer}&ElasticPort={Tester.ElasticPort}" +
         $"&ElasticUser={Tester.ElasticUser}&ElasticPassword={Tester.ElasticPassword}" +
         $"&ElasticVersion={Tester.ElasticVersion}";

      [TestMethod]
      public void SqlServer_Elasticsearch_Integration() {

         var logger = new ConsoleLogger(LogLevel.Info);

         var pool = new SingleNodeConnectionPool(new Uri(Tester.ElasticUrl));
         var settings = new ConnectionConfiguration(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword);
         var client = new ElasticLowLevelClient(settings);

         using (var outer = new ConfigurationContainer(new CSharpModule()).CreateScope($"{SqlTestFile}?Mode=init&{SqlFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AdoProviderModule(), new SqlServerModule(), new CSharpModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?Mode=init&{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         client.Indices.Refresh<DynamicResponse>("northwind");
         Assert.AreEqual(2155, client.Count<DynamicResponse>("northwind", PostData.String("{\"query\" : { \"match_all\" : { }}}")).Body["count"].Value);

         // FIRST DELTA, NO CHANGES
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         client.Indices.Refresh<DynamicResponse>("northwind");
         Assert.AreEqual(2155, client.Count<DynamicResponse>("northwind", PostData.String("{\"query\" : { \"match_all\" : { }}}")).Body["count"].Value);

         // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO
         using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            const string sql = @"UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;";
            Assert.AreEqual(1, cn.Execute(sql));
         }

         // RUN AND CHECK SQL
         using (var outer = new ConfigurationContainer(new CSharpModule()).CreateScope($"{SqlTestFile}?{SqlFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AdoProviderModule(), new SqlServerModule(), new CSharpModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT TOP 1 Updates FROM NorthWindControl WHERE Entity = 'Order Details' AND BatchId = 9;"));
            Assert.AreEqual(15.0M, cn.ExecuteScalar<decimal>("SELECT OrderDetailsUnitPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(40, cn.ExecuteScalar<int>("SELECT OrderDetailsQuantity FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(15.0 * 40, cn.ExecuteScalar<int>("SELECT OrderDetailsExtendedPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
         }

         // RUN AND CHECK ELASTIC
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         client.Indices.Refresh<DynamicResponse>("northwind");
         var response = client.Search<DynamicResponse>(
             "northwind", PostData.String(@"{
   ""query"" : {
      ""constant_score"" : {
         ""filter"" : {
            ""bool"" : {
              ""must"" : [
                 { ""term"" : {""orderdetailsorderid"" : 10253}},
                 { ""term"" : {""orderdetailsproductid"" : 39}}
              ]
           }
         }
      }
   }
}"));

         var hits = response.Body["hits"]["hits"].Value as IList<object>;
         var hit = hits[0] as IDictionary<string, object>;
         var source = hit["_source"] as IDictionary<string, object>;

         Assert.AreEqual(source["orderdetailsunitprice"], 15.0);
         Assert.AreEqual(source["orderdetailsquantity"], (long)40);
         Assert.AreEqual(source["orderdetailsextendedprice"], 40 * 15.0);

         // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
         using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
         }

         // RUN AND CHECK SQL
         using (var outer = new ConfigurationContainer(new CSharpModule()).CreateScope($"{SqlTestFile}?{SqlFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new AdoProviderModule(), new SqlServerModule(), new CSharpModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Orders' AND BatchId = 18;"));
            Assert.AreEqual("VICTE", cn.ExecuteScalar<string>("SELECT OrdersCustomerId FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
            Assert.AreEqual(20.11M, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
         }

         // RUN AND CHECK ELASTIC
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         client.Indices.Refresh<DynamicResponse>("northwind");
         response = client.Search<DynamicResponse>(
             "northwind",
             PostData.String(@"{
   ""query"" : {
      ""constant_score"" : {
         ""filter"" : {
            ""bool"" : {
              ""must"" : [
                 { ""term"" : {""orderdetailsorderid"" : 10254}}
              ]
           }
         }
      }
   }
}"));

         hits = response.Body["hits"]["hits"].Value as IList<object>;
         hit = hits[0] as IDictionary<string, object>;
         source = hit["_source"] as IDictionary<string, object>;

         Assert.AreEqual(source["orderscustomerid"], "VICTE");
         Assert.AreEqual(source["ordersfreight"], 20.11);

      }

      [TestMethod]
      [Ignore]
      public void TestSingleIndexMapping() {

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = "localhost",
            Port = 9200
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodeConnectionPool(new Uri(connection.Url));
         var settings = new ConnectionConfiguration(pool);
         var client = new ElasticLowLevelClient(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger()), connection);
         var schemaReader = new ElasticSchemaReader(context, client);

         Assert.AreEqual(11, schemaReader.GetFields("rows").Count());

      }

      [TestMethod]
      [Ignore]
      public void TestAllIndexMapping() {

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = "localhost",
            Port = 9200
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodeConnectionPool(new Uri(connection.Url));
         var settings = new ConnectionConfiguration(pool);
         var client = new ElasticLowLevelClient(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger()), connection);
         var schemaReader = new ElasticSchemaReader(context, client);

         Assert.AreEqual(1, schemaReader.GetEntities().Count());

      }

      [TestMethod]
      [Ignore]
      public void TestReadAll() {

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = "localhost",
            Port = 9200
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodeConnectionPool(new Uri(connection.Url));
         var settings = new ConnectionConfiguration(pool);
         var client = new ElasticLowLevelClient(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger(), null, new Entity { Name = "rows", Alias = "rows" }), connection);
         var code = new Field { Name = "code", Index = 0 };
         var total = new Field { Name = "total", Type = "int", Index = 1 };

         var reader = new ElasticReader(context, new[] { code, total }, client, new RowFactory(2, false, false), ReadFrom.Input);

         var rows = reader.Read().ToArray();
         Assert.AreEqual(865, rows.Length);

      }

      [TestMethod]
      [Ignore]
      public void TestReadPage() {

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = "localhost",
            Port = 9200
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodeConnectionPool(new Uri(connection.Url));
         var settings = new ConnectionConfiguration(pool);
         var client = new ElasticLowLevelClient(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger(), null, new Entity {
            Name = "rows",
            Alias = "rows",
            Page = 2,
            Size = 20
         }), connection);
         var code = new Field { Name = "code", Index = 0 };
         var total = new Field { Name = "total", Type = "int", Index = 1 };

         var reader = new ElasticReader(context, new[] { code, total }, client, new RowFactory(2, false, false), ReadFrom.Input);

         var rows = reader.Read().ToArray();
         Assert.AreEqual(20, rows.Length);

      }

   }
}
