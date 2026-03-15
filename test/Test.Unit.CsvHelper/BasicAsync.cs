using Autofac;
using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.CsvHelper.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   public class BasicAsync {


      /// <summary>
      /// Presently the provider only works reliably when synchronous is true.
      /// </summary>
      [TestMethod]
      public async Task Write() {

         const string writeXml = @"<add name='file' read-only='true'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='file' delimiter=',' file='files/bogus-test-async.csv' />
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

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(writeXml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
               Assert.AreEqual((uint)1000, process.Entities.First().Inserts, "wrote 1000 rows to bogus-test.csv");
            }
         }

         const string readXml = @"<add name='file' read-only='true'>
  <connections>
    <add name='input' provider='file' delimiter=',' file='files/bogus-test-async.csv' />
  </connections>
  <entities>
    <add name='Contact'>
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
            using (var inner = new Container(new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
               Assert.AreEqual(1000, process.Entities.First().Hits, "read 1000 rows from bogus-test.csv");
            }
         }


      }

      [TestMethod]
      public async Task WriteWithSomeLineBreaks() {
         const string xml = @"<add name='file' mode='init' read-only='true'>
  <connections>
    <add name='input' provider='internal' />
    <add name='output' provider='file' delimiter=',' file='files/data-with-line-breaks-and-commas-test-async.csv' text-qualifier='""' />
  </connections>
  <entities>
    <add name='Contact'>
      <rows>
        <add Identity='1' FirstName='Dale' LastName='Newman' Stars='1' Reviewers='1' />
        <add Identity='2' FirstName='Dale
 Jr' LastName='Newman,s' Stars='2' Reviewers='2' />
      </rows>
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
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, new ConsoleLogger(LogLevel.Info))) {
               var controller = inner.Resolve<IProcessController>();
               await controller.ExecuteAsync();
            }
         }
      }

      [TestMethod]
      public async Task Read() {

         const string xml = @"<add name='file' read-only='true'>
  <connections>
    <add name='input' provider='file' delimiter=',' file='files/bogus.csv' />
  </connections>
  <entities>
    <add name='Contact' page='1' size='20'>
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

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               var rows = (await controller.ReadAsync()).ToArray();
               Assert.AreEqual(20, rows.Length);
               var row = rows[3].ToFriendlyDictionary(process.Entities[0].Fields.ToArray());
               Assert.AreEqual("4", row["Identity"].ToString());
               Assert.AreEqual("Caleb", row["FirstName"].ToString());
               Assert.AreEqual("Hane", row["LastName"].ToString());
               Assert.AreEqual("4", row["Stars"].ToString());
               Assert.AreEqual("78", row["Reviewers"].ToString());
            }
         }

      }

      [TestMethod]
      public async Task ThrowOnBadData() {

         const string xml = @"<add name='file' mode='init' read-only='true'>
  <connections>
    <add name='input' provider='file' delimiter=',' file='files/bad-data.csv' />
  </connections>
  <entities>
    <add name='BadData'>
      <fields>
        <add name='field1' />
        <add name='field2' />
        <add name='field3' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
                var controller = inner.Resolve<IProcessController>();
                await Assert.ThrowsExactlyAsync<BadDataException>(() => controller.ExecuteAsync());
            }
         }

      }

      [TestMethod]
      public async Task IgnoreBadData() {

         const string xml = @"<add name='file' mode='init' read-only='true'>
  <connections>
    <add name='input' provider='file' delimiter=',' file='files/bad-data.csv' error-mode='IgnoreAndContinue' />
  </connections>
  <entities>
    <add name='BadData'>
      <fields>
        <add name='field1' />
        <add name='field2' />
        <add name='field3' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               var rows = await controller.ReadAsync();
               Assert.AreEqual(3, rows.Count());
            }
         }

      }

   }
}
