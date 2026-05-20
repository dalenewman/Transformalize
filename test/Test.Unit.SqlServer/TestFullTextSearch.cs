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

            // Create FTS catalog and index on Products(ProductName)
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

            // Wait for initial population to complete
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

      [TestMethod]
      public void SearchForChaiReturnsProduct1() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from FTS search for 'Chai'");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString().Contains("Chai")), "Expected a row with ProductName containing 'Chai'");
      }

      [TestMethod]
      public void NegatedSearchExcludesChaiProduct() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }

         var xml = $@"<add name='NorthwindFtsNot'>
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected rows — negated FTS should return products that don't match 'Chai'");
         Assert.IsFalse(rows.Any(r => r["ProductName"].ToString().Contains("Chai")), "Expected no rows with ProductName 'Chai' in negated result");
      }

      [TestMethod]
      public void PhraseSearchFindsExactProduct() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }
         // CONTAINS phrase syntax wraps the phrase in double quotes: "Aniseed Syrup"
         var xml = $@"<add name='NorthwindFtsPhrase'>
  <search-types>
    <add name='fulltext' />
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Phrase search for 'Aniseed Syrup' should return exactly one product");
         Assert.AreEqual("Aniseed Syrup", rows.First()["ProductName"].ToString());
      }

      [TestMethod]
      public void BooleanOrFindsMultipleProducts() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }
         // CONTAINS boolean OR: "Chai" OR "Chang"
         var xml = $@"<add name='NorthwindFtsBoolOr'>
  <search-types>
    <add name='fulltext' />
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Boolean OR should return exactly Chai and Chang");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString() == "Chai"));
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString() == "Chang"));
      }

      [TestMethod]
      public void PrefixSearchFindsChefProducts() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }
         // CONTAINS prefix syntax: "Chef*"
         var xml = $@"<add name='NorthwindFtsPrefix'>
  <search-types>
    <add name='fulltext' />
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Prefix 'Chef*' should return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["ProductName"].ToString()!.StartsWith("Chef")));
      }

      [TestMethod]
      public void BooleanAndRequiresBothTerms() {
         if (!_ftsAvailable) {
            Assert.Inconclusive("SQL Server Full-Text Search is not installed in this environment.");
            return;
         }
         // CONTAINS boolean AND: "Cajun" AND "Seasoning" — both words are in "Chef Anton's Cajun Seasoning"
         var xml = $@"<add name='NorthwindFtsBoolAnd'>
  <search-types>
    <add name='fulltext' />
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
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "AND of 'Cajun' and 'Seasoning' should return only 'Chef Anton's Cajun Seasoning'");
         Assert.AreEqual("Chef Anton's Cajun Seasoning", rows.First()["ProductName"].ToString());
      }
   }
}
