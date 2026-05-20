using Autofac;
using MySqlConnector;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.MySql.Autofac;

namespace Test {

   [TestClass]
   public class TestFullTextSearch {

      [ClassInitialize]
      public static async Task ClassInit(TestContext context) {
         await using var cn = new MySqlConnection(Tester.GetConnectionString("northwind"));
         await cn.OpenAsync();

         await using var checkCmd = new MySqlCommand(
            "SELECT COUNT(*) FROM information_schema.STATISTICS " +
            "WHERE table_schema='northwind' AND table_name='Product' AND index_name='idx_product_fts';", cn);
         var exists = Convert.ToInt64(await checkCmd.ExecuteScalarAsync()) > 0;

         if (!exists) {
            await using var addCmd = new MySqlCommand(
               "ALTER TABLE `Product` ADD FULLTEXT INDEX idx_product_fts (`productName`);", cn);
            addCmd.CommandTimeout = 60;
            await addCmd.ExecuteNonQueryAsync();
         }
      }

      [TestMethod]
      public void SearchReturnsMatchingProduct() {
         var xml = $@"<add name='NorthwindFts'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from FTS search for 'Chai'");
         Assert.IsTrue(rows.Any(r => r["productName"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void NaturalLanguageModeSearchWorks() {
         var xml = $@"<add name='NorthwindFtsNatural'>
  <search-types>
    <add name='fulltext' mode='natural' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from natural language FTS search for 'Chai'");
         Assert.IsTrue(rows.Any(r => r["productName"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void BooleanPhraseSearchFindsExactProduct() {
         // Boolean mode phrase syntax: "Aniseed Syrup"
         var xml = $@"<add name='NorthwindFtsBoolPhrase'>
  <search-types>
    <add name='fulltext' mode='boolean' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Aniseed Syrup&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Phrase search for 'Aniseed Syrup' should return exactly one product");
         Assert.AreEqual("Aniseed Syrup", rows.First()["productName"].ToString());
      }

      [TestMethod]
      public void BooleanPrefixSearchFindsChefProducts() {
         // Boolean mode prefix syntax: Chef* — matches both Chef Anton products
         var xml = $@"<add name='NorthwindFtsBoolPrefix'>
  <search-types>
    <add name='fulltext' mode='boolean' />
  </search-types>
  <parameters>
    <add name='search' value='Chef*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Prefix 'Chef*' should return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["productName"].ToString()!.StartsWith("Chef", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void BooleanRequiredTermsNarrowResults() {
         // +Aniseed +Syrup requires both terms — only "Aniseed Syrup" contains both
         var xml = $@"<add name='NorthwindFtsBoolAnd'>
  <search-types>
    <add name='fulltext' mode='boolean' />
  </search-types>
  <parameters>
    <add name='search' value='+Aniseed +Syrup' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Required terms +Aniseed +Syrup should return only 'Aniseed Syrup'");
         Assert.AreEqual("Aniseed Syrup", rows.First()["productName"].ToString());
      }

      [TestMethod]
      public void BooleanExclusionOmitsProduct() {
         // Chef* -Cajun returns Chef Anton's Gumbo Mix but not Chef Anton's Cajun Seasoning
         var xml = $@"<add name='NorthwindFtsBoolExclude'>
  <search-types>
    <add name='fulltext' mode='boolean' />
  </search-types>
  <parameters>
    <add name='search' value='Chef* -Cajun' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='mysql' server='{Tester.Server}' port='{Tester.Port}' database='northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Product'>
      <filter>
        <add field='productName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='productId' type='int' primary-key='true' />
        <add name='productName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new MySqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Chef* -Cajun should return only Chef Anton's Gumbo Mix");
         Assert.AreEqual("Chef Anton's Gumbo Mix", rows.First()["productName"].ToString());
      }
   }
}
