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
using Transformalize.Impl;
using Transformalize.Providers.Console;
using Transformalize.Transforms;

namespace Tests {

   [TestClass]
   public class TestToRow {

      [TestMethod]
      public void ToRow() {

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
            <add name='Value' t='copy(Input1,Input2,Input3).toArray().toRow()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);

         var t1 = new List<TransformHolder>() {
            new TransformHolder((c) => new ToArrayTransform(c), new ToArrayTransform().GetSignatures()),
            new TransformHolder((c) => new ToRowTransform(c), new ToRowTransform().GetSignatures())
         }.ToArray();

         using (var cfgScope = new ConfigurationContainer(t1).CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            var t2 = new List<TransformHolder>() {
               new TransformHolder((c) => new ToArrayTransform(c), new ToArrayTransform().GetSignatures()),
               new TransformHolder((c) => new ToRowTransform(c, new RowFactory(process.Entities.First().GetAllFields().Count(),true,false)), new ToRowTransform().GetSignatures())
            }.ToArray();

            using (var scope = new Container(t2).CreateScope(process, logger)) {
               scope.Resolve<IProcessController>().Execute();
               var output = process.Entities.First().Rows;
               Assert.AreEqual(3, output.Count);
               Assert.AreEqual("2", output[0]["Value"]);
               Assert.AreEqual("4", output[1]["Value"]);
               Assert.AreEqual("6", output[2]["Value"]);

            }
         }
      }
   }
}
