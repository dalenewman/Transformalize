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
    public class ReadOnlyNoPrimaryKey {

        [TestMethod]
        public void ItWorks() {
            const string xml = @"
    <add name='Test' read-only='true'>
      <entities>
        <add name='Data'>
          <rows>
            <add Date='2017-01-01 9 AM' Number='1' />
            <add Date='2016-06-07 12:31:22' Number='2' />
          </rows>
          <fields>
            <add name='Date' type='datetime' />
            <add name='Number' type='short' />
          </fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            controller.Execute();

            var process = composer.Process;
            var row1 = process.Entities.First().Rows[0];
            //var row2 = process.Entities.First().Rows[1];

            Assert.AreEqual(new DateTime(2017, 1, 1, 9, 0, 0), row1["Date"]);
            Assert.AreEqual((short)1, row1["Number"]);

        }

    }
}
