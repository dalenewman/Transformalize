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
    public class TrimThenStartsWith {

        [TestMethod]
        public void TryIt() {
            const string xml = @"
    <add name='TestProcess'>
        <connections>
            <add name='input' provider='internal' />
            <add name='output' provider='internal' />
        </connections>
      <entities>
        <add name='TestData'>
          <rows>
            <add input=' Wave 1' />
            <add input='Flight 1' />
          </rows>
          <fields>
            <add name='input' t='trim()' />
          </fields>
          <calculated-fields>
            <add name='output' type='bool' t='copy(input).startsWith(W)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";


         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               Assert.AreEqual(true, output[0][process.GetField("output")]);
               Assert.AreEqual(false, output[1][process.GetField("output")]);
            }
         }


      }

    }
}
