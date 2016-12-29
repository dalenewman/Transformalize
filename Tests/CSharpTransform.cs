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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Contracts;

namespace Tests {

    [TestClass]
    public class CSharpTransform {

        [TestMethod]
        public void CSharpTransformAdd() {

            // Problem is I am compiling code in advance, 
            // but not remembering that there are actually two processes that run, 
            // the entity processes,  and the calculated field process

            const string xml = @"
<add name='TestProcess' mode='default'>
    <entities>
        <add name='TestData'>
            <rows>
                <add Field1='1' Field2='2' Field3='3' />
                <add Field1='4' Field2='5' Field3='6' />
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
            var process = composer.Process;
            controller.Execute();
            controller.Dispose();

            var entity = process.Entities.First();

            var row1 = entity.Rows[0];
            var row2 = entity.Rows[1];

            Assert.AreEqual("123",   row1["Add"]);
            Assert.AreEqual("1-2-3", row1["Format"]);

            Assert.AreEqual("456", row2["Add"]);
            Assert.AreEqual("4-5-6", row2["Format"]);

        }

    }
}
