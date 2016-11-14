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
using NUnit.Framework;

namespace Pipeline.Test {

    [TestFixture]
    public class WebReaderTest {

        [Test(Description = "Web Reader Test")]
        [Ignore("requires access to web and an Active MQ server running")]
        public void Test() {

            const string xml = @"
    <add name='TestProcess'>
        <connections>
            <add name='input' provider='web' url='' user='' password='' />
            <add name='output' provider='log' />
        </connections>
      <entities>
        <add name='TestData' >
          <fields>
            <add name='Content' length='max' output='false'>
                <transforms>
                    <add method='fromXml' xml-mode='all'>
                        <fields>
                            <add name='name' node-type='attribute' />
                            <add name='consumerCount' type='int' node-type='attribute' />
                        </fields>
                    </add>
                </transforms>
            </add>
          </fields>
          <calculated-fields>
            <add name='pass' type='bool' t='copy(pass,consumerCount).js(consumerCount &gt; 0).filter(false)' />
            <add name='message' length='128' t='copy(name,consumerCount).format({0} queue has {1} consumers!)' />
            <add name='level' default='error' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            controller.Execute();

        }
    }
}
