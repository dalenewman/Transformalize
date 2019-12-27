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
using Transformalize.Transforms;

namespace Tests {

   [TestClass]
   public class TestSplitAndJoin {

      [TestMethod]
      public void Join() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Input='1 2 3 4' />
          </rows>
          <fields>
            <add name='Input' />
          </fields>
          <calculated-fields>
            <add name='Joined' t='copy(Input).split( ).join(-)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         var transforms = new List<TransformHolder>() {
            new TransformHolder((c) => new JoinTransform(c), new JoinTransform().GetSignatures()),
            new TransformHolder((c) => new SplitTransform(c), new SplitTransform().GetSignatures())
         }.ToArray();

         using (var cfgScope = new ConfigurationContainer(transforms).CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container(transforms).CreateScope(process, logger)) {
               scope.Resolve<IProcessController>().Execute();
               var output = process.Entities.First().Rows;
               Assert.AreEqual("1-2-3-4", output[0]["Joined"]);
            }
         }
      }
   }
}
