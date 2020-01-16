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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestFindRequiredFields {

      [TestMethod]
      public void TestFieldReference() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Input1='2' Input2='4' Input3='6' />
          </rows>
          <fields>
            <add name='Input1' />
            <add name='Input2' />
            <add name='Input3' />
          </fields>
          <calculated-fields>
            <add name='Value' t='format({Input1} and {Input3})' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var scope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = scope.Resolve<Process>();
            var targetFields = new List<Field> {
               process.Entities[0].CalculatedFields.First(f => f.Name == "Value")
            };
            var required = process.Entities[0].FindRequiredFields(targetFields, process.Maps);
            Assert.AreEqual(2, required.Count());
            Assert.IsTrue(required.Any(f => f.Name == "Input1"));
            Assert.IsFalse(required.Any(f => f.Name == "Input2"));
            Assert.IsTrue(required.Any(f => f.Name == "Input3"));

         }
      }


      [TestMethod]
      public void TestStandardParameter() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Input1='2' Input2='4' Input3='6' />
          </rows>
          <fields>
            <add name='Input1' />
            <add name='Input2' />
            <add name='Input3' />
          </fields>
          <calculated-fields>
            <add name='Value'>
               <transforms>
                  <add method='format' format='{0} and {1}'>
                     <parameters>
                        <add field='Input1' />
                        <add field='Input3' />
                     </parameters>
                  </add>
               </transforms>
            </add>
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var scope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = scope.Resolve<Process>();
            var targetFields = new List<Field> {
               process.Entities[0].CalculatedFields.First(f => f.Name == "Value")
            };
            var required = process.Entities[0].FindRequiredFields(targetFields, process.Maps);
            Assert.AreEqual(2, required.Count());
            Assert.IsTrue(required.Any(f => f.Name == "Input1"));
            Assert.IsFalse(required.Any(f => f.Name == "Input2"));
            Assert.IsTrue(required.Any(f => f.Name == "Input3"));

         }
      }
   }
}

