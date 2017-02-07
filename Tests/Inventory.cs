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
using System.Linq;
using Autofac;
using Cfg.Net.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoorMansTSqlFormatterLib;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Desktop.Loggers;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Provider.Ado.Ext;
using Transformalize.Provider.SqlServer;

namespace Tests {

    [TestClass]
    public class Inventory {


        [TestMethod]
        [Ignore]
        public void InventoryQuery() {
            const string expected = @"SELECT [InventoryKey]
	,[Id]
	,[Timestamp]
	,[StatusChangeTimestamp]
	,[PartKey]
	,[StorageLocationKey]
	,[SerialNo1]
	,[SerialNo2]
	,[SerialNo3]
	,[SerialNo4]
	,[Pallet]
	,[Lot]
	,[ShipmentOrder]
	,[DateReceived]
	,[DateInstalled]
	,[LocationInstalled]
	,[Notes]
	,[InventoryStatusId]
	,[Hide]
	,[SS_RowVersion]
FROM [Inventory]
WHERE ([InventoryStatusId] = 80)
ORDER BY [InventoryStatusId] ASC
";

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(@"Files\Shorthand.xml"));
            var container = builder.Build();

            var process = container.Resolve<Process>(new NamedParameter("cfg", @"C:\temp\Inventory.xml"));
            var context = new PipelineContext(new TraceLogger(), process, process.Entities[0]);
            var input = new InputContext(context, new Incrementer(context));
            process.Entities[0].Filter.Add(new Filter { Field = "InventoryStatusId", Value = "80" }.WithDefaults());
            process.Entities[0].Order.Add(new Order {Field = "InventoryStatusId"}.WithDefaults());
            var cf = new SqlServerConnectionFactory(input.Connection);
            var sql = new SqlFormattingManager().Format(input.SqlSelectInput(process.Entities[0].GetAllFields().Where(f => f.Input).ToArray(), cf, input.ResolveOrder));

            Assert.AreEqual(expected, sql);
        }

    }


}
