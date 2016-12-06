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
    public class TrimThenStartsWith {

        [Test(Description = "TrimThenStartsWith Combination")]
        public void TryIt() {
            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add input=' Wave 1' />
            <add input='Flight 1' />
          </rows>
          <fields>
            <add name='input' t='trim()' />
          </fields>
          <calculated-fields>
            <add name='output' type='bool' t='copy(input).startsWith(W)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            controller.Execute();

            var process = composer.Process;
            var output = process.Entities.First().Rows;

            Assert.AreEqual(true, output[0]["output"]);
            Assert.AreEqual(false, output[1]["output"]);

        }

    }
}
