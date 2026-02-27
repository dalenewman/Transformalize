using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Providers.Console;

namespace Test.Unit {

   [TestClass]
   public class TestWriteFacetQueries {

      private const string TestCfg1 = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='*' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' value='@[f2]' type='facet' size='0' />
               <add expression='1=2' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
   </cfg>
";

      [TestMethod]
      public void IgnoringAsterisk() {
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(TestCfg1, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using(var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var filter = context.Process.Entities[0].Filter[0];
               var actual = context.SqlSelectFacetFromInput(filter, new NullConnectionFactory() { AdoProvider = AdoProvider.SqlServer, SupportsLimit = false });
               Assert.AreEqual("SELECT CAST(f2 AS NVARCHAR(128)) + ' (' + CAST(COUNT(*) AS NVARCHAR(32)) + ')' AS From, f2 AS To FROM Fact WHERE (1=2) GROUP BY f2 ORDER BY f2 ASC", actual);
            }

         }

      }

      [TestMethod]
      public void CreateWithTop() {
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(TestCfg1, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            process.Entities.First().Filter.First().Size = 26;

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var filter = context.Process.Entities[0].Filter[0];
               filter.Order = "desc";
               var actual = context.SqlSelectFacetFromInput(filter, new NullConnectionFactory() { AdoProvider = AdoProvider.SqlServer, SupportsLimit = false });
               Assert.AreEqual("SELECT TOP 26 CAST(f2 AS NVARCHAR(128)) + ' (' + CAST(COUNT(*) AS NVARCHAR(32)) + ')' AS From, f2 AS To FROM Fact WHERE (1=2) GROUP BY f2 ORDER BY f2 DESC", actual);
            }

         }
      }

      [TestMethod]
      public void CreateForMySqlWithLimit() {
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(TestCfg1, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            process.Entities.First().Filter.First().Size = 29;

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var filter = context.Process.Entities[0].Filter[0];
               var actual = context.SqlSelectFacetFromInput(filter, new NullConnectionFactory() { AdoProvider = AdoProvider.MySql, SupportsLimit = true });
               Assert.AreEqual("SELECT CAST(CONCAT(f2,' (',COUNT(*),')') AS CHAR) AS From, f2 AS To FROM Fact WHERE (1=2) GROUP BY f2 ORDER BY f2 ASC LIMIT 29", actual);
            }

         }
      }

      [TestMethod]
      public void CreateForSqlite() {
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(TestCfg1, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            process.Entities.First().Filter.First().Size = 31;

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var filter = context.Process.Entities[0].Filter[0];
               var actual = context.SqlSelectFacetFromInput(filter, new NullConnectionFactory() { AdoProvider = AdoProvider.SqLite, SupportsLimit = true });
               Assert.AreEqual("SELECT CAST(f2 AS NVARCHAR(128)) || ' (' || CAST(COUNT(*) AS NVARCHAR(32)) || ')' AS From, f2 AS To FROM Fact WHERE (1=2) GROUP BY f2 ORDER BY f2 ASC LIMIT 31", actual);
            }

         }
      }


      [TestMethod]
      public void AllFiltersAreIgnored() {
         const string xml = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='*' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' value='@[f2]' type='facet' size='0' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
   </cfg>
";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var filter = context.Process.Entities[0].Filter[0];
               var actual = context.SqlSelectFacetFromInput(filter, new NullConnectionFactory() { AdoProvider = AdoProvider.SqlServer, SupportsLimit = false });
               Assert.AreEqual("SELECT CAST(f2 AS NVARCHAR(128)) + ' (' + CAST(COUNT(*) AS NVARCHAR(32)) + ')' AS From, f2 AS To FROM Fact GROUP BY f2 ORDER BY f2 ASC", actual);
            }

         }

      }
   }
}
