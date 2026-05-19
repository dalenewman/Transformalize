using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Providers.Console;

namespace Test.Unit {

   [TestClass]
   public class TestFullTextFilters {

      private const string XmlTemplate = @"<cfg name='name' mode='report'>
  <search-types>
    <add name='fulltext' analyzer='{0}' query-type='{1}' mode='{2}' />
  </search-types>
  <parameters>
    <add name='search' value='chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Northwind' />
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
</cfg>";

      private static string BuildXml(string analyzer = "", string queryType = "plain", string mode = "boolean") {
         return string.Format(XmlTemplate, analyzer, queryType, mode);
      }

      [TestMethod]
      public void SqlServerDefaultGeneratesContains() {
         var xml = BuildXml();
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void SqlServerWithLanguageClause() {
         var xml = BuildXml(analyzer: "french");
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai' LANGUAGE 'french'))", actual);
      }

      [TestMethod]
      public void PostgreSqlDefaultGeneratesPlainTsquery() {
         var xml = BuildXml();
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.PostgreSql });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (to_tsvector('english', ProductName) @@ plainto_tsquery('english', 'chai'))", actual);
      }

      [TestMethod]
      public void PostgreSqlWebQueryType() {
         var xml = BuildXml(queryType: "web");
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.PostgreSql });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (to_tsvector('english', ProductName) @@ websearch_to_tsquery('english', 'chai'))", actual);
      }

      [TestMethod]
      public void MySqlDefaultGeneratesBooleanMatch() {
         var xml = BuildXml();
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.MySql, SupportsLimit = true });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (MATCH(ProductName) AGAINST('chai' IN BOOLEAN MODE))", actual);
      }

      [TestMethod]
      public void MySqlNaturalLanguageMode() {
         var xml = BuildXml(mode: "natural");
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.MySql, SupportsLimit = true });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (MATCH(ProductName) AGAINST('chai' IN NATURAL LANGUAGE MODE))", actual);
      }

      [TestMethod]
      public void SqliteGeneratesRowidSubquery() {
         var xml = BuildXml();
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqLite, SupportsLimit = true });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (rowid IN (SELECT rowid FROM Products_fts WHERE Products_fts MATCH 'chai'))", actual);
      }

      [TestMethod]
      public void NegationWrapsInNot() {
         const string xml = @"<cfg name='name' mode='report'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Northwind' />
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
</cfg>";
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (NOT (CONTAINS(ProductName, 'chai')))", actual);
      }

      [TestMethod]
      public void FieldWithoutSearchTypeStillGeneratesLike() {
         const string xml = @"<cfg name='name' mode='report'>
  <parameters>
    <add name='search' value='chai' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Northwind' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[search]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' />
      </fields>
    </add>
  </entities>
</cfg>";
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (ProductName LIKE '%chai%')", actual);
      }

      [TestMethod]
      public void WildcardValueIsIgnored() {
         const string xml = @"<cfg name='name' mode='report'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='search' value='*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Northwind' />
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
</cfg>";
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length);
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         var actual = context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer });
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products", actual);
      }
   }
}
