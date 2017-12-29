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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {

    [TestClass]
    public class Validate {

        [TestMethod]
        public void EqualsValidator() {
            var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='11' Field2='12' Field3='13' />
            <add Field1='11' Field2='11' Field3='11' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='AreEqual' type='bool' t='copy(Field1,Field2,Field2).equals()' />
          </calculated-fields>
        </add>
      </entities>
    </add>
            ".Replace('\'', '"');

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();
            var process = composer.Process;

            Assert.AreEqual(false, output[0][process.Entities.First().CalculatedFields.First(cf => cf.Name == "AreEqual")]);
            Assert.AreEqual(true, output[1][process.Entities.First().CalculatedFields.First(cf => cf.Name == "AreEqual")]);
        }

    }
}
