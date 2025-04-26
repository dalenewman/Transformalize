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
   public class TestToTimeTransform {

      [TestMethod]
      public void ToTime() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add intField='238' longField='500' />
          </rows>
          <fields>
            <add name='intField' type='int' />
            <add name='longField' type='long' />
          </fields>
          <calculated-fields>
            <add name='intFieldToTime' t='copy(intField).toTime(hour)' />
            <add name='longFieldToTime' t='copy(longField).toTime(seconds)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, logger)) {

               var results = inner.Resolve<IProcessController>().Read().ToArray();

               var intFieldToTime = process.Entities[0].CalculatedFields[0];
               var longFieldToTime = process.Entities[0].CalculatedFields[1];

               Assert.AreEqual("9.22:00:00", results[0][intFieldToTime]);
               Assert.AreEqual("00:08:20", results[0][longFieldToTime]);

            }
         }



      }
   }
}
