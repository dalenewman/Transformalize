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
using NUnit.Framework;

namespace Transformalize.Test {
    [TestFixture]
    public class SolrIntegration {

        [Test]
        [Ignore("Solr required")]
        public void Test1()
        {
            const string xml = @"<cfg>
    <processes>
        <add name='one' mode='init'>
            <connections>
                <add name='input' provider='solr' server='***change-me***' port='8080' path='solr' core='TimeCard' />
            </connections>
            <entities>
                <add name='TimeCard'>
                    <filter>
                        <add expression='fullnamesearch:christ' />
                    </filter>
                    <fields>
                        <add name='tflkey' alias='key' type='int' />
                        <add name='tflbatchid' alias='batchid' type='int' />
                        <add name='tfldeleted' alias='deleted' type='bool' />
                        <add name='tflhashcode' alias='hashcode' type='int' />
                        <add name='exportcode' />
                        <add name='weekending' />
                        <add name='costcode' />
                        <add name='jobcode' />
                        <add name='acct' />
                        <add name='sub' />
                        <add name='regularhours' type='decimal' />
                        <add name='overtimehours' type='decimal' />
                        <add name='worklocation' />
                        <add name='firstname' />
                        <add name='lastname' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</cfg>";
            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var rows = controller.Read().ToArray();

            Assert.AreEqual(2, rows.Length);
        }
    }
}
