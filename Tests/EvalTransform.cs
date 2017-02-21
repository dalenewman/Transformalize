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
    public class EvalTransform {

        [TestMethod]
        public void Eval() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='1' Field2='3' Field3='stuff' />
            <add Field1='5' Field2='4' Field3='more stuff'/>
          </rows>
          <fields>
            <add name='Field1' type='int' />
            <add name='Field2' type='int' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='First' type='int' t='copy(Field1,Field2).eval(Field1+Field2)' />
            <add name='Last' t='copy(Field1,Field2,Field3).eval(Field3 == ""stuff"" ? Field1 : Field2)' />
            <add name='Bad' t='eval(Field1+Field2+Field3)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var first = composer.Process.Entities.First().CalculatedFields.First();
            Assert.AreEqual(4, output[0][first]);
            Assert.AreEqual(9, output[1][first]);

            var last = composer.Process.Entities.First().CalculatedFields.First(f=>f.Name == "Last");
            Assert.AreEqual("1", output[0][last]);
            Assert.AreEqual("4", output[1][last]);

            var bad = composer.Process.Entities.First().CalculatedFields.First(f => f.Name == "Bad");
            Assert.AreEqual("4stuff", output[0][bad]);
            Assert.AreEqual("9more stuff", output[1][bad]);

        }
    }
}
