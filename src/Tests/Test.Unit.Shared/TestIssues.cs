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
   public class TestIssues {

      [TestMethod]
      public void TestIssue1() {

         const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add Pile='1Stories: 2Installed on: 09/14/2016 FSO: scpActual Appoi' />
            </rows>
            <fields>
                <add name='Pile' length='max'/>
            </fields>
            <calculated-fields>
                <add name='WithShorthand' t='copy(Pile).tolower().matching(installed on:\s*\d{1,2}\/\d{1,2}\/\d{2,4})' />
                <add name='WithLonghand'>
                  <transforms>
                     <add method='copy' value='Pile' />
                     <add method='tolower' />
                     <add method='matching' pattern='installed on:\s*\d{1,2}\/\d{1,2}\/\d{2,4}' />
                  </transforms>
                </add>
            </calculated-fields>
        </add>
    </entities>
</add>";

         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, logger)) {
               var row = inner.Resolve<IProcessController>().Read().ToArray()[0];
               var withShorthand = process.Entities[0].CalculatedFields[0];
               var withLonghand = process.Entities[0].CalculatedFields[1];
               Assert.AreEqual("installed on: 09/14/2016", row[withShorthand]);
               Assert.AreEqual("installed on: 09/14/2016", row[withLonghand]);

            }
         }

      }

   }
}
