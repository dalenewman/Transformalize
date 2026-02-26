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
   public class TestSlice {

      [TestMethod]
      public void SliceWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='stranger things in the upside down' Field2='10.2' Field3='jobs.mineplex.com' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' type='double' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).slice(1:3, )' />
            <add name='t2' t='copy(Field2).slice(1,.)' />
            <add name='t3' t='copy(Field3).slice(0:1,.)' />
            <add name='t4' t='copy(Field1).slice(2, )' />
            <add name='t5' t='copy(Field3).slice(-2,.)' />
            <add name='t6' t='copy(Field1).slice(::2, )' />
            <add name='t7' t='copy(Field1).slice(3:0:-1, )' />
            <add name='t8' t='copy(Field2).slice(0:2)' />
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
               Assert.AreEqual("things in", output[0][cf[0]]);
               Assert.AreEqual("2", output[0][cf[1]]);
               Assert.AreEqual("jobs", output[0][cf[2]]);
               Assert.AreEqual("in the upside down", output[0][cf[3]]);
               Assert.AreEqual("mineplex.com", output[0][cf[4]]);
               Assert.AreEqual("stranger in upside", output[0][cf[5]]);
               Assert.AreEqual("the in things", output[0][cf[6]]);
               Assert.AreEqual("10", output[0][cf[7]]);
            }
         }


      }
   }
}
