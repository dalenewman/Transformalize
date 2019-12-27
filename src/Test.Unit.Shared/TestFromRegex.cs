#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
   public class TestFromRegex {

      [TestMethod]
      public void FromRegexWorks() {
         const string xml = @"
    <add name='Test'>
      <entities>
        <add name='Test'>
          <rows>
            <add address='1ST AV 1011' />
            <add address='1ST AV 402 APT 1' />
            <add address='ABLE RD 101 LOT 3' />
          </rows>
          <fields>
            <add name='address' length='128'>
                <transforms>
                    <add method='fromregex' pattern='( \d+)'>
                        <fields>
                            <add name='number' />
                        </fields>
                    </add>
                </transforms>
            </add>
          </fields>
          <calculated-fields>
            <add name='numberspace' t='copy(number).trim().append( )' />
            <add name='flipped' t='copy(address).replace(number,).prepend(numberspace)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               var flipped = process.GetField("flipped");

               Assert.AreEqual(3, output.Length);

               Assert.AreEqual("1011 1ST AV", output[0][flipped]);
               Assert.AreEqual("402 1ST AV APT 1", output[1][flipped]);
               Assert.AreEqual("101 ABLE RD LOT 3", output[2][flipped]);

               foreach (var row in output) {
                  Console.WriteLine(row);
               }
            }
         }

      }
   }
}
