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

namespace Pipeline.Test {

    [TestFixture]
    public class Validate {

        [Test(Description = "Contains Validator")]
        public void ContainsValidator() {
            var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='streams'>
          <rows>
            <add Field1='11' Field2='12' Field3='13' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='c1' type='bool' t='copy(Field1).contains(1)' />
            <add name='c3' type='bool' t='copy(Field2).contains(1).contains(2)' />
            <add name='c4' type='bool' t='copy(Field2).contains(1).contains(3)' />
          </calculated-fields>
        </add>
      </entities>
    </add>
            ".Replace('\'', '"');

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();
            var process = composer.Process;

            Assert.AreEqual(true, output[0][process.Entities.First().CalculatedFields.First(cf => cf.Name == "c1")]);

            Assert.AreEqual(true, output[0][process.Entities.First().CalculatedFields.First(cf => cf.Name == "c3")]);
            Assert.AreEqual(false, output[0][process.Entities.First().CalculatedFields.First(cf => cf.Name == "c4")]);


        }
    }
}
