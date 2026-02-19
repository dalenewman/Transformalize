using Autofac;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.SqlServer.Autofac;

namespace Test.Unit.SqlServer {

   [TestClass]
   public class TestTransformThenValidate {

      [TestMethod]
      public void Test() {

         var cfg = $@"<cfg name='TestTransformThenValidate' mode='@[Mode]'>
   <parameters>
      <add name='Mode' value='default' />
   </parameters>
   <connections>
      <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Junk' user='{Tester.User}' password='{Tester.Pw}' />
   </connections>
   <entities>
      <add name='TestData' alias='Data'>
         <fields>
            <add name='Id' type='int' primary-key='true' />
            <add name='Field1' t='replace(\,,)' v='is(int)' valid-field='Valid' message-field='Message' />
         </fields>
         <calculated-fields>
            <add name='Valid' type='bool' default='true' />
            <add name='Message' length='255' />
         </calculated-fields>
      </add>
   </entities>
</cfg>
";
         var logger = new DebugLogger(LogLevel.Debug);

         using (var outer = new ConfigurationContainer().CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();

            if (process.Errors().Any()) {
               foreach (var error in process.Errors()) {
                  System.Diagnostics.Debug.Write(error);
               }
            } else {
               using (var inner = new Container(new AdoProviderModule(), new SqlServerModule(process)).CreateScope(process, logger)) {
                  var connectionFactory = inner.ResolveNamed<IConnectionFactory>("TestTransformThenValidateinput");

                  using (var cn = connectionFactory.GetConnection("Test")) {
                     // set up test data
                     try {
                        cn.Execute("DROP TABLE TestData;");
                     } catch (System.Exception) {

                     }
                     cn.Execute("CREATE TABLE TestData(Id INT NOT NULL PRIMARY KEY,Field1 NVARCHAR(60));");
                     cn.Execute("INSERT INTO TestData(Id,Field1) VALUES(@Id,@Field1)", new { Id = 1, Field1 = "1,001" });
                     cn.Execute("INSERT INTO TestData(Id,Field1) VALUES(@Id,@Field1)", new { Id = 2, Field1 = "2.002" });
                  }

                  var controller = inner.Resolve<IProcessController>();
                  var rows = controller.Read().ToArray();

                  Assert.AreEqual(rows.Length, 2);
                  Assert.IsTrue((bool)rows[0][process.Entities.First().GetField("Valid")]);
                  Assert.IsFalse((bool)rows[1][process.Entities.First().GetField("Valid")]);
               }
            }

         }

      }
   }
}
