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
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging;
using Transformalize.Providers.SqlServer;

namespace Tests {

    [TestClass]
    public class SlaveGetsInserts : TestBase {

        const string xml = @"
<cfg name='Test' mode='@(Mode)' flatten='true'>
  <parameters>
    <add name='Mode' value='default' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' database='TestInput' />
    <add name='output' provider='sqlserver' database='TestOutput' />
  </connections>
  <entities>
    <add name='MasterTable'>
        <fields>
            <add name='Id' type='int' primary-key='true' />
            <add name='d1' />
            <add name='d2' />
        </fields>
    </add>
    <add name='SlaveTable'>
        <fields>
            <add name='Id' type='int' primary-key='true' />
            <add name='d3' />
            <add name='d4' />
        </fields>
    </add>
  </entities>
  <relationships>
    <add left-entity='MasterTable' left-field='Id' right-entity='SlaveTable' right-field='Id' />
  </relationships>
</cfg>
";

        public Connection InputConnection { get; set; } = new Connection {
            Name = "input",
            Provider = "sqlserver",
            ConnectionString = "server=localhost;database=TestInput;trusted_connection=true;"
        };

        public Connection OutputConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "sqlserver",
            ConnectionString = "Server=localhost;Database=TestOutput;trusted_connection=true;"
        };

        [TestMethod]
        [Ignore]
        public void SlaveGetsInserts_Integration() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ShorthandModule());
            builder.RegisterModule(new RootModule());
            var container = builder.Build();

            // SETUP 
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(3, cn.Execute(@"
                    IF OBJECT_ID('MasterTable') IS NULL
	BEGIN
		create table MasterTable(
			Id int not null primary key,
			d1 nvarchar(64) not null,
			d2 nvarchar(64) not null
		);
END

IF OBJECT_ID('SlaveTable') IS NULL
	BEGIN
		create table SlaveTable(
			Id int not null primary key,
			d3 nvarchar(64) not null,
			d4 nvarchar(64) not null
		);
	END

TRUNCATE TABLE MasterTable;
TRUNCATE TABLE SlaveTable;

INSERT INTO MasterTable(Id,d1,d2)VALUES(1,'d1','d2');
INSERT INTO MasterTable(Id,d1,d2)VALUES(2,'d3','d4');

INSERT INTO SlaveTable(Id,d3,d4)VALUES(1,'d5','d6');

                "));
            }

            // RUN INIT AND TEST
            var root = ResolveRoot(container, xml, new Dictionary<string, string>() { { "Mode", "init" } });
            var responseSql = Execute(root);

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(2, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestMasterTable;"));
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestSlaveTable;"));
            }

            // FIRST DELTA, NO CHANGES
            root = ResolveRoot(container, xml);
            responseSql = Execute(root);

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(2, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestMasterTable;"));
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestSlaveTable;"));
                Assert.AreEqual(2, cn.ExecuteScalar<int>("select Id from TestStar where d3 = '' and d4 = '';"));

            }

            

            // insert into slave
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                const string sql = @"INSERT INTO SlaveTable(Id,d3,d4)VALUES(2,'d7','d8');";
                Assert.AreEqual(1, cn.Execute(sql));
            }

            // RUN AND CHECK
            root = ResolveRoot(container, xml);
            responseSql = Execute(root);

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(2, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestMasterTable;"));
                Assert.AreEqual(2, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestSlaveTable;"));
                Assert.AreEqual(0, cn.ExecuteScalar<int>("select Id from TestStar where d3 = '' and d4 = '';"));
                Assert.AreEqual(2, cn.ExecuteScalar<int>("select Id from TestStar where d3 = 'd7' and d4 = 'd8';"));
            }

            /*

            // CHANGE 1 RECORD'S CUSTOMERID AND FREIGHT ON ORDERS TABLE
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.Execute("UPDATE Orders SET CustomerID = 'VICTE', Freight = 20.11 WHERE OrderId = 10254;"));
            }

            root = ResolveRoot(container, TestFile);
            responseSql = Execute(root);

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Orders' AND BatchId = 26;"));

                Assert.AreEqual("VICTE", cn.ExecuteScalar<string>("SELECT OrdersCustomerId FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
                Assert.AreEqual(20.11M, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));
                Assert.AreEqual(26, cn.ExecuteScalar<int>("SELECT TflBatchId FROM NorthWindStar WHERE OrderDetailsOrderId= 10254;"));

                Assert.AreEqual("VICTE", cn.ExecuteScalar<string>("SELECT OrdersCustomerId FROM NorthWindFlat WHERE OrderDetailsOrderId= 10254;"));
                Assert.AreEqual(20.11M, cn.ExecuteScalar<decimal>("SELECT OrdersFreight FROM NorthWindFlat WHERE OrderDetailsOrderId= 10254;"));
                Assert.AreEqual(26, cn.ExecuteScalar<int>("SELECT TflBatchId FROM NorthWindFlat WHERE OrderDetailsOrderId= 10254;"));
            }

            // CHANGE A CUSTOMER'S CONTACT NAME FROM Palle Ibsen TO Paul Ibsen
            using (var cn = new SqlServerConnectionFactory(InputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.Execute("UPDATE Customers SET ContactName = 'Paul Ibsen' WHERE CustomerID = 'VAFFE';"));
            }

            root = ResolveRoot(container, TestFile);
            responseSql = Execute(root);

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(1, cn.ExecuteScalar<int>("SELECT Updates FROM NorthWindControl WHERE Entity = 'Customers' AND BatchId = 35;"));

                Assert.AreEqual("Paul Ibsen", cn.ExecuteScalar<string>("SELECT DISTINCT CustomersContactName FROM NorthWindStar WHERE OrdersCustomerID = 'VAFFE';"));
                Assert.AreEqual(35, cn.ExecuteScalar<int>("SELECT DISTINCT TflBatchId FROM NorthWindStar WHERE OrdersCustomerID = 'VAFFE';"), "The TflBatchId should be updated on the master to indicate a change has occured.");

                Assert.AreEqual("Paul Ibsen", cn.ExecuteScalar<string>("SELECT DISTINCT CustomersContactName FROM NorthWindFlat WHERE OrdersCustomerID = 'VAFFE';"));
                Assert.AreEqual(35, cn.ExecuteScalar<int>("SELECT DISTINCT TflBatchId FROM NorthWindFlat WHERE OrdersCustomerID = 'VAFFE';"), "The TflBatchId should be updated on the master to indicate a change has occured.");

            }
            */

        }
    }
}
