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

namespace Tests {

    [TestClass]
    public class JoinTwoInMemoryTables {

        [TestMethod]
        public void Try1() {

            const string xml = @"
    <add name='name'>
      <entities>
        <add name='Employee'>
          <rows>
            <add EmpId='1' FirstName='John' LastName='Doe' JobId='1' />
            <add EmpId='2' FirstName='Jane' LastName='Doe' JobId='2' />
          </rows>
          <fields>
            <add name='EmpId' type='int' primary-key='true' />
            <add name='FirstName' />
            <add name='LastName' />
            <add name='JobId' type='int' />
          </fields>
          <calculated-fields>
            <add name='FullName' t='copy(FirstName,LastName).format({0} {1})' />
          </calculated-fields>
        </add>
        <add name='Job'>
            <rows>
                <add JobId='1' Title='Carpenter' />
                <add JobId='2' Title='Seamstress' />
            </rows>
            <fields>
                <add name='JobId' type='int' primary-key='true' />
                <add name='Title' />
            </fields>
        </add>
      </entities>
      <relationships>
        <add left-entity='Employee' left-field='JobId' right-entity='Job' right-field='JobId' />
      </relationships>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            Assert.AreEqual(2, output.Length);

        }
    }
}
