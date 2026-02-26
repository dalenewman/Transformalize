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

namespace Tests.Validation {

   [TestClass]
   public class TestMaxLength {

      [TestMethod]
      public void Run() {
         var xml = @"
    <add name='Test Max Length'>
      <entities>
        <add name='Test'>
          <rows>
            <add Field1='abcdefghijklmnopqrstuvwxyz' Field2='abcdefghijklmnopqrstuvwxyz' Field3='abcdefghijklmnopqrstuvwxyz' />
            <add Field1='0123456789' Field2='0123456789' Field3='0123456789' />
          </rows>
          <fields>
            <add name='Field1' v='maxLength(10)' />
            <add name='Field2' v='maxLength(20)' />
            <add name='Field3' v='maxLength(30)' />
          </fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(false, output[0][process.GetField("Field1Valid")],"invalid because input is longer than 10 characters");
               Assert.AreEqual(false, output[0][process.GetField("Field2Valid")], "invalid because input is longer than 20 characters");
               Assert.AreEqual(true, output[0][process.GetField("Field3Valid")], "valid because input is shorter than 30 characters") ;

               Assert.AreEqual(true, output[1][process.GetField("Field1Valid")], "valid because input is exactly 10 characters");
               Assert.AreEqual(true, output[1][process.GetField("Field2Valid")], "valid because input is shorter than 20 characters");
               Assert.AreEqual(true, output[1][process.GetField("Field3Valid")], "valid because input is shorter than 30 characters");

            }
         }
      }
   }
}
