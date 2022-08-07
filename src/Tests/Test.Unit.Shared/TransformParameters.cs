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

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Trace;

namespace Tests {

   [TestClass]
   public class TransformParameters {

      [TestMethod]
      public void A() {

         const string xml = @"
    <add name='TestProcess'>
      <parameters>
        <add name='humanized' value='camelCase' t='replace(lC,l-c).upper()' />
        <add name='replaced' value='TestEnvironment' t='replace(Test,Production).trimEnd(s).append(s)' />
        <add name='copied' t='copy(humanized).lower()' />
      </parameters>
      <entities>
      </entities>
    </add>";

         var logger = new TraceLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger, null, "@()")) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, logger)) {

               inner.Resolve<IProcessController>().Execute();

               Assert.AreEqual("CAMEL-CASE", process.Parameters.First().Value);
               Assert.AreEqual("ProductionEnvironments", process.Parameters.Skip(1).First().Value);
               Assert.AreEqual("camel-case", process.Parameters.Last().Value);


            }
         }



      }
   }
}
