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

      private static string BuildXmlWithValue(string value, string queryType = "plain", string analyzer = "") {
         return $@"<cfg name='name' mode='report'>
  <search-types>
    <add name='fulltext' analyzer='{analyzer}' query-type='{queryType}' />
  </search-types>
  <parameters>
    <add name='search' value='{value}' prompt='true' />
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
      }

      private static string RunSqlServer(string xml, AdoProvider provider = AdoProvider.SqlServer) {
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(xml, logger);
         var process = outer.Resolve<Process>();
         Assert.AreEqual(0, process.Errors().Length, string.Join(", ", process.Errors()));
         using var inner = new Container(new AdoProviderModule()).CreateScope(process, logger);
         var context = inner.ResolveNamed<InputContext>("nameProducts");
         return context.SqlSelectInput(
            context.Entity.GetAllFields().Where(f => f.Input).ToArray(),
            new NullConnectionFactory { AdoProvider = provider });
      }

      // --- SQL Server: CONTAINS (default) ---

      [TestMethod]
      public void SqlServerDefaultGeneratesContains() {
         var actual = RunSqlServer(BuildXml());
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void SqlServerContainsWithLanguageClause() {
         var actual = RunSqlServer(BuildXml(analyzer: "french"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai' LANGUAGE 'french'))", actual);
      }

      // --- SQL Server: FREETEXT (opt-in via query-type='freetext') ---

      [TestMethod]
      public void SqlServerFreetextExplicit() {
         var actual = RunSqlServer(BuildXml(queryType: "freetext"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (FREETEXT(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void SqlServerFreetextWithLanguageClause() {
         var actual = RunSqlServer(BuildXml(queryType: "freetext", analyzer: "french"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (FREETEXT(ProductName, 'chai' LANGUAGE 'french'))", actual);
      }

      // --- CONTAINS normalizer unit tests ---

      [TestMethod]
      public void ContainsSingleWordPassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("chai", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsPrefixAutoQuoted() {
         var actual = RunSqlServer(BuildXmlWithValue("chef*", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\"'))", actual);
      }

      [TestMethod]
      public void ContainsAlreadyQuotedPrefixPassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("&quot;chef*&quot;", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\"'))", actual);
      }

      [TestMethod]
      public void ContainsMultiWordJoinedWithAnd() {
         var actual = RunSqlServer(BuildXmlWithValue("chai chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai AND chang'))", actual);
      }

      [TestMethod]
      public void ContainsMultiWordWithPrefixNormalized() {
         var actual = RunSqlServer(BuildXmlWithValue("chef* cajun", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\" AND cajun'))", actual);
      }

      [TestMethod]
      public void ContainsExplicitOrPassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("chai OR chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai OR chang'))", actual);
      }

      [TestMethod]
      public void ContainsExplicitAndPassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("&quot;Chai&quot; AND &quot;Chang&quot;", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"Chai\" AND \"Chang\"'))", actual);
      }

      [TestMethod]
      public void ContainsQuotedPhrasePassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("&quot;Aniseed Syrup&quot;", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"Aniseed Syrup\"'))", actual);
      }

      [TestMethod]
      public void ContainsAndNotPassthrough() {
         var actual = RunSqlServer(BuildXmlWithValue("&quot;Chai&quot; AND NOT &quot;Chang&quot;", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"Chai\" AND NOT \"Chang\"'))", actual);
      }

      [TestMethod]
      public void ContainsBareNotAutoFixed() {
         var actual = RunSqlServer(BuildXmlWithValue("chai NOT chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai AND NOT chang'))", actual);
      }

      // --- CONTAINS normalizer: suffix (leading *) handling ---

      [TestMethod]
      public void ContainsSuffixStripsLeadingAsterisk() {
         // *word is not valid in CONTAINS — leading * is stripped, result is plain word
         var actual = RunSqlServer(BuildXmlWithValue("*chai", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsSuffixAndPrefixCombined() {
         // *word* — strip leading *, keep trailing * and quote it → "word*"
         var actual = RunSqlServer(BuildXmlWithValue("*chai*", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chai*\"'))", actual);
      }

      // --- CONTAINS normalizer: operators present but terms still need fixing ---

      [TestMethod]
      public void ContainsMixedPrefixWithOr() {
         // chef* is still unquoted even though OR is present — normalizer fixes it
         var actual = RunSqlServer(BuildXmlWithValue("chef* OR chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\" OR chang'))", actual);
      }

      [TestMethod]
      public void ContainsMixedPrefixesWithAnd() {
         // Both unquoted prefix tokens should be quoted even though AND is present
         var actual = RunSqlServer(BuildXmlWithValue("chef* AND cajun*", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\" AND \"cajun*\"'))", actual);
      }

      [TestMethod]
      public void ContainsMixedLeadingAndTrailingAsterisk() {
         // *chef* AND cajun — strip leading *, quote trailing *, leave cajun alone
         var actual = RunSqlServer(BuildXmlWithValue("*chef* AND cajun", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"chef*\" AND cajun'))", actual);
      }

      [TestMethod]
      public void ContainsMixedSuffixAndOr() {
         // *chai OR *chang — strip both leading wildcards
         var actual = RunSqlServer(BuildXmlWithValue("*chai OR *chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai OR chang'))", actual);
      }

      // --- CONTAINS normalizer: unbalanced/dangling operators ---

      [TestMethod]
      public void ContainsDanglingLeadingOr() {
         // "OR chai" — leading OR has no left-hand term, stripped
         var actual = RunSqlServer(BuildXmlWithValue("OR chai", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsDanglingLeadingAnd() {
         // "AND chai" — leading AND has no left-hand term, stripped
         var actual = RunSqlServer(BuildXmlWithValue("AND chai", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsDanglingLeadingAndNot() {
         // "AND NOT chai" — leading AND NOT has no left-hand term, stripped
         var actual = RunSqlServer(BuildXmlWithValue("AND NOT chai", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsDanglingTrailingOr() {
         // "chai OR" — trailing OR has no right-hand term, stripped
         var actual = RunSqlServer(BuildXmlWithValue("chai OR", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsDanglingTrailingAnd() {
         // "chai AND" — trailing AND has no right-hand term, stripped
         var actual = RunSqlServer(BuildXmlWithValue("chai AND", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai'))", actual);
      }

      [TestMethod]
      public void ContainsDanglingTrailingAndFullExpression() {
         // "something* AND somethingelse OR" — trailing OR stripped, prefix still quoted
         var actual = RunSqlServer(BuildXmlWithValue("something* AND somethingelse OR", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, '\"something*\" AND somethingelse'))", actual);
      }

      [TestMethod]
      public void ContainsAdjacentOperatorsKeepsFirst() {
         // "chai AND OR chang" — consecutive operators, first one (AND) wins
         var actual = RunSqlServer(BuildXmlWithValue("chai AND OR chang", queryType: "contains"));
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (CONTAINS(ProductName, 'chai AND chang'))", actual);
      }

      // --- Other providers (unchanged behavior) ---

      [TestMethod]
      public void PostgreSqlDefaultGeneratesPlainTsquery() {
         var actual = RunSqlServer(BuildXml(), AdoProvider.PostgreSql);
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (to_tsvector('english', ProductName) @@ plainto_tsquery('english', 'chai'))", actual);
      }

      [TestMethod]
      public void PostgreSqlWebQueryType() {
         var actual = RunSqlServer(BuildXml(queryType: "web"), AdoProvider.PostgreSql);
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products WHERE (to_tsvector('english', ProductName) @@ websearch_to_tsquery('english', 'chai'))", actual);
      }

      [TestMethod]
      public void MySqlDefaultGeneratesBooleanMatch() {
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(BuildXml(), logger);
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
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(BuildXml(mode: "natural"), logger);
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
         var logger = new ConsoleLogger();
         using var outer = new ConfigurationContainer().CreateScope(BuildXml(), logger);
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
         var actual = RunSqlServer(xml);
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
         var actual = RunSqlServer(xml);
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
         var actual = RunSqlServer(xml);
         Assert.AreEqual("SELECT ProductID,ProductName FROM Products", actual);
      }
   }
}
