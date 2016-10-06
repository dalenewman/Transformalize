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
    public class MathTransforms {

        [Test(Description = "Round Transformation")]
        public void DoMath() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='10.6954' Field2='129.992' />
          </rows>
          <fields>
            <add name='Field1' type='double' />
            <add name='Field2' type='decimal' />
          </fields>
          <calculated-fields>
            <add name='Ceiling' type='double' t='copy(Field1).ceiling()' />
            <add name='Floor' type='double' t='copy(Field1).floor()' />
            <add name='Round' type='decimal' t='copy(Field2).round(1)' />
            <add name='Abs' type='decimal' t='copy(Field2).abs()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var cf = composer.Process.Entities.First().CalculatedFields.ToArray();
            var row = output.First();
            Assert.AreEqual(11, row[cf[0]]);
            Assert.AreEqual(10, row[cf[1]]);
            Assert.AreEqual(130.00d, row[cf[2]]);
            Assert.AreEqual(129.992, row[cf[3]]);
        }
    }
}
