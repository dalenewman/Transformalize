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
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class MapTransformTester {

      [TestMethod]
      public void MapTransformAdd() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Map'>
            <items>
                <add from='1' to='One' />
                <add from='2' to='Two' />
                <add from='3' parameter='Field3' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Field1='1' Field3='^' />
                <add Field1='2' Field3='#' />
                <add Field1='3' Field3='$THREE$' />
                <add Field1='4' Field3='@' />
            </rows>
            <fields>
                <add name='Field1' />
                <add name='Field3' />
            </fields>
            <calculated-fields>
                <add name='Map' t='copy(Field1).map(map)' default='None' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("One", output[0][field]);
               Assert.AreEqual("Two", output[1][field]);
               Assert.AreEqual("$THREE$", output[2][field]);
               Assert.AreEqual("None", output[3][field]);
            }
         }
      }

      [TestMethod]
      public void MapTransformPassThrough() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Map' pass-through='true'>
            <items>
                <add from='A' to='Letter A' />
                <add from='B' to='Letter B' />
                <add from='C' to='Letter C' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Field1='A' />
                <add Field1='D' />
                <add Field1='B' />
                <add Field1='C' />
            </rows>
            <fields>
                <add name='Field1' />
            </fields>
            <calculated-fields>
                <add name='Map' t='copy(Field1).map(map)' default='None' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("Letter A", output[0][field], "'A' got mapped to 'Letter A'.");
               Assert.AreEqual("D", output[1][field], "'D' didn't get mapped and passed through.");
               Assert.AreEqual("Letter B", output[2][field], "'B' got mapped to 'Letter B'.");
               Assert.AreEqual("Letter C", output[3][field], "'C' got mapped to 'Letter C'.");
            }
         }
      }

   }
}
