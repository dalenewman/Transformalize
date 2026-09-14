using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Logging;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Providers.SQLite;

namespace Test.Unit.Sqlite;

[TestClass]
public class TestParameterizedFilters {

   [TestMethod]
   public void FilterValueIsBoundInsteadOfInterpolated() {
      const string hostileValue = "missing' OR 1=1 --";
      var (context, factory, field) = CreateContext(hostileValue);

      using var connection = new SqliteConnection("Data Source=:memory:");
      connection.Open();
      using (var setup = connection.CreateCommand()) {
         setup.CommandText = "CREATE TABLE People (Name TEXT); INSERT INTO People VALUES ('Alice'), ('Bob');";
         setup.ExecuteNonQuery();
      }

      using var command = connection.CreateCommand();
      command.CommandText = context.SqlSelectInput(new[] { field }, factory);
      command.AddAdoParameters(context, factory);

      Assert.AreEqual("SELECT \"Name\" FROM \"People\" WHERE (\"Name\" = @TflFilter0)", command.CommandText);
      Assert.AreEqual(hostileValue, command.Parameters["@TflFilter0"].Value);
      using var reader = command.ExecuteReader();
      Assert.IsFalse(reader.Read(), "The hostile filter text must be treated as a value, not executable SQL.");
   }

   [TestMethod]
   public void ListFilterCreatesOneTypedParameterPerValue() {
      var connection = new Connection { Name = "input", Provider = "sqlite", ConnectionString = "Data Source=:memory:" };
      var field = new Field { Name = "Id", Alias = "Id", Type = "int" };
      var filter = new Filter {
         Field = field.Name,
         Value = "1,2",
         Operator = "in",
         Delimiter = ",",
         IsField = true,
         LeftField = field
      };
      var entity = new Entity { Name = "People", Alias = "People", Input = "input", Fields = new() { field }, Filter = new() { filter } };
      var process = new Process { Name = "Test", Connections = new() { connection }, Entities = new() { entity } };
      var context = new InputContext(new PipelineContext(new NullLogger(), process, entity));
      var factory = new SqliteConnectionFactory(connection);

      using var command = new SqliteCommand {
         CommandText = context.SqlSelectInput(new[] { field }, factory)
      };
      command.AddAdoParameters(context, factory);

      Assert.AreEqual("SELECT \"Id\" FROM \"People\" WHERE (\"Id\" IN (@TflFilter0_0,@TflFilter0_1))", command.CommandText);
      Assert.AreEqual(1, command.Parameters["@TflFilter0_0"].Value);
      Assert.AreEqual(2, command.Parameters["@TflFilter0_1"].Value);
   }

   [TestMethod]
   [DataRow("chai", "chai")]
   [DataRow("chef*", "\"chef*\"")]
   [DataRow("\"chef*\"", "\"chef*\"")]
   [DataRow("chai chang", "chai AND chang")]
   [DataRow("chef* cajun", "\"chef*\" AND cajun")]
   [DataRow("chai OR chang", "chai OR chang")]
   [DataRow("\"Chai\" AND \"Chang\"", "\"Chai\" AND \"Chang\"")]
   [DataRow("\"Aniseed Syrup\"", "\"Aniseed Syrup\"")]
   [DataRow("\"Chai\" AND NOT \"Chang\"", "\"Chai\" AND NOT \"Chang\"")]
   [DataRow("chai NOT chang", "chai AND NOT chang")]
   [DataRow("*chai", "chai")]
   [DataRow("*chai*", "\"chai*\"")]
   [DataRow("chef* OR chang", "\"chef*\" OR chang")]
   [DataRow("chef* AND cajun*", "\"chef*\" AND \"cajun*\"")]
   [DataRow("*chef* AND cajun", "\"chef*\" AND cajun")]
   [DataRow("*chai OR *chang", "chai OR chang")]
   [DataRow("OR chai", "chai")]
   [DataRow("AND chai", "chai")]
   [DataRow("AND NOT chai", "chai")]
   [DataRow("chai OR", "chai")]
   [DataRow("chai AND", "chai")]
   [DataRow("something* AND somethingelse OR", "\"something*\" AND somethingelse")]
   [DataRow("chai AND OR chang", "chai AND chang")]
   public void SqlServerContainsNormalizationIsAppliedToParameter(string input, string expected) {
      var connection = new Connection { Name = "input", Provider = "sqlserver" };
      var field = new Field { Name = "Name", Alias = "Name", Type = "string", SearchType = "fulltext" };
      var filter = new Filter {
         Field = field.Name,
         Value = input,
         Operator = "equal",
         Type = "search",
         WildCard = "*",
         IsField = true,
         LeftField = field
      };
      var entity = new Entity { Name = "People", Alias = "People", Input = "input", Fields = new() { field }, Filter = new() { filter } };
      var process = new Process {
         Name = "Test",
         Connections = new() { connection },
         Entities = new() { entity },
         SearchTypes = new() { new SearchType { Name = "fulltext", QueryType = "contains" } }
      };
      var context = new InputContext(new PipelineContext(new NullLogger(), process, entity));
      var factory = new NullConnectionFactory { AdoProvider = AdoProvider.SqlServer };

      using var command = new SqliteCommand { CommandText = context.ResolveFilter(factory) };
      command.AddAdoParameters(context, factory);

      Assert.AreEqual(expected, command.Parameters["@TflFilter0"].Value);
   }

   private static (InputContext Context, SqliteConnectionFactory Factory, Field Field) CreateContext(string value) {
      var connection = new Connection { Name = "input", Provider = "sqlite", ConnectionString = "Data Source=:memory:" };
      var field = new Field { Name = "Name", Alias = "Name", Type = "string" };
      var filter = new Filter {
         Field = field.Name,
         Value = value,
         Operator = "equal",
         WildCard = "*",
         IsField = true,
         LeftField = field
      };
      var entity = new Entity { Name = "People", Alias = "People", Input = "input", Fields = new() { field }, Filter = new() { filter } };
      var process = new Process { Name = "Test", Connections = new() { connection }, Entities = new() { entity } };
      return (new InputContext(new PipelineContext(new NullLogger(), process, entity)), new SqliteConnectionFactory(connection), field);
   }
}
