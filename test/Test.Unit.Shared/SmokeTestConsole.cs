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
using Transformalize.Providers.Console.Autofac;

namespace Tests {

   [TestClass]
   public class TestConsole {

      [TestMethod]
      public void Test() {

         const string xml = @"
    <add name='TestProcess'>
      <connections>
         <add name='input' provider='internal' />
         <add name='output' provider='console' format='text' />
      </connections>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='1' Field2='2' Field3='3' />
            <add Field1='4' Field2='5' Field3='6' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' type='double' />
          </fields>
          <calculated-fields>
            <add name='Format' t='format({Field1}-{Field2}+{Field3} ).trim()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container(new ConsoleProviderModule()).CreateScope(process, logger)) {

               var controller = scope.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)2, process.Entities.First().Inserts);

            }
         }


      }
   }
}
