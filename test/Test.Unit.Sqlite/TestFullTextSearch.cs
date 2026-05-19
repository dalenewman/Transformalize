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
   }
}
