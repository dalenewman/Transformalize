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
                <add text='sample text' date='{date}' metric='1k' roman='XXXVIII' number='1' />
                <add text='Sample_Texts' date='{date}' metric='16 twerps' roman='P' number='22' />
                <add text='$ample-text' date='{now}' metric='1000 μ' roman='CC' number='3000' />
            </rows>
            <fields>
                <add name='text' />
                <add name='date' type='datetime' />
                <add name='metric' />
                <add name='roman' />
                <add name='number' type='int' />
            </fields>
            <calculated-fields>
                <add name='Humanized' t='copy(text).humanize()' />
                <add name='Dehumanized' t='copy(text).dehumanize()' />
                <add name='HumanizedDate' t='copy(date).addminutes(30.1).humanize()' />
                <add name='Camelized' t='copy(text).camelize()' />
                <add name='FromMetric' type='double' t='copy(metric).fromMetric()' />
                <add name='FromRoman' type='double' t='copy(roman).fromRoman()' />
                <add name='Hyphenated' t='copy(text).hyphenate()' />
                <add name='Ordinalized' t='copy(number).ordinalize()' />
                <add name='Pascalized' t='copy(text).pascalize()' />
                <add name='Pluralized' t='copy(text).pluralize()' />
                <add name='Singularized' t='copy(text).singularize()' />
                <add name='Titleized' t='copy(text).titleize()' />
                <add name='ToMetric' t='copy(number).tometric()' />
                <add name='ToOrdinalWorded' t='copy(number).toOrdinalWords()' />
                <add name='ToRoman' t='copy(number).toRoman()' />
                <add name='ToWords' t='copy(number).toWords()' />
                <add name='Underscored' t='copy(text).underscore()' />
            </calculated-fields>
        </add>
    </entities>

</add>";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);

            var cf = composer.Process.Entities.First().CalculatedFields;

            var humanized = cf.First();
            var dehumanized = cf.First(f=>f.Name == "Dehumanized");
            var humanizedDate = cf.First(f => f.Name == "HumanizedDate");
            var camelized = cf.First(f => f.Name == "Camelized");
            var fromMetric = cf.First(f => f.Name == "FromMetric");
            var fromRoman = cf.First(f => f.Name == "FromRoman");
            var hyphenated = cf.First(f => f.Name == "Hyphenated");
            var ordinalized = cf.First(f => f.Name == "Ordinalized");
            var pascalized = cf.First(f => f.Name == "Pascalized");
            var pluralized = cf.First(f => f.Name == "Pluralized");
            var singularized = cf.First(f => f.Name == "Singularized");
            var titleized = cf.First(f => f.Name == "Titleized");
            var toMetric = cf.First(f => f.Name == "ToMetric");
            var toOrdinalWorded = cf.First(f => f.Name == "ToOrdinalWorded");
            var toRoman = cf.First(f => f.Name == "ToRoman");
            var toWords = cf.First(f => f.Name == "ToWords");
            var underscored = cf.First(f => f.Name == "Underscored");

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

            Assert.AreEqual("sample text", rows[0][camelized]);
            Assert.AreEqual("sampleTexts", rows[1][camelized]);
            Assert.AreEqual("$ample-text", rows[2][camelized]);

            Assert.AreEqual(1000d, rows[0][fromMetric]);
            Assert.AreEqual(16d, rows[1][fromMetric]);
            Assert.AreEqual(0.001d, rows[2][fromMetric]);

            Assert.AreEqual(38.0d, rows[0][fromRoman]);
            Assert.AreEqual(0d, rows[1][fromRoman]);
            Assert.AreEqual(200d, rows[2][fromRoman]);

            Assert.AreEqual("sample text", rows[0][hyphenated]);
            Assert.AreEqual("Sample-Texts", rows[1][hyphenated]);
            Assert.AreEqual("$ample-text", rows[2][hyphenated]);

            Assert.AreEqual("1st", rows[0][ordinalized]);
            Assert.AreEqual("22nd", rows[1][ordinalized]);
            Assert.AreEqual("3000th", rows[2][ordinalized]);

            Assert.AreEqual("Sample text", rows[0][pascalized]);
            Assert.AreEqual("SampleTexts", rows[1][pascalized]);
            Assert.AreEqual("$ample-text", rows[2][pascalized]);

            Assert.AreEqual("sample texts", rows[0][pluralized]);
            Assert.AreEqual("Sample_Texts", rows[1][pluralized]);
            Assert.AreEqual("$ample-texts", rows[2][pluralized]);

            Assert.AreEqual("sample text", rows[0][singularized]);
            Assert.AreEqual("Sample_Text", rows[1][singularized]);
            Assert.AreEqual("$ample-text", rows[2][singularized]);

            Assert.AreEqual("Sample Text", rows[0][titleized]);
            Assert.AreEqual("Sample Texts", rows[1][titleized]);
            Assert.AreEqual("$ample Text", rows[2][titleized]);

            Assert.AreEqual("1", rows[0][toMetric]);
            Assert.AreEqual("22", rows[1][toMetric]);
            Assert.AreEqual("3k", rows[2][toMetric]);

            Assert.AreEqual("first", rows[0][toOrdinalWorded]);
            Assert.AreEqual("twenty-second", rows[1][toOrdinalWorded]);
            Assert.AreEqual("three thousandth", rows[2][toOrdinalWorded]);

            Assert.AreEqual("I", rows[0][toRoman]);
            Assert.AreEqual("XXII", rows[1][toRoman]);
            Assert.AreEqual("MMM", rows[2][toRoman]);

            Assert.AreEqual("one", rows[0][toWords]);
            Assert.AreEqual("twenty-two", rows[1][toWords]);
            Assert.AreEqual("three thousand", rows[2][toWords]);

            Assert.AreEqual("sample_text", rows[0][underscored]);
            Assert.AreEqual("sample_texts", rows[1][underscored]);
            Assert.AreEqual("$ample_text", rows[2][underscored]);

            /*
            sample text'
            Sample_Texts
            $ample-text'
            */
            }
        }
    }
