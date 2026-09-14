using Transformalize.Extensions;
using System.Threading.Tasks;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;

namespace Tests.Unit {
   [TestClass]
   [DoNotParallelize]
   public class UnitTest1 {
      [TestMethod]
      [DataRow(false)]
      [DataRow(true)]
      public async Task Read(bool streaming) {
         const string xml = @"<add name='Bogus'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Bogus' alias='Contact' page='1' size='10'>
      <order>
        <add field='Identity' />
      </order>
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
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               if (streaming) await controller.ExecuteStreamAsync(); else controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(10, rows.Count);

               Assert.AreEqual("Justin", rows[0]["FirstName"]);
               Assert.AreEqual("Konopelski", rows[0]["LastName"]);

            }
         }
      }
   }
}
