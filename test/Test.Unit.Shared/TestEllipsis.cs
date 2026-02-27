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
   public class TestEllipsis {

      [TestMethod]
      public void TryEllipsis() {
         const string xml = @"
    <add name='TestDistinct'>
      <entities>
        <add name='Dates'>
          <rows>
            <add desc='Once upon a time there was a dude' />
            <add desc='The end' />
          </rows>
          <fields>
            <add name='desc' length='max' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(desc).ellipsis(15)' />
            <add name='t2' t='copy(desc).ellipsis(5, blah blah blah)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               scope.Resolve<IProcessController>().Execute();

               var row1 = process.Entities.First().Rows[0];
               var row2 = process.Entities.First().Rows[1];

               Assert.AreEqual("Once upon a tim...", row1["t1"], "Should stop after 15 characters and do ...");
               Assert.AreEqual("The end", row2["t1"], "Should be the original since it's less than 15 characters");
               Assert.AreEqual("Once blah blah blah", row1["t2"], "Should stop after 5 non empty characters and do blah blah blah");
               Assert.AreEqual("The e blah blah blah", row2["t2"], "Should stop after 5 characters and do blah blah blah");
            }
         }
      }
   }
}
