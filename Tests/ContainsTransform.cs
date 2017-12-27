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
    public class ContainsTransform {

        [TestMethod]
        public void Contains() {

            const string xml = @"
    <add name='TestProcess'>
      <maps>
        <add name='Map'>
            <items>
                <add from='true' to='It is True' />
                <add from='false' to='It is False' />
                <add from='*' to='It is Unknown' />
            </items>
        </add>
      </maps>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Tags='Tag1 Tag2' />
            <add Tags='Tag2 Tag3' />
          </rows>
          <fields>
            <add name='Tags' />
          </fields>
          <calculated-fields>
            <add name='IsTag1' type='bool' t='copy(Tags).contains(Tag1)' />
            <add name='IsTag2' type='bool' t='copy(Tags).contains(Tag2)' />
            <add name='IsTag3' type='bool' t='copy(Tags).contains(Tag3)' />
            <add name='IsTag3Map' t='copy(Tags).contains(Tag3).map(Map)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var cf = composer.Process.Entities.First().CalculatedFields.ToArray();
            Assert.AreEqual(true, output[0][cf[0]]);
            Assert.AreEqual(true, output[0][cf[1]]);
            Assert.AreEqual(false, output[0][cf[2]]);
            Assert.AreEqual("It is False", output[0][cf[3]]);

            Assert.AreEqual(false, output[1][cf[0]]);
            Assert.AreEqual(true, output[1][cf[1]]);
            Assert.AreEqual(true, output[1][cf[2]]);
            Assert.AreEqual("It is True", output[1][cf[3]]);
        }
    }
}
