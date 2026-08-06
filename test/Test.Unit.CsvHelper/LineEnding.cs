using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.CsvHelper.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   public class LineEnding {

      [TestMethod]
      public void WriteCrlf() {
         var text = WriteWithLineEnding("crlf", "files/bogus-crlf.csv");
         Assert.AreEqual(11, text.Count(c => c == '\n'), "header + 10 rows");
         Assert.AreEqual(11, text.Split(new[] { "\r\n" }, System.StringSplitOptions.None).Length - 1, "every line ends with crlf");
      }

      [TestMethod]
      public void WriteLf() {
         var text = WriteWithLineEnding("lf", "files/bogus-lf.csv");
         Assert.AreEqual(11, text.Count(c => c == '\n'), "header + 10 rows");
         Assert.IsFalse(text.Contains('\r'), "no carriage returns");
      }

      private static string WriteWithLineEnding(string lineEnding, string file) {

         string xml = $@"<add name='file' read-only='true'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='file' delimiter=',' line-ending='{lineEnding}' file='{file}' />
  </connections>
  <entities>
    <add name='Contact' size='10'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

         return File.ReadAllText(file);
      }
   }
}
