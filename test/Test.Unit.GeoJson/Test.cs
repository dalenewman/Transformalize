#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using System.Text.Json;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.GeoJson.Autofac;

namespace Test {

   [TestClass]
   public class Test {

      [TestMethod]
      public void Write() {
         const string xml = @"<add name='Test' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' use='legacy' file='bogus.geo.json' />
  </connections>
  <entities>
    <add name='Contact' size='2'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' />
        <add name='Color' type='string' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new GeoJsonProviderModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               var expected = "{\"type\":\"FeatureCollection\",\"features\":[{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[-74.145,49.3317]},\"properties\":{\"Identity\":1,\"FirstName\":\"Justin\",\"LastName\":\"Konopelski\",\"description\":\"<table class=\\\"table table-striped table-condensed\\\">\\n<tr>\\n<td><strong>\\nIdentity\\n:</strong></td>\\n<td>\\n1\\n</td>\\n</tr>\\n<tr>\\n<td><strong>\\nFirstName\\n:</strong></td>\\n<td>\\nJustin\\n</td>\\n</tr>\\n<tr>\\n<td><strong>\\nLastName\\n:</strong></td>\\n<td>\\nKonopelski\\n</td>\\n</tr>\\n</table>\\n\",\"marker-color\":\"#661c0c\"}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[-117.9625,49.2812]},\"properties\":{\"Identity\":2,\"FirstName\":\"Duane\",\"LastName\":\"Will\",\"description\":\"<table class=\\\"table table-striped table-condensed\\\">\\n<tr>\\n<td><strong>\\nIdentity\\n:</strong></td>\\n<td>\\n2\\n</td>\\n</tr>\\n<tr>\\n<td><strong>\\nFirstName\\n:</strong></td>\\n<td>\\nDuane\\n</td>\\n</tr>\\n<tr>\\n<td><strong>\\nLastName\\n:</strong></td>\\n<td>\\nWill\\n</td>\\n</tr>\\n</table>\\n\",\"marker-color\":\"#6c5975\"}}]}";
               var actual = File.ReadAllText("bogus.geo.json");
               // Normalize newline escape sequences so the test passes on Windows and Unix-like systems
               actual = actual.Replace("\\r\\n", "\\n");

               Assert.AreEqual((uint)2, process.Entities.First().Inserts);
               Assert.AreEqual(expected, actual);
            }
         }
      }

      [TestMethod]
      public void WriteRoleModelWithBBox() {
         const string xml = @"<add name='Test' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' use='geo' file='bogus-role.geo.json' min-lat='24.0' min-lon='-125.0' max-lat='50.0' max-lon='-66.0' />
  </connections>
  <entities>
    <add name='Contact' size='2'>
      <fields>
        <add name='Identity' type='int' geo='id' />
        <add name='FirstName' geo='property' />
        <add name='IgnoreMe' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' geo='latitude' />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' geo='longitude' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new GeoJsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               var actual = File.ReadAllText("bogus-role.geo.json");
               using var doc = JsonDocument.Parse(actual);
               var root = doc.RootElement;

               Assert.AreEqual("FeatureCollection", root.GetProperty("type").GetString());

               var bbox = root.GetProperty("bbox").EnumerateArray().Select(x => x.GetDouble()).ToArray();
               CollectionAssert.AreEqual(new[] { -125d, 24d, -66d, 50d }, bbox);

               var features = root.GetProperty("features").EnumerateArray().ToArray();
               Assert.AreEqual(2, features.Length);

               for (var i = 0; i < features.Length; i++) {
                  var feature = features[i];
                  var geometry = feature.GetProperty("geometry");
                  Assert.AreEqual("Point", geometry.GetProperty("type").GetString());
                  var coordinates = geometry.GetProperty("coordinates").EnumerateArray().ToArray();
                  Assert.AreEqual(2, coordinates.Length);
                  var id = feature.GetProperty("id");
                  Assert.AreEqual(i + 1, id.GetInt32());

                  var properties = feature.GetProperty("properties");
                  Assert.IsFalse(properties.TryGetProperty("Identity", out _));
                  Assert.IsTrue(properties.TryGetProperty("FirstName", out _));
                  Assert.IsFalse(properties.TryGetProperty("IgnoreMe", out _));
               }

               Assert.AreEqual((uint)2, process.Entities.First().Inserts);
            }
         }
      }

      [TestMethod]
      public void WriteDefaultsToLegacyType() {
         const string xml = @"<add name='Test' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' file='bogus-legacy.geo.json' />
  </connections>
  <entities>
    <add name='Contact' size='1'>
      <fields>
        <add name='Identity' type='int' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' />
        <add name='Color' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new GeoJsonProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               var actual = File.ReadAllText("bogus-legacy.geo.json");
               using var doc = JsonDocument.Parse(actual);
               var properties = doc.RootElement.GetProperty("features")[0].GetProperty("properties");
               Assert.AreEqual("<table", properties.GetProperty("description").GetString()?.Substring(0,6));
               Assert.IsTrue(properties.TryGetProperty("marker-color", out _));
               Assert.AreEqual((uint)1, process.Entities.First().Inserts);
            }
         }
      }

   }
}
