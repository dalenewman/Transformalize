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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {

    [TestClass]
    public class FormatXmlTransform {

        [TestMethod]
        public void FormatXmlTransform1() {

            const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add xml='<stuff value=""1""><things><add item=""1""/><add item=""2""/></things></stuff>' />
            </rows>
            <fields>
                <add name='xml' length='128' />
            </fields>
            <calculated-fields>
                <add name='formatted' length='128' t='copy(xml).formatXml()' />
            </calculated-fields>
        </add>
    </entities>
</add>";
            
            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();

            Assert.AreEqual(@"<stuff value=""1"">
  <things>
    <add item=""1"" />
    <add item=""2"" />
  </things>
</stuff>", output[0][composer.Process.Entities.First().CalculatedFields.First()]);


        }

    }
}
