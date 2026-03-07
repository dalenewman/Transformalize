using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Json.Autofac;

namespace Test.Integration {
   [TestClass]
   public class ReadWrite {

      [TestMethod]
      public void WriteThenReadJsonArray() {

         var logger = new ConsoleLogger(LogLevel.Info);
         const string writeXml = @"<add name='file' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='json' file='bogus.json' format='json' />
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

         using (var outer = new ConfigurationContainer().CreateScope(writeXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new JsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
            }
         }

         const string readXml = @"<add name='file'>
  <connections>
    <add name='input' provider='json' file='bogus.json' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' page='2' size='10'>
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

         using (var outer = new ConfigurationContainer().CreateScope(readXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();
               Assert.AreEqual(10, output.Length);
               Assert.AreEqual("Delia", output[0][process.Entities[0].Fields[5]]);
            }
         }

      }

      [TestMethod]
      public void WriteThenReadJsonLines() {

         var logger = new ConsoleLogger(LogLevel.Info);

         const string writeXml = @"<add name='file' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='json' file='bogus.jsonl' format='json' />
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

         using (var outer = new ConfigurationContainer().CreateScope(writeXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new JsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
            }
         }

         const string readXml = @"<add name='file'>
  <connections>
    <add name='input' provider='json' file='bogus.jsonl' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' page='2' size='10'>
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

         using (var outer = new ConfigurationContainer().CreateScope(readXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(10, output.Length);
               Assert.AreEqual("Delia", output[0][process.Entities[0].Fields[5]]);

            }
         }

      }

      [TestMethod]
      public async Task WriteThenReadJsonArrayAsync() {
         var logger = new ConsoleLogger(LogLevel.Info);
         var file = CreateUniqueFilePath("json");

         try {
            var writeXml = $@"<add name='file' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='json' file='{EscapeXmlAttribute(file)}' format='json' />
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

            using (var outer = new ConfigurationContainer().CreateScope(writeXml, logger)) {
               var process = outer.Resolve<Process>();
               using (var inner = new Container(new BogusModule(), new JsonProviderModule()).CreateScope(process, logger)) {
                  var controller = inner.Resolve<IProcessController>();
                  await controller.ExecuteAsync();
                  Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
               }
            }

            var readXml = $@"<add name='file'>
  <connections>
    <add name='input' provider='json' file='{EscapeXmlAttribute(file)}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' page='2' size='10'>
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

            using (var outer = new ConfigurationContainer().CreateScope(readXml, logger)) {
               var process = outer.Resolve<Process>();
               using (var inner = new Container(new JsonProviderModule()).CreateScope(process, logger)) {
                  var controller = inner.Resolve<IProcessController>();
                  await controller.ExecuteAsync();
                  var rows = process.Entities.First().Rows.ToArray();

                  Assert.AreEqual(10, rows.Length);
                  Assert.AreEqual("Delia", rows[0]["FirstName"]);
               }
            }
         } finally {
            if (File.Exists(file)) {
               File.Delete(file);
            }
         }
      }

      [TestMethod]
      public async Task WriteThenReadJsonLinesAsync() {
         var logger = new ConsoleLogger(LogLevel.Info);
         var file = CreateUniqueFilePath("jsonl");

         try {
            var writeXml = $@"<add name='file' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='json' file='{EscapeXmlAttribute(file)}' format='json' />
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

            using (var outer = new ConfigurationContainer().CreateScope(writeXml, logger)) {
               var process = outer.Resolve<Process>();
               using (var inner = new Container(new BogusModule(), new JsonProviderModule()).CreateScope(process, logger)) {
                  var controller = inner.Resolve<IProcessController>();
                  await controller.ExecuteAsync();
                  Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
               }
            }

            var readXml = $@"<add name='file'>
  <connections>
    <add name='input' provider='json' file='{EscapeXmlAttribute(file)}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='BogusStar' alias='Contact' page='2' size='10'>
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

            using (var outer = new ConfigurationContainer().CreateScope(readXml, logger)) {
               var process = outer.Resolve<Process>();
               using (var inner = new Container(new JsonProviderModule()).CreateScope(process, logger)) {
                  var controller = inner.Resolve<IProcessController>();
                  await controller.ExecuteAsync();
                  var rows = process.Entities.First().Rows.ToArray();

                  Assert.AreEqual(10, rows.Length);
                  Assert.AreEqual("Delia", rows[0]["FirstName"]);
               }
            }
         } finally {
            if (File.Exists(file)) {
               File.Delete(file);
            }
         }
      }

      private static string CreateUniqueFilePath(string extension) {
         return Path.Combine(Path.GetTempPath(), $"transformalize-json-{Guid.NewGuid():N}.{extension}");
      }

      private static string EscapeXmlAttribute(string value) {
         return SecurityElement.Escape(value) ?? string.Empty;
      }
   }
}
