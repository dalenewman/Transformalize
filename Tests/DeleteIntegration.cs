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
using System.Collections.Generic;
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
    public class DeleteIntegration {

        public Connection InputConnection { get; set; } = new Connection {
            Name = "input",
            Provider = "sqlserver",
            ConnectionString = "server=localhost;database=NorthWind;trusted_connection=true;"
        };

        public Connection OutputConnection { get; set; } = new Connection {
            Name = "output",
            Provider = "sqlserver",
            ConnectionString = "Server=localhost;Database=NorthWindStar;trusted_connection=true;"
        };

        public Process ResolveRoot(IContainer container, string cfg, IDictionary<string, string> parameters) {
            return container.Resolve<Process>(new NamedParameter("cfg", cfg), new NamedParameter("parameters", parameters));
        }


        [TestMethod]
        [Ignore]
        public void Delete_Integration() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(@"Shorthand.xml"));
            var container = builder.Build();

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

            const string cfg = @"<cfg name='TestDeletes' mode='@(Mode)'>
    <connections>
        <add name='input' provider='sqlserver' database='NorthWind' />
        <add name='output' provider='sqlserver' database='NorthWindStar' />
    </connections>
<entities>
    <add name='TestDeletes' alias='Data' delete='true' version='RowVersion'>
        <fields>
            <add name='TextValue' />
            <add name='Id' primary-key='true' type='int' />
            <add name='NumericValue' primary-key='true' type='int' />
            <add name='RowVersion' type='byte[]' length='8' />
        </fields>
    </add>
</entities>
</cfg>";

            // RUN INIT AND TEST
            var root = ResolveRoot(container, cfg, InitMode());
            var responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
            }

            // FIRST DELTA, NO CHANGES
            root = ResolveRoot(container, cfg, DefaultMode());
            responseSql = new PipelineAction(root,new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

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
            root = ResolveRoot(container, cfg, DefaultMode());
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar;"));
                Assert.AreEqual(1, cn.ExecuteScalar<decimal>("SELECT COUNT(*) FROM TestDeletesStar WHERE TflDeleted = 1;"));
            }

            // RUN AGAIN
            root = ResolveRoot(container, cfg, DefaultMode());
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

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
            root = ResolveRoot(container, cfg, DefaultMode());
            responseSql = new PipelineAction(root, new PipelineContext(new DebugLogger(), root)).Execute();

            Assert.AreEqual(200, responseSql.Code);
            Assert.AreEqual(string.Empty, responseSql.Message);

            using (var cn = new SqlServerConnectionFactory(OutputConnection).GetConnection()) {
                cn.Open();
                Assert.AreEqual(3, cn.ExecuteScalar<int>("SELECT COUNT(*) FROM TestDeletesStar WHERE TflDeleted = 0;"));
            }


        }

        private static Dictionary<string, string> InitMode() {
            return new Dictionary<string, string> { { "Mode", "init" } };
        }

        private static Dictionary<string, string> DefaultMode() {
            return new Dictionary<string, string> { { "Mode", "default" } };
        }
    }
}
