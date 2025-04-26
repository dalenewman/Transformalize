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
   public class TagTransform {

      [TestMethod]
      public void Tag() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='1' />
            <add Field1='5' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='span' t='copy(Field1).tag(span)' />
            <add name='div' t='copy(Field1).tag(div,class:fun)' />
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
               Assert.AreEqual("<span>1</span>", output[0][cf[0]]);
               Assert.AreEqual("<div class=\"fun\">1</div>", output[0][cf[1]]);
               Assert.AreEqual("<span>5</span>", output[1][cf[0]]);
               Assert.AreEqual("<div class=\"fun\">5</div>", output[1][cf[1]]);
            }
         }

      }
   }
}
