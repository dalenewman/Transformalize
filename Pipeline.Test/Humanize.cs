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

using System;
using System.Linq;
using NUnit.Framework;

namespace Pipeline.Test {

    [TestFixture]
    public class HumanizeTester {

        [Test(Description = "Humanize Transform")]
        public void HumanizeTranformTests()
        {

            var date = DateTime.UtcNow.AddMonths(2).ToString("O");
            var now = DateTime.UtcNow.ToString("O");

            var xml = $@"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add text='sampleText' date='{date}' />
                <add text='Sample_Texts' date='{date}' />
                <add text='$ample-text' date='{now}' />
            </rows>
            <fields>
                <add name='text' />
                <add name='date' type='datetime' default='now()' />
            </fields>
            <calculated-fields>
                <add name='Humanized' t='copy(text).humanize()' />
                <add name='Dehumanized' t='copy(text).dehumanize()' />
                <add name='HumanizedDate' t='copy(date).addminutes(30.1).humanize()' />
            </calculated-fields>
        </add>
    </entities>

</add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var cf = composer.Process.Entities.First().CalculatedFields;
            var humanized = cf.First();
            var dehumanized = cf.FirstOrDefault(f=>f.Name == "Dehumanized");
            var humanizedDate = cf.FirstOrDefault(f => f.Name == "HumanizedDate");
            var rows = controller.Read().ToArray();

            Assert.AreEqual("Sample text", rows[0][humanized]);
            Assert.AreEqual("Sample Texts", rows[1][humanized]);
            Assert.AreEqual("$ample text", rows[2][humanized]);

            Assert.AreEqual("SampleText", rows[0][dehumanized]);
            Assert.AreEqual("SampleTexts", rows[1][dehumanized]);
            Assert.AreEqual("$ampleText", rows[2][dehumanized]);

            Assert.AreEqual("2 months from now", rows[0][humanizedDate]);
            Assert.AreEqual("2 months from now", rows[1][humanizedDate]);
            Assert.AreEqual("30 minutes from now", rows[2][humanizedDate]);

        }
    }
}
