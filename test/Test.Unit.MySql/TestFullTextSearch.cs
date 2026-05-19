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

         // Check if FULLTEXT index already exists before adding
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
         // MySQL Northwind uses anonymized product names like 'Product HHYDP'.
         // Search for 'HHYDP' which appears only in productId=1.
         var xml = $@"<add name='NorthwindFts'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='HHYDP' prompt='true' />
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
         Assert.IsTrue(rows.Any(), "Expected at least one row from FTS search for 'HHYDP'");
         Assert.IsTrue(rows.Any(r => r["productName"].ToString()!.Contains("HHYDP", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void NaturalLanguageModeSearchWorks() {
         var xml = $@"<add name='NorthwindFtsNatural'>
  <search-types>
    <add name='fulltext' mode='natural' />
  </search-types>
  <parameters>
    <add name='search' value='RECZE' prompt='true' />
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
         Assert.IsTrue(rows.Any(), "Expected at least one row from natural language FTS search for 'RECZE'");
         Assert.IsTrue(rows.Any(r => r["productName"].ToString()!.Contains("RECZE", StringComparison.OrdinalIgnoreCase)));
      }
   }
}
