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
    public class TestXmlDecode {

        [TestMethod]
        public void XmlDecodeWorks() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='stranger &amp; things in the upside down' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).xmlDecode()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var cf = composer.Process.Entities.First().CalculatedFields.ToArray();
            Assert.AreEqual("stranger & things in the upside down", output[0][cf[0]]);

        }
    }
}
