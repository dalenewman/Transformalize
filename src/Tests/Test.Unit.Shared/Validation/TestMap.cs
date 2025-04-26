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

namespace Tests.Validation {

   [TestClass]
   public class TestMap {

      [TestMethod]
      public void MapWithOnlyFrom() {
         var xml = @"
    <add name='TestProcess'>
      <maps>
         <add name='map'>
            <items>
               <add from='1' />
               <add from='2' />
               <add from='3' />
            </items>
         </add>
      </maps>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='2' Field2='4' Field3='6' />
            <add Field1='1' Field2='2' Field3='3' />
          </rows>
          <fields>
            <add name='Field1' v='map(map)' />
            <add name='Field2' v='in(map)' />
            <add name='Field3' v='map(map).invert()' />
          </fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(true, output[0][process.GetField("Field1Valid")], "valid because 2 is in the map");
               Assert.AreEqual(false, output[0][process.GetField("Field2Valid")], "invalid because 4 is not in the map");
               Assert.AreEqual("Field2's value 4 must be one of these 3 items: 1, 2, or 3.|", output[0][process.GetField("Field2Message")]);

               Assert.AreEqual(true, output[0][process.GetField("Field3Valid")], "valid because 6 is not in the map");

               Assert.AreEqual(true, output[1][process.GetField("Field1Valid")], "valid because 1 is in the map");
               Assert.AreEqual(true, output[1][process.GetField("Field2Valid")], "valid because 2 is in the map");
               Assert.AreEqual(false, output[1][process.GetField("Field3Valid")], "invalid because 3 is in the map");
               Assert.AreEqual("Field3's value 3 must not be one of these 3 items: 1, 2, or 3.|", output[1][process.GetField("Field3Message")]);

            }
         }
      }

      [TestMethod]
      public void MapWithFromAndTo() {
         var xml = @"
    <add name='TestProcess'>
      <maps>
         <add name='map'>
            <items>
               <add from='One' to='1' />
               <add from='Two' to='2' />
               <add from='Three' to='3' />
            </items>
         </add>
      </maps>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='2' Field2='4' Field3='6' />
            <add Field1='1' Field2='2' Field3='3' />
          </rows>
          <fields>
            <add name='Field1' v='map(map)' />
            <add name='Field2' v='in(map)' />
            <add name='Field3' v='map(map).invert()' />
          </fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(true, output[0][process.GetField("Field1Valid")], "valid because 2 is in the map");
               Assert.AreEqual(false, output[0][process.GetField("Field2Valid")], "invalid because 4 is not in the map");
               Assert.AreEqual("Field2 must be one of these 3 items: One, Two, or Three.|", output[0][process.GetField("Field2Message")]);

               Assert.AreEqual(true, output[0][process.GetField("Field3Valid")], "valid because 6 is not in the map");

               Assert.AreEqual(true, output[1][process.GetField("Field1Valid")], "valid because 1 is in the map");
               Assert.AreEqual(true, output[1][process.GetField("Field2Valid")], "valid because 2 is in the map");
               Assert.AreEqual(false, output[1][process.GetField("Field3Valid")], "invalid because 3 is in the map");
               Assert.AreEqual("Field3 must not be one of these 3 items: One, Two, or Three.|", output[1][process.GetField("Field3Message")]);

            }
         }
      }
   }
}
