using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Providers.Sqlite.Autofac;

namespace IntegrationTests {

   [TestClass]
   public class TestFullTextSearch {

      private const string FtsDbFile = "files/northwind-fts.sqlite3";

      [ClassInitialize]
      public static void ClassInit(TestContext context) {
         // Make a fresh copy of the Northwind database for FTS testing
         File.Copy("files/northwind-sqlite.db", FtsDbFile, overwrite: true);

         // Create the FTS5 virtual table for Products
         using var cn = new SqliteConnection($"Data Source={FtsDbFile}");
         cn.Open();

         using var createCmd = cn.CreateCommand();
         createCmd.CommandText = @"
CREATE VIRTUAL TABLE IF NOT EXISTS Products_fts
    USING fts5(ProductName, content='Products', content_rowid='ProductID');";
         createCmd.ExecuteNonQuery();

         using var rebuildCmd = cn.CreateCommand();
         rebuildCmd.CommandText = "INSERT INTO Products_fts(Products_fts) VALUES('rebuild');";
         rebuildCmd.ExecuteNonQuery();
      }

      [ClassCleanup]
      public static void ClassCleanup() {
         if (File.Exists(FtsDbFile))
            File.Delete(FtsDbFile);
      }

      [TestMethod]
      public void SearchForChaiReturnsProduct1() {
         var xml = $@"<add name='NorthwindFts'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected at least one row from FTS search for 'Chai'");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void NegatedSearchExcludesChaiProduct() {
         var xml = $@"<add name='NorthwindFtsNot'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.IsTrue(rows.Any(), "Expected rows — negated FTS should return products that don't match 'Chai'");
         Assert.IsFalse(rows.Any(r => r["ProductName"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void PhraseSearchFindsExactProduct() {
         // FTS5 phrase syntax: "Aniseed Syrup"
         var xml = $@"<add name='NorthwindFtsPhrase'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='&quot;Aniseed Syrup&quot;' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Phrase search for 'Aniseed Syrup' should return exactly one product");
         Assert.AreEqual("Aniseed Syrup", rows.First()["ProductName"].ToString());
      }

      [TestMethod]
      public void OrSearchFindsMultipleProducts() {
         // FTS5 OR syntax: Chai OR Chang
         var xml = $@"<add name='NorthwindFtsOr'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chai OR Chang' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "OR search should return exactly Chai and Chang");
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString()!.Contains("Chai", StringComparison.OrdinalIgnoreCase)));
         Assert.IsTrue(rows.Any(r => r["ProductName"].ToString()!.Contains("Chang", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void PrefixSearchFindsChefProducts() {
         // FTS5 prefix syntax: Chef*
         var xml = $@"<add name='NorthwindFtsPrefix'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chef*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(2, rows.Count, "Prefix 'Chef*' should return both Chef Anton products");
         Assert.IsTrue(rows.All(r => r["ProductName"].ToString()!.StartsWith("Chef", StringComparison.OrdinalIgnoreCase)));
      }

      [TestMethod]
      public void AndSearchRequiresBothTerms() {
         // FTS5 AND syntax: Cajun AND Seasoning — both are in "Chef Anton's Cajun Seasoning"
         var xml = $@"<add name='NorthwindFtsAnd'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Cajun AND Seasoning' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "AND of 'Cajun' and 'Seasoning' should return only 'Chef Anton's Cajun Seasoning'");
         Assert.AreEqual("Chef Anton's Cajun Seasoning", rows.First()["ProductName"].ToString());
      }

      [TestMethod]
      public void NotSearchExcludesTerm() {
         // FTS5 NOT syntax: Chef* NOT Cajun — matches Chef Anton's Gumbo Mix but not Cajun Seasoning
         var xml = $@"<add name='NorthwindFtsNot2'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='Chef* NOT Cajun' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlite' file='{FtsDbFile}' />
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

         using var inner = new Container(new SqliteModule()).CreateScope(process, logger);
         var controller = inner.Resolve<IProcessController>();
         controller.Execute();

         var rows = process.Entities.First().Rows;
         Assert.AreEqual(1, rows.Count, "Chef* NOT Cajun should return only Chef Anton's Gumbo Mix");
         Assert.AreEqual("Chef Anton's Gumbo Mix", rows.First()["ProductName"].ToString());
      }
   }
}
