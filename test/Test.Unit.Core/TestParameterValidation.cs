using System.Xml.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Logging;

namespace Tests {
   [TestClass]
   public class TestParameterValidation {
      [TestMethod]
      [DataRow("O'Brien")]
      [DataRow("one;two")]
      [DataRow("`quoted`")]
      [DataRow("missing'; DROP TABLE People; --")]
      public void StringParameterPreservesPunctuation(string value) {
         var xml = new XElement("cfg", new XAttribute("name", "ParameterValidation"),
            new XElement("parameters", new XElement("add",
               new XAttribute("name", "text"), new XAttribute("value", value)))).ToString();

         using var process = new Process(xml);

         Assert.AreEqual(0, process.Errors().Length, string.Join(Environment.NewLine, process.Errors()));
         Assert.AreEqual(value, process.Parameters.Single().Value);
      }

      [TestMethod]
      public void CallerSuppliedParameterPreservesPunctuation() {
         const string value = "O'Brien; `quoted`";
         const string xml = "<cfg name='ParameterValidation'><parameters><add name='text' value='' /></parameters></cfg>";
         using var scope = new ConfigurationContainer().CreateScope(xml, new NullLogger(),
            new Dictionary<string, string> { ["text"] = value });
         var process = scope.Resolve<Process>();

         Assert.AreEqual(0, process.Errors().Length, string.Join(Environment.NewLine, process.Errors()));
         Assert.AreEqual(value, process.Parameters.Single().Value);
      }

      [TestMethod]
      public void ConfiguredParameterPreservesPunctuationThroughTransforms() {
         const string value = "O'Brien; `quoted`";
         var xml = new XElement("cfg", new XAttribute("name", "ParameterValidation"),
            new XElement("parameters", new XElement("add",
               new XAttribute("name", "text"), new XAttribute("value", value),
               new XAttribute("t", "append(!)")))).ToString();
         using var scope = new ConfigurationContainer().CreateScope(xml, new NullLogger());
         var process = scope.Resolve<Process>();

         Assert.AreEqual(0, process.Errors().Length, string.Join(Environment.NewLine, process.Errors()));
         Assert.AreEqual(value + "!", process.Parameters.Single().Value);
      }

      [TestMethod]
      [DataRow(";", "one;two")]
      [DataRow("'", "O'Brien")]
      [DataRow("`", "`quoted`")]
      public void ExplicitInvalidCharactersStillRejectMatchingValues(string invalidCharacters, string value) {
         var xml = new XElement("cfg", new XAttribute("name", "ParameterValidation"),
            new XElement("parameters", new XElement("add",
               new XAttribute("name", "text"), new XAttribute("value", value),
               new XAttribute("invalid-characters", invalidCharacters)))).ToString();

         using var process = new Process(xml);

         CollectionAssert.Contains(process.Errors(), $"The text parameter contains an invalid '{invalidCharacters}' character");
      }

      [TestMethod]
      public void ExplicitInvalidCharactersSurviveParameterTransforms() {
         const string xml = "<cfg name='ParameterValidation'><parameters><add name='text' value='one;two' invalid-characters=';' t='append(!)' /></parameters></cfg>";
         using var scope = new ConfigurationContainer().CreateScope(xml, new NullLogger());
         var process = scope.Resolve<Process>();

         Assert.AreEqual(";", process.Parameters.Single().InvalidCharacters);
         Assert.AreEqual("one;two!", process.Parameters.Single().Value);
         CollectionAssert.Contains(process.Errors(), "The text parameter contains an invalid ';' character");
      }

      [TestMethod]
      [DataRow("input='false'", "one;two", ";")]
      [DataRow("multiple='true'", "one,two", ",")]
      public void ExplicitInvalidCharactersKeepExistingExceptions(string options, string value, string invalidCharacters) {
         using var process = new Process($"<cfg name='ParameterValidation'><parameters><add name='text' value='{value}' invalid-characters='{invalidCharacters}' {options} /></parameters></cfg>");

         Assert.AreEqual(0, process.Errors().Length, string.Join(Environment.NewLine, process.Errors()));
         Assert.AreEqual(value, process.Parameters.Single().Value);
      }

      [TestMethod]
      public void NonStringParameterStillRequiresCorrectType() {
         using var process = new Process("<cfg name='ParameterValidation'><parameters><add name='count' type='int' value='not-a-number' /></parameters></cfg>");

         CollectionAssert.Contains(process.Errors(), "The parameter count is supposed to be an int, but not-a-number can not be parsed as such.");
      }
   }
}
