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
   public class FromLengthsTransform {

      [TestMethod]
      public void FromLengths() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add line='122333444455555666666' />
            <add line='111111222223333444556' />
          </rows>
          <fields>
            <add name='line'>
                <transforms>
                    <add method='fromlengths'>
                        <fields>
                            <add name='f1' length='1' />
                            <add name='f2' length='2' />
                            <add name='f3' length='3' type='int' />
                            <add name='f4' length='4' />
                            <add name='f5' length='5' />
                            <add name='f6' length='6' />
                        </fields>
                    </add>
                </transforms>
            </add>
          </fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using(var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using(var inner = new Container().CreateScope(process, logger)) {
               var output = inner.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();

               var first = output[0];
               Assert.AreEqual("1", first[cf[0]]);
               Assert.AreEqual("22", first[cf[1]]);
               Assert.AreEqual(333, first[cf[2]]);
               Assert.AreEqual("4444", first[cf[3]]);
               Assert.AreEqual("55555", first[cf[4]]);
               Assert.AreEqual("666666", first[cf[5]]);

               var second = output[1];
               Assert.AreEqual("1", second[cf[0]]);
               Assert.AreEqual("11", second[cf[1]]);
               Assert.AreEqual(111, second[cf[2]]);
               Assert.AreEqual("2222", second[cf[3]]);
               Assert.AreEqual("23333", second[cf[4]]);
               Assert.AreEqual("444556", second[cf[5]]);

            }
         }
      }
   }
}
