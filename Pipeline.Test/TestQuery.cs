#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac;
using Pipeline.Ioc.Autofac.Modules;
using Pipeline.Logging;

namespace Pipeline.Test {
    public class TestQuery {

        [Test]
        [Ignore("Requires SQL Server")]
        public void Test1() {

            const string xml = @"
<add name='p1' mode='meta'>
    <connections>
        <add name='input' provider='sqlserver' server='localhost' database='NorthWindStar' />
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

            builder.RegisterModule(new RootModule("Shorthand.xml"));

            using (var scope = builder.Build().BeginLifetimeScope()) {
                var process = scope.Resolve<Process>(new NamedParameter("cfg", xml));

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        System.Diagnostics.Trace.WriteLine(error);
                    }
                    throw new Exception(string.Join(System.Environment.NewLine, process.Errors()));
                }

                var reader = new RunTimeDataReader(new DebugLogger(LogLevel.Debug));
                var rows = reader.Run(process).ToArray();

                Assert.AreEqual(2155, rows.Length);
            }
        }

    }
}
