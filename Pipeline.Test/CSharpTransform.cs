#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Linq;
using NUnit.Framework;
using Pipeline.Contracts;

namespace Pipeline.Test {

    [TestFixture]
    public class CSharpTransform {

        [Test(Description = "C# Transform")]
        public void CSharpTransformAdd() {

            var xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData' pipeline='streams'>
            <rows>
                <add Field1='1' Field2='2' Field3='3' />
            </rows>
            <fields>
                <add name='Field1' />
                <add name='Field2' />
                <add name='Field3' />
            </fields>
            <calculated-fields>
                <add name='Add' t='copy(Field1,Field2,Field3).cs(Field1+Field2+Field3;)' />
                <add name='Format' t='copy(Field1,Field2,Field3).cs(string.Format(&quot;{0}-{1}-{2}&quot;,Field1,Field2,Field3);)' />
            </calculated-fields>
        </add>
    </entities>
</add>";


            var composer = new CompositionRoot();
            var controller = composer.Compose(xml, LogLevel.Debug);

            var output = controller.Read().ToArray();

            Assert.AreEqual("123", output[0][composer.Process.Entities.First().CalculatedFields.First()]);
            Assert.AreEqual("1-2-3", output[0][composer.Process.Entities.First().CalculatedFields.Last()]);


        }

    }
}
