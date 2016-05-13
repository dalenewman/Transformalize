#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Provider.SqlServer;
using Dapper;
using Nest;
using System;
using System.Linq;
using Autofac;
using Cfg.Net.Ext;
using Elasticsearch.Net;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Test {

    [TestFixture]
    public class NorthWindIntegrationSqlServerThenElastic {

        public string ElasticTestFile { get; set; } = @"Files\NorthWindSqlServerToElastic.xml";
        public string SqlTestFile { get; set; } = @"Files\NorthWind.xml";

        public Connection InputConnection { get; set; } = new Connection {
            Name = "input",
            Provider = "sqlserver",
            ConnectionString = "server=localhost;database=NorthWind;trusted_connection=true;"
        }.WithValidation();

        public Connection OutputConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "sqlserver",
            ConnectionString = "Server=localhost;Database=NorthWindStar;trusted_connection=true;"
        };

        public Connection ElasticConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "elastic",
            Index = "northwind",
            Url = "http://localhost:9200"
        }.WithValidation();

        public Process ResolveRoot(IContainer container, string file, bool init) {
            return container.Resolve<Process>(new NamedParameter("cfg", file + (init ? "?Mode=init" : string.Empty)));
        }


        [Test]
        [Ignore("Needs local sql server and elasticsearch.")]
        public void Integration() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(@"Files\Shorthand.xml"));
            var container = builder.Build();

            IConnectionSettingsValues settings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(ElasticConnection.Url)));
            var es = new ElasticClient(settings);

            // CORRECT DATA AND INITIAL LOAD
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(3, cn.Execute(@"
                    UPDATE [Order Details] SET UnitPrice = 14.40, Quantity = 42 WHERE OrderId = 10253 AND ProductId = 39;
                    UPDATE Orders SET CustomerID = 'CHOPS', Freight = 22.98 WHERE OrderId = 10254;
                    UPDATE Customers SET ContactName = 'Palle Ibsen' WHERE CustomerID = 'VAFFE';
                "));
            }

            var root = ResolveRoot(container, SqlTestFile, true);
            var responseSql = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseSql.Code);

            root = ResolveRoot(container, ElasticTestFile, true);
            var responseElastic = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseElastic.Code);

            Assert.AreEqual(2155, es.Count<DynamicResponse>(c => c.Index("northwind").Type("star").QueryOnQueryString("*:*")).Count);

            // FIRST DELTA, NO CHANGES
            root = ResolveRoot(container, ElasticTestFile, false);
            responseElastic = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseElastic.Code);
            Assert.AreEqual(string.Empty, responseElastic.Content);

            Assert.AreEqual(2155, es.Count<DynamicResponse>(c => c.Index("northwind").Type("star").QueryOnQueryString("*:*")).Count);


            // CHANGE 2 FIELDS IN 1 RECORD IN MASTER TABLE THAT WILL CAUSE CALCULATED FIELD TO BE UPDATED TOO 
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                const string sql = @"UPDATE [Order Details] SET UnitPrice = 15, Quantity = 40 WHERE OrderId = 10253 AND ProductId = 39;";
                Assert.AreEqual(1, cn.Execute(sql));
            }

            // RUN AND CHECK SQL
            root = ResolveRoot(container, SqlTestFile, false);
            responseSql = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Content);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT TOP 1 Updates FROM NorthWindControl WHERE Entity = 'Order Details' AND BatchId = 9;"));
                Assert.AreEqual(15.0, cn.ExecuteScalar<decimal>("SELECT OrderDetailsUnitPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
                Assert.AreEqual(40, cn.ExecuteScalar<int>("SELECT OrderDetailsQuantity FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
                Assert.AreEqual(15.0 * 40, cn.ExecuteScalar<int>("SELECT OrderDetailsExtendedPrice FROM NorthWindStar WHERE OrderDetailsOrderId= 10253 AND OrderDetailsProductId = 39;"));
            }

            // RUN AND CHECK ELASTIC
            root = ResolveRoot(container, ElasticTestFile, false);
            responseElastic = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseElastic.Code);
            Assert.AreEqual(string.Empty, responseElastic.Content);

            var response = es.Search<DynamicResponse>(s=> s
                .Index("northwind")
                .Type("star")
                .From(0)
                .Size(1)
                .DefaultOperator(DefaultOperator.And)
                .Query(q=>q.Term("orderdetailsorderid",10253) && q.Term("orderdetailsproductid", 39))
            );

            Assert.AreEqual(response.Documents.First()["orderdetailsunitprice"], 15.0);
            Assert.AreEqual(response.Documents.First()["orderdetailsquantity"], 40);
            Assert.AreEqual(response.Documents.First()["orderdetailsextendedprice"], 40*15.0);

            // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
            }

            // RUN AND CHECK SQL
            root = ResolveRoot(container, SqlTestFile, false);
            responseSql = new PipelineAction(root).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Content);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Orders' AND BatchId = 18;"));
                Assert.AreEqual("VICTE", cn.ExecuteScalar<string>("SELECT OrdersCustomerId FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
                Assert.AreEqual(20.11, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
            }

            // RUN AND CHECK ELASTIC
            root = ResolveRoot(container, ElasticTestFile, false);
            responseElastic = new PipelineAction(root).Execute();
            Assert.AreEqual(200, responseElastic.Code);
            Assert.AreEqual(string.Empty, responseElastic.Content);

            response = es.Search<DynamicResponse>(s => s
                .Index("northwind")
                .Type("star")
                .From(0)
                .Size(1)
                .Query(q => q.Term("orderdetailsorderid", 10254))
            );

            Assert.AreEqual(response.Documents.First()["orderscustomerid"], "VICTE");
            Assert.AreEqual(response.Documents.First()["ordersfreight"], 20.11);

        }
    }
}
