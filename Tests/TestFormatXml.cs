#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using System.Xml.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TestContainer;
using Transformalize.Configuration;

using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestFormatXml {

      [TestMethod]
      public void WtfHowDoesSystemXmlLinqWork() {
         var doc = XDocument.Parse(@"<items>
  <item>
    <Item>Latch</Item>
    <Quantity>1</Quantity>
    <Price>20</Price>
  </item>
  <item>
    <Item>Hasp</Item>
    <Quantity>1</Quantity>
    <Price>20</Price>
  </item>
</items>");

         Assert.AreEqual(2, doc.Descendants("item").Count());
         var items = string.Join(", ", doc.Descendants("item").Select(item => item.Element("Quantity").Value + " " + item.Element("Item").Value));
         var price = doc.Descendants("item").Where(item => !string.IsNullOrEmpty(item.Element("Price").Value)).Sum(item => System.Convert.ToInt32(item.Element("Price").Value) * System.Convert.ToInt32(item.Element("Quantity").Value));

         System.Console.WriteLine(price);
      }

      [TestMethod]
      public void FormatXmlTransform1() {

         const string xml = @"
<add name='TestProcess'>
    <entities>
        <add name='TestData'>
            <rows>
                <add xml='<stuff value=""1""><things><add item=""1""/><add item=""2""/></things></stuff>' />
            </rows>
            <fields>
                <add name='xml' length='128' />
            </fields>
            <calculated-fields>
                <add name='formatted' length='128' t='copy(xml).formatXml()' />
            </calculated-fields>
        </add>
    </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               Assert.AreEqual(@"<stuff value=""1"">
  <things>
    <add item=""1"" />
    <add item=""2"" />
  </things>
</stuff>", output[0][process.Entities.First().CalculatedFields.First()]);
            }
         }




      }



   }



}
