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
    public class MapTransformTester {

        [TestMethod]
        public void MapTransformAdd() {

            const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Map'>
            <items>
                <add from='1' to='One' />
                <add from='2' to='Two' />
                <add from='3' parameter='Field3' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Field1='1' Field3='^' />
                <add Field1='2' Field3='#' />
                <add Field1='3' Field3='$THREE$' />
                <add Field1='4' Field3='@' />
            </rows>
            <fields>
                <add name='Field1' />
                <add name='Field2' />
                <add name='Field3' />
            </fields>
            <calculated-fields>
                <add name='Map' t='copy(Field1).map(map)' default='None' />
            </calculated-fields>
        </add>
    </entities>

</add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var field = composer.Process.Entities.First().CalculatedFields.First();
            var output = controller.Read().ToArray();

            Assert.AreEqual("One", output[0][field]);
            Assert.AreEqual("Two", output[1][field]);
            Assert.AreEqual("$THREE$", output[2][field]);
            Assert.AreEqual("None", output[3][field]);
        }
    }
}
