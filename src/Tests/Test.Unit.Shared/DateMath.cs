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
using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Dates;

namespace Tests {

   [TestClass]
   public class DateMath {

      [TestMethod]
      public void TrySomeDateMath() {
         const string xml = @"
    <add name='TestDateMath'>
      <entities>
        <add name='Dates'>
          <rows>
            <add Date='2017-01-01 9 AM' />
            <add Date='2016-06-07 12:31:22' />
          </rows>
          <fields>
            <add name='Date' type='datetime' />
          </fields>
          <calculated-fields>
            <add name='RoundToMonth' type='datetime' t='copy(Date).dateMath(/M)' />
            <add name='AddOneHourOneMinute' type='datetime' t='copy(Date).DateMath(+1h+1m)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         var dateMath = new TransformHolder((c)=>new DateMathTransform(c), new DateMathTransform().GetSignatures());

         using (var cfgScope = new ConfigurationContainer(dateMath).CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container(dateMath).CreateScope(process, logger)) {
               scope.Resolve<IProcessController>().Execute();

               var row1 = process.Entities.First().Rows[0];
               var row2 = process.Entities.First().Rows[1];

               Assert.AreEqual(new DateTime(2017, 1, 1, 0, 0, 0), row1["RoundToMonth"]);
               Assert.AreEqual(new DateTime(2017, 1, 1, 10, 1, 0), row1["AddOneHourOneMinute"]);

               Assert.AreEqual(new DateTime(2016, 6, 1, 0, 0, 0), row2["RoundToMonth"]);
               Assert.AreEqual(new DateTime(2016, 6, 7, 13, 32, 22), row2["AddOneHourOneMinute"]);
            }
         }



      }

   }
}
