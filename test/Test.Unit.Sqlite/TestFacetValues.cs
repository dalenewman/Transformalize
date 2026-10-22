using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Logging;
using Transformalize.Providers.Ado;
using Transformalize.Providers.SQLite;

namespace Test.Unit.Sqlite;

[TestClass]
public class TestFacetValues {
   private const string Supplier = "Cooperativa de Quesos 'Las Cabras'";
   private static readonly string[] Values = {
      Supplier, "O'Brien & Sons", "Actual ''double'' apostrophes", "missing' OR 1=1 --"
   };

   [TestMethod]
   [DataRow("sync", false)]
   [DataRow("async", false)]
   [DataRow("stream", false)]
   [DataRow("sync", true)]
   [DataRow("async", true)]
   [DataRow("stream", true)]
   public async Task FacetOptionsRoundTripWithoutChangingApostrophes(string mode, bool multiple) {
      using var database = new TestDatabase();
      var unfiltered = database.CreateContext("*", "facet", multiple);
      await Read(unfiltered, mode);
      var options = unfiltered.Process.Maps.Single().Items;
      CollectionAssert.AreEquivalent(Values, options.Select(item => (string)item.To).ToArray());

      foreach (var option in options) {
         // Submit the actual option value, as the report dropdown does.
         var selected = database.CreateContext((string)option.To, "facet", multiple);
         var rows = await Read(selected, mode);
         var expectedHits = option.To.Equals(Supplier) ? 3 : 1;
         Assert.AreEqual(expectedHits, selected.Entity.Hits);
         Assert.AreEqual(Math.Min(2, expectedHits), rows.Count);
         Assert.IsTrue(rows.All(row => Equals(option.To, row[selected.InputFields.Single()])));
         Assert.AreEqual(option.To, selected.Process.Maps.Single().Items.Single().To);
      }

      if (multiple) {
         var selected = database.CreateContext(string.Join(",", Values.Take(2)), "facet", true);
         var rows = await Read(selected, mode);
         Assert.AreEqual(4, selected.Entity.Hits);
         Assert.AreEqual(2, rows.Count);
         CollectionAssert.AreEquivalent(Values.Take(2).ToArray(), selected.Process.Maps.Single().Items.Select(item => (string)item.To).ToArray());
      }
   }

   [TestMethod]
   [DataRow("sync")]
   [DataRow("async")]
   [DataRow("stream")]
   public async Task SearchPreservesApostrophes(string mode) {
      using var database = new TestDatabase();
      var context = database.CreateContext("'Las Cabras'", "search", false);
      var rows = await Read(context, mode);
      Assert.AreEqual(3, context.Entity.Hits);
      Assert.AreEqual(2, rows.Count);
      Assert.IsTrue(rows.All(row => Equals(Supplier, row[context.InputFields.Single()])));
   }

   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task CustomMapQueryBindsOriginalParameterValue(bool async) {
      using var database = new TestDatabase();
      foreach (var value in Values) {
         var context = database.CreateContext("*", "facet", false);
         context.Process.Parameters.Single().Value = value;
         var map = context.Process.Maps.Single();
         map.Connection = "input";
         map.Query = "SELECT DISTINCT Name AS 'From', Name AS 'To' FROM People WHERE Name = @Name";
         var reader = new AdoMapReader(context, new SqliteConnectionFactory(context.Connection), map.Name);
         var items = async ? await reader.ReadAsync(context) : reader.Read(context).ToArray();
         var item = items.Single();
         Assert.AreEqual(value, item.From);
         Assert.AreEqual(value, item.To);
      }
   }

   private static async Task<List<IRow>> Read(InputContext context, string mode) {
      var reader = new AdoInputReader(context, context.InputFields, new SqliteConnectionFactory(context.Connection),
         new RowFactory(context.RowCapacity, true, false));
      if (mode == "sync") return reader.Read().ToList();
      if (mode == "async") return (await reader.ReadAsync()).ToList();
      var rows = new List<IRow>();
      await foreach (var row in reader.ReadStreamAsync()) rows.Add(row);
      return rows;
   }

   private sealed class TestDatabase : IDisposable {
      private readonly string _path = Path.Combine(Path.GetTempPath(), $"tfl-facet-{Guid.NewGuid():N}.db");

      public TestDatabase() {
         using var connection = new SqliteConnection($"Data Source={_path};Pooling=False");
         connection.Open();
         using var command = connection.CreateCommand();
         command.CommandText = "CREATE TABLE People (Name TEXT)";
         command.ExecuteNonQuery();
         command.CommandText = "INSERT INTO People VALUES (@Name)";
         command.Parameters.AddWithValue("@Name", "");
         foreach (var value in Values.Concat(new[] { Supplier, Supplier })) {
            command.Parameters["@Name"].Value = value;
            command.ExecuteNonQuery();
         }
      }

      public InputContext CreateContext(string value, string type, bool multiple) {
         var cfg = XElement.Parse("""
            <cfg name="facet" read-only="true">
              <connections><add name="input" provider="sqlite" /></connections>
              <parameters><add name="Name" value="*" prompt="true" map="names" /></parameters>
              <maps><add name="names" /></maps>
              <entities><add name="People" page="1" size="2">
                <fields><add name="Name" /></fields>
                <filter><add field="Name" map="names" /></filter>
              </add></entities>
            </cfg>
            """);
         var connection = cfg.Element("connections").Element("add");
         connection.SetAttributeValue("file", _path);
         connection.SetAttributeValue("connection-string", $"Data Source={_path};Pooling=False");
         var parameter = cfg.Element("parameters").Element("add");
         parameter.SetAttributeValue("multiple", multiple);
         parameter.SetAttributeValue("value", value);
         var filter = cfg.Element("entities").Element("add").Element("filter").Element("add");
         filter.SetAttributeValue("type", type);
         filter.SetAttributeValue("value", value);
         var process = new Process(cfg.ToString());
         Assert.AreEqual(0, process.Errors().Length, string.Join("; ", process.Errors()));
         return new InputContext(new PipelineContext(new NullLogger(), process, process.Entities.Single()));
      }

      public void Dispose() => File.Delete(_path);
   }
}
