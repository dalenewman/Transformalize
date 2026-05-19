using Autofac;
using Npgsql;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.PostgreSql.Autofac;

namespace Test {

   [TestClass]
   public class TestFullTextSearch {

      [ClassInitialize]
      public static async Task ClassInit(TestContext context) {
         await using var cn = new NpgsqlConnection(Tester.GetConnectionString("northwind_source"));
         await cn.OpenAsync();

         // Create GIN index on products(product_name) for FTS
         await using var cmd = new NpgsqlCommand(@"
CREATE INDEX IF NOT EXISTS idx_products_fts
    ON products USING GIN(to_tsvector('english', product_name));", cn);
         cmd.CommandTimeout = 60;
         await cmd.ExecuteNonQueryAsync();
      }

      [TestMethod]
      public void SearchForChaiReturnsProduct1() {
         var xml = $@"<add name='NorthwindFts'>
  <search-types>
    <add name='fulltext' analyzer='english' />
  </search-types>
  <parameters>
    <add name='search' value='chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='postgresql' server='{Tester.Server}' port='{Tester.Port}' database='northwind_source' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='products'>
      <filter>
        <add field='product_name' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='product_id' type='int' primary-key='true' />
        <add name='product_name' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new PostgreSqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from FTS search for 'chai'");
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)), "Expected a row with product_name containing 'Chai'");
      }

      [TestMethod]
      public void WebQueryTypeSearchWorks() {
         var xml = $@"<add name='NorthwindFtsWeb'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='web' />
  </search-types>
  <parameters>
    <add name='search' value='chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='postgresql' server='{Tester.Server}' port='{Tester.Port}' database='northwind_source' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='products'>
      <filter>
        <add field='product_name' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='product_id' type='int' primary-key='true' />
        <add name='product_name' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));

         using var inner = new Container(new PostgreSqlModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row using websearch_to_tsquery for 'chai'");
      }
   }
}
