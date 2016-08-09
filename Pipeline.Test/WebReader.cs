#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Extensions;

namespace Pipeline.Test {

    [TestFixture]
    public class WebReaderTest {

        [Test(Description = "Web Reader Test")]
        [Ignore("requires access to web")]
        public void Test() {

            const string xml = @"
    <add name='TestProcess'>
        <connections>
            <add name='input' provider='web' url='http://www.transformalize.com/Pipeline/Api/Cfg/13' />
        </connections>
      <entities>
        <add name='TestData' >
          <fields>
            <add name='Content' length='max' t='xpath(//fields)' read-inner-xml='false' />
          </fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var content = composer.Process.Entities.First().Fields.First(f=>f.Name == "Content");
            Assert.AreEqual("<fields>", output[0][content].ToString().Left(8));
        }
    }
}
