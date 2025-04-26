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
   public class TestAny {

      [TestMethod]
      public void AnyWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='1' Field2='2' Field3='3' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='Has1' type='bool' t='copy(*).any(1)' />
            <add name='Has2' type='bool' t='copy(Field1,Field2,Field3).any(2)' />
            <add name='Has3' type='bool' t='copy(*).any(3)' />
            <add name='Has4' type='bool' t='copy(Field1,Field2,Field3).any(4)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(true, output[0][cf[0]]);
               Assert.AreEqual(true, output[0][cf[1]]);
               Assert.AreEqual(true, output[0][cf[2]]);
               Assert.AreEqual(false, output[0][cf[3]]);
            }
         }
      }
   }
}
