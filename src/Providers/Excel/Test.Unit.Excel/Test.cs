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

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Excel.Autofac;

namespace Test {

   [TestClass]
   public class Test {

      [TestMethod]
      public void Write() {
         const string xml = @"<add name='Excel' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='excel' file='bogus.xlsx' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new ExcelModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
            }
         }
      }

      [TestMethod]
      public void Read() {
         const string xml = @"<add name='Excel'>
  <connections>
    <add name='input' provider='excel' file='bogus.xlsx' start='2' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' page='1' size='10'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' />
        <add name='Reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new ExcelModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(10, rows.Count);


            }
         }
      }

      [TestMethod]
      public void ReadSchema() {
         const string xml = @"<add name='Excel'>
  <connections>
    <add name='input' provider='excel' file='bogus.xlsx'>
        <types>
            <add type='byte' />
            <add type='int' />
            <add type='string' />
        </types>
    </add>
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' />
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();

            using (var inner = new Container(new ExcelModule()).CreateScope(process, logger)) {

               var schemaReader = inner.ResolveNamed<ISchemaReader>(process.Connections.First().Key);
               var schema = schemaReader.Read();

               var entity = schema.Entities.First();

               Assert.AreEqual(5, entity.Fields.Count);
               Assert.AreEqual("int", entity.Fields[0].Type);
               Assert.AreEqual("string", entity.Fields[1].Type);
               Assert.AreEqual("string", entity.Fields[2].Type);
               Assert.AreEqual("byte", entity.Fields[3].Type);
               Assert.AreEqual("int", entity.Fields[4].Type);


            }
         }
      }

   }
}
