using System;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.Json.Autofac;

namespace Test.Integration {
   [TestClass]
   public class ReadColors {

      [TestMethod]
      public void Read() {
         const string xml = @"<add name='file'>
  <connections>
    <add name='input' provider='json' file='files/nfl-team-colors.json' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Colors'>
      <fields>
        <add name='name' />
        <add name='colors' length='max' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(32, rows.Count);

            }
         }
      }
   }
}
