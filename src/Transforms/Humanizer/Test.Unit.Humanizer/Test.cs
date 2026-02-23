using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Humanizer.Autofac;

namespace UnitTests {
   [TestClass]
   public class Test {

      [TestMethod]
      public void BasicTests() {

         var logger = new ConsoleLogger(LogLevel.Debug);
         var date = DateTime.UtcNow.AddMonths(2).AddDays(1).ToString("O");
         var now = DateTime.Now.ToString("O");
         var now30 = DateTime.Now.AddMinutes(30.1);

         var xml = $@"
<add name='TestProcess' read-only='false'>
    <entities>
        <add name='TestData'>
            <rows>
                <add text='sample text' date='{date}' d2='{date}' metric='1k' roman='XXXVIII' number='1' />
                <add text='Sample_Texts' date='{date}' d2='{date}' metric='16 twerps' roman='P' number='22' />
                <add text='$ample-text' date='{now}' d2='{now30}' metric='1000 μ' roman='CC' number='3000' />
            </rows>
            <fields>
                <add name='text' />
                <add name='date' type='datetime' />
                <add name='d2' type='datetime' />
                <add name='metric' />
                <add name='roman' />
                <add name='number' type='int' />
            </fields>
            <calculated-fields>
                <add name='Humanized' t='copy(text).humanize()' />
                <add name='Dehumanized' t='copy(text).dehumanize()' />
                <add name='HumanizedDate' t='copy(d2).humanize()' />
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
         using (var outer = new ConfigurationContainer(new HumanizeModule()).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new HumanizeModule()).CreateScope(process, logger)) {

               var cf = process.Entities.First().CalculatedFields;

               var humanized = cf.First();
               var dehumanized = cf.First(f => f.Name == "Dehumanized");
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

               var controller = inner.Resolve<IProcessController>();
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

               Assert.AreEqual("sampleText", rows[0][camelized]);
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

               Assert.AreEqual("SampleText", rows[0][pascalized]);
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
               Assert.AreEqual("$Ample Text", rows[2][titleized]);

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

            }
         }

      }
   }
}
