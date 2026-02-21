#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright © 2013-2023 Dale Newman
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

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Validate.Jint.Autofac;

namespace Tests {

   [TestClass]
   public class TestSometimesRequired {

      [TestMethod]
      public void Run() {

         var logger = new ConsoleLogger();

         const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add id='1' store='Walmart' other='' />
                <add id='2' store='Target' other='' />
                <add id='3' store='Other' other='' />
                <add id='4' store='Other' other='Costco' />
            </rows>
            <fields>
                <add name='id' type='int' primary-key='true' />
                <add name='store' />
                <add name='other' v=""jint(
   if(store === 'Other' && other === '') {
      otherMessage = 'other is required';
      false;
   } else {
      true;
   }
)""/>
            </fields>
            <calculated-fields>
               
            </calculated-fields>
        </add>
    </entities>

</add>";
         using (var outer = new ConfigurationContainer(new JintValidateModule()).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();

            if (process.Errors().Any()) {
               foreach(var error in process.Errors()) {
                  System.Console.WriteLine(error);
               }
               return;
            }
            using (var inner = new Container(new JintValidateModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(true, rows[0]["otherValid"]);
               Assert.AreEqual("", rows[0]["otherMessage"]);

               Assert.AreEqual(true, rows[1]["otherValid"]);
               Assert.AreEqual("", rows[1]["otherMessage"]);

               Assert.AreEqual(false, rows[2]["otherValid"]);
               Assert.AreEqual("other is required|", rows[2]["otherMessage"]);

               Assert.AreEqual(true, rows[3]["otherValid"]);
               Assert.AreEqual("", rows[3]["otherMessage"]);

            }
         }
      }
   }
}
