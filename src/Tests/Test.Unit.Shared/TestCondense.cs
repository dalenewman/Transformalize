#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

    [TestClass]
    public class TestCondense {

        [TestMethod]
        public void Condense() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='Some  duplicates spa ces' />
            <add Field1='O n e  S p a c ee' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='CondenseField1' t='copy(Field1).condense()' />
            <add name='DuplicateE' t='copy(Field1).condense(e)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var logger = new ConsoleLogger(LogLevel.Debug);
            using (var outer = new ConfigurationContainer().CreateScope(xml, logger, null, "@()")) {
                var process = outer.Resolve<Process>();
                using (var inner = new Container().CreateScope(process, logger)) {

                    var results = inner.Resolve<IProcessController>().Read().ToArray();

                    var condensed = process.Entities.First().CalculatedFields.First();
                    var duplicate = process.Entities.First().CalculatedFields.Last();
                    Assert.AreEqual("Some duplicates spa ces", results[0][condensed]);
                    Assert.AreEqual("O n e S p a c ee", results[1][condensed]);
                    Assert.AreEqual("O n e  S p a c e", results[1][duplicate]);
                }
            }



        }
    }
}
