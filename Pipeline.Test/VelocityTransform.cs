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
    public class VelocityTransform {

        [Test(Description = "Velocity Transform")]
        public void VelocityTransformAdd() {

            const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add Field1='1' Field2='2' Field3='3' />
            </rows>
            <fields>
                <add name='Field1' />
                <add name='Field2' />
                <add name='Field3' />
            </fields>
            <calculated-fields>
                <add name='Add' t='copy(Field1,Field2,Field3).velocity(#set($x = $Field1)$x$Field2$Field3)' />
            </calculated-fields>
        </add>
    </entities>
</add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();

            Assert.AreEqual("123", output[0][composer.Process.Entities.First().CalculatedFields.First()]);
        }

    }
}
