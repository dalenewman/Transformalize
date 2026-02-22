using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.Mail.Autofac;

namespace Integration.Test.Mail {

   [TestClass]
   public class UnitTest1 {

      [TestMethod]
      [Ignore]
      public void TestWrite() {

         // get temp credentials from https://mailtrap.io
         const string tempUsername = "11111111111111";
         const string tempPassword = "22222222222222";
         const string fromTo = "you@gmail.com";

         const string xml = $@"<cfg name='Mail'>

    <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='mail' server='sandbox.smtp.mailtrap.io' port='465' user='{tempUsername}' password='{tempPassword}' startTls='true' />
    </connections>

    <entities>
        <add name='Messages'>
            <rows>
                <add From='{fromTo}' To='{fromTo}' Cc='' Bcc='' Subject='Test Message' Body='I am a test message via mailtrap.' />
            </rows>
            <fields>
                <add name='From' />
                <add name='To' />
                <add name='Cc' />
                <add name='Ncc' />
                <add name='Subject' />
                <add name='Body' />
            </fields>
        </add>
    </entities>

</cfg>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new MailModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }
   }
}
