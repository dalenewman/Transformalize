﻿#region license
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
   public class DatePartTransform {

      [TestMethod]
      public void DatePart1() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' >
          <rows>
            <add StartDate='2016-06-01' EndDate='2016-08-01' />
          </rows>
          <fields>
            <add name='StartDate' type='datetime' />
            <add name='EndDate' type='datetime' />
          </fields>
          <calculated-fields>
            <add name='StartYear' type='int' t='copy(StartDate).datepart(year)' />
            <add name='EndYear' type='int' t='copy(EndDate).datepart(year)' />
            <add name='StartWeek' type='int' t='copy(StartDate).datepart(weekofyear)' />
            <add name='EndWeek' type='int' t='copy(EndDate).datepart(weekofyear)' />
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
               Assert.AreEqual(2016, output[0][cf[0]]);
               Assert.AreEqual(2016, output[0][cf[1]]);
               Assert.AreEqual(23, output[0][cf[2]]);
               Assert.AreEqual(32, output[0][cf[3]]);
            }
         }



      }
   }
}
