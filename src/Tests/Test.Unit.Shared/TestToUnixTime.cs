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
using Transformalize.Transforms;
using Transformalize.Transforms.Dates;

namespace Tests {

   [TestClass]
   public class TestToUnixTime {

      [TestMethod]
      public void Test() {

         const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add Date='2019-07-01 10:30 AM' />
            </rows>
            <fields>
                <add name='Date' type='datetime'/>
            </fields>
            <calculated-fields>
                <add name='Milliseconds' type='long' t='copy(Date).timezone(Eastern Standard Time,UTC).tounixtime(ms)' />
                <add name='Seconds' type='long' t='copy(Date).timezone(Eastern Standard Time,UTC).tounixtime(seconds)' />
            </calculated-fields>
        </add>
    </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         var specifyKind = new TransformHolder((c) => new TimeZoneTransform(c), new TimeZoneTransform().GetSignatures());
         var toUnixTime = new TransformHolder((c)=>new ToUnixTimeTransform(c), new ToUnixTimeTransform().GetSignatures());

         using (var outer = new ConfigurationContainer( new[] { specifyKind, toUnixTime } ).CreateScope(xml, logger)) {

            var process = outer.Resolve<Process>();
            using (var inner = new Container(new[] { specifyKind, toUnixTime }).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               var output = controller.Read().ToArray();
               var milliseconds = (long) output[0][process.Entities.First().CalculatedFields.First()];
               var seconds = (long)output[0][process.Entities.First().CalculatedFields.Last()];

               Assert.AreEqual(1561991400000,  milliseconds);
               Assert.AreEqual(1561991400, seconds);

            }
         }
      }

   }
}
