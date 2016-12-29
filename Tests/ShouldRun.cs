#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
    public class ShouldRun {

        [TestMethod]
        public void ConditionalFormat() {

            const string xml = @"
<add name='Test Conditional Format'>
    <entities>
        <add name='People'>
            <rows>
                <add First='Dale' Last='Newman' Gender='Male' />
                <add First='Owen' Last='Watson' Gender='Male' />
                <add First='Jeremy' Last='Christ' Gender='Male' />
                <add First='Kelly' Last='Jones' Gender='Female' />
            </rows>
            <fields>
                <add name='First' />
                <add name='Last' />
                <add name='Gender' />
            </fields>
            <calculated-fields>
                <add name='FullName' default='None'>
                    <transforms>
                        <add method='format' 
                            run-field='Gender'
                            run-operator='Equal'
                            run-value='Male'
                            format='{0} {1}'>
                            <parameters>
                                <add field='First' />
                                <add field='Last' />
                            </parameters>
                        </add>
                    </transforms>
                </add>
            </calculated-fields>
        </add>
    </entities>
</add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();
            var full = composer.Process.Entities.First().CalculatedFields[0];

            Assert.AreEqual(4, output.Count());
            Assert.AreEqual("Dale Newman", output[0][full]);
            Assert.AreEqual("Owen Watson", output[1][full]);
            Assert.AreEqual("Jeremy Christ", output[2][full]);
            Assert.AreEqual("None", output[3][full]);

        }
    }
}
