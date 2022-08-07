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

namespace Tests.Validation {

   [TestClass]
   public class TestRequired {

      [TestMethod]
      public void Run() {
         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='present' Field2='here' Field3='' />
            <add Field1='' Field2='present' Field3='here' />
          </rows>
          <fields>
            <add name='Field1' v='required()' />
            <add name='Field2' v='required()' />
            <add name='Field3' v='required()' />
          </fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(true, output[0][process.GetField("Field1Valid")]);
               Assert.AreEqual(true, output[0][process.GetField("Field2Valid")]);
               Assert.AreEqual(false, output[0][process.GetField("Field3Valid")]);

               Assert.AreEqual(false, output[1][process.GetField("Field1Valid")]);
               Assert.AreEqual(true, output[1][process.GetField("Field2Valid")]);
               Assert.AreEqual(true, output[1][process.GetField("Field3Valid")]);

            }
         }

      }

      [TestMethod]
      public void RunCf() {
         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='present' Field2='here' Field3='' />
            <add Field1='' Field2='present' Field3='here' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='Test' t='copy(Field1)' v='required()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual("present", output[0][process.GetField("Test")]);
               Assert.AreEqual("", output[1][process.GetField("Test")]);

               Assert.AreEqual(true, output[0][process.GetField("TestValid")]);
               Assert.AreEqual(false, output[1][process.GetField("TestValid")]);

            }
         }

      }

      [TestMethod]
      public void RunWithDefaults() {
         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='present' Field2='here' Field3='' />
            <add Field1='' Field2='present' Field3='here' />
          </rows>
          <fields>
            <add name='Field1' v='required()' default='x' />
            <add name='Field2' v='required()' default='present' />
            <add name='Field3' v='required()' default='default' default-empty='true' />
          </fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(true, output[0][process.GetField("Field1Valid")]);
               Assert.AreEqual(true, output[0][process.GetField("Field2Valid")]);
               Assert.AreEqual(true, output[0][process.GetField("Field3Valid")], "Field3 is valid without a value because it gets a default value when null or blank.");

               Assert.AreEqual(false, output[1][process.GetField("Field1Valid")], "Field1 is not valid without a value because it only gets a default value when it's null.");
               Assert.AreEqual(true, output[1][process.GetField("Field2Valid")], "Field2 is valid and the default value being the same doesn't break it (like it used to).");
               Assert.AreEqual(true, output[1][process.GetField("Field3Valid")]);

            }
         }

      }
   }
}
