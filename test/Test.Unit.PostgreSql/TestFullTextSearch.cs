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

      [TestMethod]
      public void WebQueryPhraseSearchFindsExactProduct() {
         // websearch_to_tsquery recognises "quoted phrases"
         var xml = $@"<add name='NorthwindFtsWebPhrase'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='web' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;aniseed syrup&quot;' prompt='true' />
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
         Assert.AreEqual(1, rows.Count, "Phrase search for 'aniseed syrup' should return exactly one product");
         Assert.IsTrue(rows.First()["product_name"].ToString()!.Contains("Aniseed", StringComparison.OrdinalIgnoreCase));
      }

      [TestMethod]
      public void WebQueryOrFindsMultipleProducts() {
         // websearch_to_tsquery supports OR keyword
         var xml = $@"<add name='NorthwindFtsWebOr'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='web' />
  </search-types>
  <parameters>
    <add name='search' value='chai OR chang' prompt='true' />
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
         Assert.AreEqual(2, rows.Count, "OR search should return exactly Chai and Chang");
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chang", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void WebQueryExclusionOmitsProduct() {
         // websearch_to_tsquery supports -term exclusion
         var xml = $@"<add name='NorthwindFtsWebExclude'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='web' />
  </search-types>
  <parameters>
    <add name='search' value='chai -chang' prompt='true' />
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
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)), "Should find Chai");
         Assert.IsFalse(rows.Any(r => r["product_name"].ToString()!.Contains("Chang", StringComparison.OrdinalIgnoreCase)), "Should not find Chang");
      }

      [TestMethod]
      public void RawQueryOrOperatorFindsMultipleProducts() {
         // to_tsquery raw syntax: chai | chang
         var xml = $@"<add name='NorthwindFtsRawOr'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='raw' />
  </search-types>
  <parameters>
    <add name='search' value='chai | chang' prompt='true' />
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
         Assert.AreEqual(2, rows.Count, "Raw OR (|) should return exactly Chai and Chang");
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
         Assert.IsTrue(rows.Any(r => r["product_name"].ToString()!.Contains("Chang", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void RawQueryPrefixFindsChefProducts() {
         // to_tsquery prefix syntax: chef:*
         var xml = $@"<add name='NorthwindFtsRawPrefix'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='raw' />
  </search-types>
  <parameters>
    <add name='search' value='chef:*' prompt='true' />
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
         Assert.AreEqual(2, rows.Count, "Prefix 'chef:*' should return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["product_name"].ToString()!.StartsWith("Chef", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void PhraseQueryTypeFindsAdjacentWords() {
         // phraseto_tsquery matches words in order: aniseed syrup
         var xml = $@"<add name='NorthwindFtsPhrase'>
  <search-types>
    <add name='fulltext' analyzer='english' query-type='phrase' />
  </search-types>
  <parameters>
    <add name='search' value='aniseed syrup' prompt='true' />
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
         Assert.AreEqual(1, rows.Count, "phraseto_tsquery for 'aniseed syrup' should return exactly one product");
         Assert.IsTrue(rows.First()["product_name"].ToString()!.Contains("Aniseed", StringComparison.OrdinalIgnoreCase));
      }
   }
}
