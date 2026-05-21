using Autofac;
using Dapper;
using Microsoft.Data.SqlClient;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.SqlServer.Autofac;

namespace Test.Unit.SqlServer {

   [TestClass]
   public class TestFullTextSearch {

      private static bool _ftsAvailable;

      [ClassInitialize]
      public static async Task ClassInit(TestContext context) {
         try {
            using var cn = new SqlConnection(Tester.GetConnectionString("Northwind"));
            await cn.OpenAsync();

            await cn.ExecuteAsync(@"
IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'NorthwindFtsCatalog')
    CREATE FULLTEXT CATALOG NorthwindFtsCatalog AS DEFAULT;");

            await cn.ExecuteAsync(@"
IF NOT EXISTS (
    SELECT * FROM sys.fulltext_indexes fi
    JOIN sys.objects o ON fi.object_id = o.object_id
    WHERE o.name = 'Products')
BEGIN
    CREATE FULLTEXT INDEX ON Products(ProductName)
        KEY INDEX PK_Products
        ON NorthwindFtsCatalog
        WITH CHANGE_TRACKING AUTO;
END");

            await cn.ExecuteAsync(@"
DECLARE @status int = 1;
WHILE @status <> 0
BEGIN
    SELECT @status = CONVERT(int, OBJECTPROPERTYEX(OBJECT_ID('Products'), 'TableFulltextPopulateStatus'));
    IF @status <> 0 WAITFOR DELAY '00:00:02';
END");

            _ftsAvailable = true;
         } catch (SqlException ex) when (ex.Message.Contains("Full-Text Search is not installed")) {
            Console.WriteLine("SQL Server Full-Text Search is not installed in this container — FTS tests will be skipped.");
            _ftsAvailable = false;
         }
      }

      // --- CONTAINS (default — no query-type needed) ---

      [TestMethod]
      public void ContainsDefaultSearchForChaiReturnsProduct() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         var xml = $@"<add name='NorthwindFts'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from FREETEXT search for 'Chai'");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString().Contains("Chai")));
      }

      [TestMethod]
      public void FreetextMultiWordReturnsResults() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         // FREETEXT accepts plain multi-word input without any operators
         var xml = $@"<add name='NorthwindFtsMulti'>
  <search-types>
    <add name='fulltext' query-type='freetext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai Chang' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "FREETEXT with 'Chai Chang' should return at least one product");
      }

      [TestMethod]
      public void FreetextNegatedSearchExcludesChaiProduct() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         var xml = $@"<add name='NorthwindFtsNot'>
  <search-types>
    <add name='fulltext' query-type='freetext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' operator='notequal' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Negated FREETEXT should return products that don't match 'Chai'");
         Assert.IsFalse(rows.Any(r => r["ProductName"].ToString().Contains("Chai")));
      }

      // --- CONTAINS (opt-in via query-type='contains') ---

      [TestMethod]
      public void ContainsPhraseSearchFindsExactProduct() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         // Quoted phrase passed through normalizer unchanged → CONTAINS(ProductName, '"Aniseed Syrup"')
         var xml = $@"<add name='NorthwindFtsPhrase'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Aniseed Syrup&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Phrase search for 'Aniseed Syrup' should return exactly one product");
         Assert.AreEqual("Aniseed Syrup", rows.First()["ProductName"].ToString());
      }

      [TestMethod]
      public void ContainsBooleanOrFindsMultipleProducts() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         var xml = $@"<add name='NorthwindFtsBoolOr'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Chai&quot; OR &quot;Chang&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Boolean OR should return exactly Chai and Chang");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString() == "Chai"));
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString() == "Chang"));
      }

      [TestMethod]
      public void ContainsPrefixSearchFindsChefProducts() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         // Quoted prefix passed through normalizer unchanged → CONTAINS(ProductName, '"Chef*"')
         var xml = $@"<add name='NorthwindFtsPrefix'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Chef*&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Prefix 'Chef*' should return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["ProductName"].ToString()!.StartsWith("Chef")));
      }

      [TestMethod]
      public void ContainsBooleanAndRequiresBothTerms() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         var xml = $@"<add name='NorthwindFtsBoolAnd'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Cajun&quot; AND &quot;Seasoning&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "AND of 'Cajun' and 'Seasoning' should return only Chef Anton's Cajun Seasoning");
         Assert.AreEqual("Chef Anton's Cajun Seasoning", rows.First()["ProductName"].ToString());
      }

      [TestMethod]
      public void ContainsNormalizerAutoQuotesPrefixSearch() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         // User enters Chef* without quotes — normalizer wraps it in "Chef*" automatically
         var xml = $@"<add name='NorthwindFtsNormPrefix'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='Chef*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Normalizer should auto-quote Chef* and return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["ProductName"].ToString()!.StartsWith("Chef")));
      }

      [TestMethod]
      public void ContainsNormalizerAutoJoinsMultiWordWithAnd() {
         if (!_ftsAvailable) { Assert.Inconclusive("SQL Server Full-Text Search is not installed."); return; }
         // User enters bare words — normalizer joins them with AND automatically
         var xml = $@"<add name='NorthwindFtsNormMulti'>
  <search-types>
    <add name='fulltext' query-type='contains' />
  </search-types>
  <parameters>
    <add name='search' value='Cajun Seasoning' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='{Tester.Server},{Tester.Port}' encrypt='true' trust-server-certificate='true' database='Northwind' user='{Tester.User}' password='{Tester.Pw}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new SqlServerModule()).CreateScope(process, logger);
         inner.Resolve<IProcessController>().Execute();
         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Normalizer should join 'Cajun Seasoning' with AND, returning only Chef Anton's Cajun Seasoning");
         Assert.AreEqual("Chef Anton's Cajun Seasoning", rows.First()["ProductName"].ToString());
      }
   }
}
