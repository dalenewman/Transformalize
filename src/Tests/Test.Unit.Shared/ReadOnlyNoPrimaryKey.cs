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

namespace Tests {

   [TestClass]
   public class ReadOnlyNoPrimaryKey {

      [TestMethod]
      public void ItWorks() {
         const string xml = @"
    <add name='Test' read-only='true'>
      <entities>
        <add name='Data'>
          <rows>
            <add Date='2017-01-01 9 AM' Number='1' />
            <add Date='2016-06-07 12:31:22' Number='2' />
          </rows>
          <fields>
            <add name='Date' type='datetime' />
            <add name='Number' type='short' />
          </fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(new DateTime(2017, 1, 1, 9, 0, 0), output[0][process.GetField("Date")]);
               Assert.AreEqual((short)1, output[0][process.GetField("Number")]);
            }
         }



      }

   }
}
