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
   public class TestContains {

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
            <add name='Field1' v='contains(pre)' />
            <add name='Field2' v='contains(h)' />
            <add name='Field3' v='contains(ere).invert()' />
          </fields>
          <calculated-fields>
            <add name='Multiple' t='copy(Field1,Field2,Field3)' v='contains(pres)' />
          </calculated-fields>
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
               Assert.AreEqual(true, output[0][process.GetField("Field3Valid")]);
               Assert.AreEqual(true, output[0][process.GetField("MultipleValid")]);

               Assert.AreEqual(false, output[1][process.GetField("Field1Valid")]);
               Assert.AreEqual(false, output[1][process.GetField("Field2Valid")]);
               Assert.AreEqual(false, output[1][process.GetField("Field3Valid")]);
               Assert.AreEqual(true, output[1][process.GetField("MultipleValid")]);

            }
         }

      }


   }
}
