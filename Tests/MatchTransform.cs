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
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class MatchTransform {

      [TestMethod]
      public void Match() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='Local/3114@tolocalext-fc5d,1' />
            <add Field1='SIP/Generic-Vitel_Outbound-0002b45a' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
          </fields>
          <calculated-fields>
            <add name='MatchField1' t='copy(Field1).match(3[0-9]{3}(?=@))' default='None' />
            <add name='MatchAnyField' t='copy(Field1,Field2).match(3[0-9]{3}(?=@))' default='None' />
            <add name='MatchCount' type='int' t='copy(Field1).matchCount(Vitel)' />
            <add name='Matching' t='copy(Field1).matching([0-9a-zA-Z])' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger();

         using(var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using(var inner = new Container().CreateScope(process, logger)) {
               var output = inner.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("3114", output[0][cf[0]]);
               Assert.AreEqual("3114", output[0][cf[1]]);
               Assert.AreEqual("None", output[1][cf[0]]);
               Assert.AreEqual("None", output[1][cf[1]]);

               Assert.AreEqual(1, output[1][cf[2]]);
               Assert.AreEqual("Local3114tolocalextfc5d1", output[0][cf[3]]);
            }
         }
      }
   }
}
