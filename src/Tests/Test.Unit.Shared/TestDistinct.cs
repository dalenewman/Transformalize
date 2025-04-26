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
   public class TestDistinct {

      [TestMethod]
      public void TryDistinct() {
         const string xml = @"
    <add name='TestDistinct'>
      <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='internal' />
      </connections>
      <entities>
        <add name='Dates'>
          <rows>
            <add Words='One Two Three One' />
            <add Words='111-222-3333 222-333-4444' />
          </rows>
          <fields>
            <add name='Words' />
          </fields>
          <calculated-fields>
            <add name='DistinctWords' t='copy(Words).split( ).distinct().join( )' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               scope.Resolve<IProcessController>().Execute();
               var row1 = process.Entities.First().Rows[0];
               var row2 = process.Entities.First().Rows[1];

               Assert.AreEqual("One Two Three", row1["DistinctWords"]);
               Assert.AreEqual("111-222-3333 222-333-4444", row2["DistinctWords"]);
            }
         }
      }
   }
}
