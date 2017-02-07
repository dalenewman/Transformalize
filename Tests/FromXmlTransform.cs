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
    public class FromXmlTransform {

        [TestMethod]
        public void Try1() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add id='1' xml=""&lt;things&gt;&lt;add name='deez'/&gt;&lt;add name='nutz'/&gt;&lt;/things&gt;"" />
            <add id='2' xml=""&lt;things&gt;&lt;add name='got'/&gt;&lt;add name='eeee'/&gt;&lt;/things&gt;"" />
          </rows>
          <fields>
                <add name='id' type='int' />
                <add name='xml' length='max'>
                    <transforms>
                        <add method='fromxml' root='things' xml-mode='all'>
                            <fields>
                                <add name='name' node-type='attribute' />
                            </fields>
                        </add>
                    </transforms>
                </add>
            </fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            Assert.AreEqual(4, output.Length);

            var name = composer.Process.Entities.First().CalculatedFields.First();

            var first = output[0];
            Assert.AreEqual("deez", first[name]);

            var second = output[1];
            Assert.AreEqual("nutz", second[name]);

        }
    }
}
