#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright © 2013-2023 Dale Newman
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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Jint.Autofac;

namespace Tests {

   [TestClass]
   public class TestShorthandScript {

      [TestMethod]
      public void Run() {

         var logger = new ConsoleLogger(LogLevel.Debug);

         var xml = @"
<add name='Test' read-only='true'>
    <scripts>
        <add name='s1' file='scripts/script.js' />
    </scripts>
    <entities>
        <add name='Data'>
            <rows>
                <add number1='1' number2='1.0' />
                <add number1='2' number2='2.0' />
                <add number1='3' number2='3.0' />
            </rows>
            <fields>
                <add name='number1' type='int' primary-key='true' />
                <add name='number2' type='double' />
            </fields>
            <calculated-fields>
                <add name='scripted' type='double' t='jint(s1)' />
            </calculated-fields>
        </add>
    </entities>

</add>";
         using (var outer = new ConfigurationContainer(new JintTransformModule()).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JintTransformModule()).CreateScope(process, new ConsoleLogger(LogLevel.Debug))) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(-1.0, rows[0]["scripted"]);
               Assert.AreEqual(4.0, rows[1]["scripted"]);
               Assert.AreEqual(9.0, rows[2]["scripted"]);
            }
         }
      }
   }
}
