#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Transforms.Razor.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   public class TestTransform {

      /// <summary>
      /// note: just copied jint test to do this faster
      /// </summary>
      [TestMethod]
      public void BasicTests() {

         var xml = @"
<add name='TestProcess' read-only='false'>
    <entities>
        <add name='TestData'>
            <rows>
                <add number1='1' number2='1.0' text1='One' text2='One' />
                <add number1='2' number2='2.0' text1='Two' text2='Two' />
                <add number1='3' number2='3.0' text1='Three' text2='Three' />
            </rows>
            <fields>
                <add name='number1' type='int' primary-key='true' />
                <add name='number2' type='double' />
                <add name='text1' />
                <add name='text2' />
            </fields>
            <calculated-fields>
                <add name='added' type='double' t='razor(@{var x = Model.number1 + Model.number2; }@x)' />
                <add name='joined' t='razor(@{var x = Model.text1 + Model.text2;}@x)' />
                <add name='if' t='razor(@{var x = Model.text1 == ""Two"" || Model.text2 == ""Two"" || Model.number1 == 2 || Model.number2 == 2.0 ? ""It is Two"" : ""It is not Two"";}@x)' />
                <add name='is2' t='razor(@{var x = Model.number1 == 2;}@x)' type='bool' />
            </calculated-fields>
        </add>
    </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer(new RazorTransformModule()).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new RazorTransformModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(2.0, rows[0]["added"]);
               Assert.AreEqual(4.0, rows[1]["added"]);
               Assert.AreEqual(6.0, rows[2]["added"]);

               Assert.AreEqual("OneOne", rows[0]["joined"]);
               Assert.AreEqual("TwoTwo", rows[1]["joined"]);
               Assert.AreEqual("ThreeThree", rows[2]["joined"]);

               Assert.AreEqual("It is not Two", rows[0]["if"]);
               Assert.AreEqual("It is Two", rows[1]["if"]);
               Assert.AreEqual("It is not Two", rows[2]["if"]);

               Assert.AreEqual(false, rows[0]["is2"]);
               Assert.AreEqual(true, rows[1]["is2"]);
               Assert.AreEqual(false, rows[2]["is2"]);

            }
         }
      }
   }
}