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
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {

    [TestClass]
    public class SubstringTest {

        [TestMethod]
        public void TrySubstring() {
            const string xml = @"
    <add name='TestDistinct'>
      <entities>
        <add name='Dates'>
          <rows>
            <add name='dalenewman' />
          </rows>
          <fields>
            <add name='name' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(name).substring(4)' />
            <add name='t2' t='copy(name).substring(4,3)' />
            <add name='t3' t='copy(name).substring(4,6)' />
            <add name='t4' t='copy(name).substring(4,7)' />
            <add name='t5' t='copy(name).substring(10)' />
            <add name='t6' t='copy(name).substring(11,1)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            controller.Execute();

            var process = composer.Process;
            var row = process.Entities.First().Rows[0];

            Assert.AreEqual("newman", row["t1"], "should skip dale and return newman (the remaining part of the string)");
            Assert.AreEqual("new", row["t2"], "should skip dale and return new (3 chars)");
            Assert.AreEqual("newman", row["t3"], "should skip dale and return newman (6 characters)");
            Assert.AreEqual("newman", row["t4"], "should skip dale and return newman (7 characters, but only 6 available)");
            Assert.AreEqual("", row["t5"],"should skip dalenewman and return blank");
            Assert.AreEqual("", row["t6"],"should skip more than dalenewman and return blank");
        }

    }
}
