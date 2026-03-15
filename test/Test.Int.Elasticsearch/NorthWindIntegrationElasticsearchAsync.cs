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
using Elastic.Transport;
using ElasticHttpMethod = Elastic.Transport.HttpMethod;
using TflField = Transformalize.Configuration.Field;
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
using Transformalize.Providers.CsvHelper.Autofac;
using Transformalize.Providers.Elasticsearch;
using Transformalize.Providers.Elasticsearch.Autofac;
using Transformalize.Providers.Elasticsearch.Ext;
using System.IO;
using System.Text.Json;
using Transformalize.Providers.Sqlite.Autofac;
using Transformalize.Providers.SQLite;
using Transformalize.Transforms.Jint.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   [DoNotParallelize]
   public class IntegrationAsync {

      public string SqliteTestFile { get; set; } = @"files/NorthWindIntegrationSqlite.xml";
      public string ElasticTestFile { get; set; } = @"files/NorthWindSqliteToElasticsearch.xml";
      public static string SqliteInputFile {get; set;} = @"files/northwind-input-async.sqlite3";
      public static string SqliteOutputFile {get; set;} = @"files/northwind-output-async.sqlite3";

      public Connection InputConnection => new Connection {
         Name = "input",
         Provider = "sqlite",
         File = SqliteInputFile
      };

      public Connection OutputConnection => new Connection {
         Name = "output",
         Provider = "sqlite",
         File = SqliteOutputFile
      };

      [ClassInitialize]
      public static void ClassInit(TestContext context) {
         File.Copy("files/northwind-sqlite.db", SqliteInputFile, overwrite: true);
         if (File.Exists(SqliteOutputFile)) {
            File.Delete(SqliteOutputFile);
         }
      }

      private string ElasticFileParams =>
         $"ElasticServer={Tester.ElasticServer}&ElasticPort={Tester.ElasticPort}" +
         $"&ElasticUser={Tester.ElasticUser}&ElasticPassword={Tester.ElasticPassword}" +
         $"&ElasticVersion={Tester.ElasticVersion}&SqliteFile={SqliteOutputFile}";

      private static string ColorsCsvToElasticXml => $@"<add name='ColorsCsvToElastic' mode='init' read-only='true'>
  <connections>
    <add name='input' provider='file' delimiter=',' file='files/colors.csv' synchronous='true' start='0' />
    <add name='output' provider='elasticsearch' server='{Tester.ElasticServer}' port='{Tester.ElasticPort}' index='colors' user='{Tester.ElasticUser}' password='{Tester.ElasticPassword}' useSsl='true' version='{Tester.ElasticVersion}' />
  </connections>
  <entities>
    <add name='rows'>
      <fields>
        <add name='code' primary-key='true' />
        <add name='name' />
        <add name='hex' />
        <add name='r' type='int' />
        <add name='g' type='int' />
        <add name='b' type='int' />
      </fields>
      <calculated-fields>
        <add name='rgb' t='copy(r,g,b).join(,)' />
        <add name='total' type='int' t='rownumber()' />
      </calculated-fields>
    </add>
  </entities>
</add>";

      private async Task BuildColorsIndex(uint? expectedInserts = null) {
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(ColorsCsvToElasticXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new CsvHelperProviderModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               await inner.Resolve<IProcessController>().ExecuteAsync();
               if (expectedInserts.HasValue) {
                  Assert.AreEqual(expectedInserts.Value, process.Entities.First().Inserts);
               }
            }
         }
      }

      [TestMethod]
      [DoNotParallelize]
      public async Task SqliteToElastic() {

         var logger = new ConsoleLogger(LogLevel.Info);

         var pool = new SingleNodePool(new Uri(Tester.ElasticUrl));
         var settings = new TransportConfigurationDescriptor(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .Authentication(new BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword));
         ITransport client = new DistributedTransport(settings);

         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{SqliteTestFile}?Mode=init&SqliteInputFile={SqliteInputFile}&SqliteOutputFile={SqliteOutputFile}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?Mode=init&{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         var refreshPath = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_refresh");
         client.Request<DynamicResponse>(in refreshPath);
         var countPath = new EndpointPath(ElasticHttpMethod.GET, "/northwind/_count");
         Assert.AreEqual(2155, (int)client.Request<DynamicResponse>(in countPath, PostData.String("{\"query\" : { \"match_all\" : { }}}")).Body["count"]);

         // FIRST DELTA, NO CHANGES
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         var refreshPath2 = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_refresh");
         client.Request<DynamicResponse>(in refreshPath2);
         var countPath2 = new EndpointPath(ElasticHttpMethod.GET, "/northwind/_count");
         Assert.AreEqual(2155, (int)client.Request<DynamicResponse>(in countPath2, PostData.String("{\"query\" : { \"match_all\" : { }}}")).Body["count"]);

         // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO
         using (var cn = new SqliteConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            const string sql = @"UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;";
            Assert.AreEqual(1, cn.Execute(sql));
         }

         // RUN AND CHECK SQL
         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{SqliteTestFile}?SqliteInputFile={SqliteInputFile}&SqliteOutputFile={SqliteOutputFile}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         using (var cn = new SqliteConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Order Details' AND BatchId = 9 LIMIT 1;"));
            Assert.AreEqual(15.0M, cn.ExecuteScalar<decimal>("SELECT OrderDetailsUnitPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(40, cn.ExecuteScalar<int>("SELECT OrderDetailsQuantity FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
            Assert.AreEqual(15.0 * 40, cn.ExecuteScalar<int>("SELECT OrderDetailsExtendedPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
         }

         // RUN AND CHECK ELASTIC
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         var refreshPath3 = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_refresh");
         client.Request<DynamicResponse>(in refreshPath3);
         var searchPath1 = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_search");
         var response = client.Request<DynamicResponse>(in searchPath1, PostData.String(@"{
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

         var source = GetFirstSource(response);

         Assert.AreEqual(source["orderdetailsunitprice"], 15.0);
         Assert.AreEqual(source["orderdetailsquantity"], (long)40);
         Assert.AreEqual(source["orderdetailsextendedprice"], 40 * 15.0);

         // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
         using (var cn = new SqliteConnectionFactory(InputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
         }

         // RUN AND CHECK SQL
         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope($"{SqliteTestFile}?SqliteInputFile={SqliteInputFile}&SqliteOutputFile={SqliteOutputFile}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new JintTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         using (var cn = new SqliteConnectionFactory(OutputConnection).GetConnection()) {
            cn.Open();
            Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Orders' AND BatchId = 18;"));
            Assert.AreEqual("VICTE", cn.ExecuteScalar<string>("SELECT OrdersCustomerId FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
            Assert.AreEqual(20.11M, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
         }

         // RUN AND CHECK ELASTIC
         using (var outer = new ConfigurationContainer().CreateScope($"{ElasticTestFile}?{ElasticFileParams}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqliteModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }

         var refreshPath4 = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_refresh");
         client.Request<DynamicResponse>(in refreshPath4);
         var searchPath2 = new EndpointPath(ElasticHttpMethod.POST, "/northwind/_search");
         response = client.Request<DynamicResponse>(in searchPath2, PostData.String(@"{
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

         source = GetFirstSource(response);

         Assert.AreEqual(source["orderscustomerid"], "VICTE");
         Assert.AreEqual(source["ordersfreight"], 20.11);

      }

      [TestMethod]
      public async Task Colors00BuildIndexFromCsv() {
         await BuildColorsIndex(865);
      }

      [TestMethod]
      public async Task TestSingleIndexMapping() {
         await BuildColorsIndex();

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = Tester.ElasticServer,
            Port = Tester.ElasticPort,
            User = Tester.ElasticUser,
            Password = Tester.ElasticPassword,
            UseSsl = true,
            Version = Tester.ElasticVersion
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodePool(new Uri(connection.Url));
         var settings = new TransportConfigurationDescriptor(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .Authentication(new BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword));
         ITransport client = new DistributedTransport(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger()), connection);
         var schemaReader = new ElasticSchemaReader(context, client);

         Assert.AreEqual(9, (await schemaReader.GetFieldsAsync("rows")).Count());

      }

      [TestMethod]
      public async Task TestAllIndexMapping() {
         await BuildColorsIndex();

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = Tester.ElasticServer,
            Port = Tester.ElasticPort,
            User = Tester.ElasticUser,
            Password = Tester.ElasticPassword,
            UseSsl = true,
            Version = Tester.ElasticVersion
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodePool(new Uri(connection.Url));
         var settings = new TransportConfigurationDescriptor(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .Authentication(new BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword));
         ITransport client = new DistributedTransport(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger()), connection);
         var schemaReader = new ElasticSchemaReader(context, client);

         Assert.AreEqual(1, (await schemaReader.GetEntitiesAsync()).Count());

      }

      [TestMethod]
      public async Task TestReadAll() {
         await BuildColorsIndex();

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = Tester.ElasticServer,
            Port = Tester.ElasticPort,
            User = Tester.ElasticUser,
            Password = Tester.ElasticPassword,
            UseSsl = true,
            Version = Tester.ElasticVersion
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodePool(new Uri(connection.Url));
         var settings = new TransportConfigurationDescriptor(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .Authentication(new BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword));
         ITransport client = new DistributedTransport(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger(), null, new Entity { Name = "rows", Alias = "rows" }), connection);
         var code = new TflField { Name = "code", Index = 0 };
         var total = new TflField { Name = "total", Type = "int", Index = 1 };

         var reader = new ElasticReader(context, new[] { code, total }, client, new RowFactory(2, false, false), ReadFrom.Input);

         var rows = (await reader.ReadAsync()).ToArray();
         Assert.AreEqual(865, rows.Length);

      }

      [TestMethod]
      public async Task TestReadPage() {
         await BuildColorsIndex();

         var connection = new Connection {
            Name = "input",
            Provider = "elasticsearch",
            Index = "colors",
            Server = Tester.ElasticServer,
            Port = Tester.ElasticPort,
            User = Tester.ElasticUser,
            Password = Tester.ElasticPassword,
            UseSsl = true,
            Version = Tester.ElasticVersion
         };

         connection.Url = connection.GetElasticUrl();

         var pool = new SingleNodePool(new Uri(connection.Url));
         var settings = new TransportConfigurationDescriptor(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .Authentication(new BasicAuthentication(Tester.ElasticUser, Tester.ElasticPassword));
         ITransport client = new DistributedTransport(settings);
         var context = new ConnectionContext(new PipelineContext(new DebugLogger(), null, new Entity {
            Name = "rows",
            Alias = "rows",
            Page = 2,
            Size = 20
         }), connection);
         var code = new TflField { Name = "code", Index = 0 };
         var total = new TflField { Name = "total", Type = "int", Index = 1 };

         var reader = new ElasticReader(context, new[] { code, total }, client, new RowFactory(2, false, false), ReadFrom.Input);

         var rows = (await reader.ReadAsync()).ToArray();
         Assert.AreEqual(20, rows.Length);

      }

      private static IDictionary<string, object> GetFirstSource(DynamicResponse response) {
         var hitsVal = response.Body["hits"]["hits"].Value;
         IList<object>? hits;
         if (hitsVal is JsonElement je && je.ValueKind == JsonValueKind.Array)
            hits = DynamicValue.ConsumeJsonElement(typeof(object), je) as IList<object>;
         else
            hits = hitsVal as IList<object>;
         var hit = (IDictionary<string, object>)hits![0]!;
         return (IDictionary<string, object>)hit["_source"]!;
      }

   }
}
