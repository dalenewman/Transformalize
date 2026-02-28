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
   public class TestAppend {

      [TestMethod]
      public void AppendWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='one' Field2='2' Field3='three' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' type='double' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).append( is 1)' />
            <add name='t2' t='copy(Field2).append( is two)' />
            <add name='t3' t='copy(Field3).append(Field2)' />
            <add name='t4' t='copy(Field3).append(t3)' />
            <add name='t5' t='copy(Field1).append()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("one is 1", output[0][cf[0]]);
               Assert.AreEqual("2 is two", output[0][cf[1]]);
               Assert.AreEqual("three2", output[0][cf[2]]);
               Assert.AreEqual("threethree2", output[0][cf[3]]);
            }
         }


      }
   }
}
