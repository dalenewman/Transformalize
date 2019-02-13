#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac;
using Transformalize.Providers.Trace;

namespace Tests {

    [TestClass]
    public class TwoEntitiesReadOnly {

        [TestMethod]
        public void Go() {
            const string xml = @"
    <add name='bsh' read-only='true'>
        <entities>
            <add name='t1'>
                <rows>
                    <add name='dalenewman' />
                    <add name='pywakit' />
                </rows>
                <fields>
                    <add name='name' />
                </fields>
                <calculated-fields>
                    <add name='underscored' t='copy(name).prepend(_)' />
                </calculated-fields>
            </add>
            <add name='t2'>
                <rows>
                    <add name='daryl' />
                    <add name='titanic' />
                </rows>
                <fields>
                    <add name='name' />
                </fields>
                <calculated-fields>
                    <add name='plural' t='copy(name).append(s)' />
                </calculated-fields>
            </add>
        </entities>
    </add>";

            var tracer = new TraceLogger(LogLevel.Debug);
            using (var outer = ConfigurationContainer.Create(xml, tracer, new Dictionary<string, string>(), "@[]")) {
                var process = outer.Resolve<Process>();
                using (var inner = DefaultContainer.Create(process, tracer, "@[]")) {
                    var context = inner.Resolve<IContext>();
                    foreach (var error in process.Errors()) {
                        context.Error(error);
                    }

                    var controller = inner.Resolve<IProcessController>();
                    controller.Execute();

                    Assert.AreEqual(0, process.Errors().Length);
                    Assert.AreEqual(2, process.Entities[0].Rows.Count);
                    Assert.AreEqual("_dalenewman", process.Entities[0].Rows[0]["underscored"]);
                    Assert.AreEqual(2, process.Entities[1].Rows.Count);
                    Assert.AreEqual("titanics", process.Entities[1].Rows[1]["plural"]);
                }
            }


        }

    }
}
