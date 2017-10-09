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
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging;

namespace Tests {
    public class TestQuery {

        [TestMethod]
        [Ignore]
        public void Test1() {

            const string xml = @"
<add name='p1' mode='meta'>
    <connections>
        <add name='input' provider='sqlserver' server='localhost' database='TflNorthWind' />
    </connections>
    <entities>
        <add name='e1' query='select OrderDetailsDiscount, OrderDetailsOrderID from NorthWindStar;' />
    </entities>
</add>";

            var builder = new ContainerBuilder();

            builder.Register<ISchemaReader>((ctx, p) => {
                var process = p.TypedAs<Process>();
                var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(new NullLogger(), process);
                return new RunTimeSchemaReader(process, context);
            }).As<ISchemaReader>();

            builder.RegisterModule(new ShorthandTransformModule());
            builder.RegisterModule(new RootModule());

            using (var scope = builder.Build().BeginLifetimeScope()) {
                var process = scope.Resolve<Process>(new NamedParameter("cfg", xml));

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        System.Diagnostics.Trace.WriteLine(error);
                    }
                    throw new Exception(string.Join(System.Environment.NewLine, process.Errors()));
                }

                using (var s = DefaultContainer.Create(process, new DebugLogger(LogLevel.Debug), "@()")) {
                    var rows = s.Resolve<IProcessController>().Read().ToArray();
                    Assert.AreEqual(2155, rows.Length);
                }

            }
        }

    }
}
