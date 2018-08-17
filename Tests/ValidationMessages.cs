#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
    public class ValidationMessages {

        [TestMethod]
        public void TryIt() {

            const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          
          <rows>
            <add Field1='11' Field2='12' Field3='13' />
            <add Field1='xx' Field2='' Field3='100.8' />
          </rows>

          <fields>
            <add name='Field1' v='contains(1)' message-field='Message' />
            <add name='Field2' v='required().is(int)' message-field='Message' />
            <add name='Field3' v='matches(^[0-9/.]{5}$)' message-field='Message' help='Field3 must be a 5 digit number (including decimal pt.)' />
          </fields>
          <calculated-fields>
            <add name='Message' length='1000' default='' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var output = controller.Read().ToArray();
            var process = composer.Process;

            var message = process.Entities.First().CalculatedFields.First(cf => cf.Name == "Message");
            var valid = process.Entities.First().CalculatedFields.First(cf => cf.Name == "TestDataValid");
            Assert.IsFalse((bool)output[0][valid]);
            Assert.AreEqual("Field3 must be a 5 digit number (including decimal pt.)", output[0][message].ToString().Replace("|"," ").TrimEnd());
            Assert.AreEqual("Field1 must contain 1. Field2 is required. Field2's value is incompatable with the int data type.", output[1][message].ToString().Replace("|", " ").TrimEnd());
            
        }


    }
}
