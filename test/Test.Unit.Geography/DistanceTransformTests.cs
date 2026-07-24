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
using System.Linq;
using Autofac;
using Geolocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Transforms.Geography;

namespace Tests {

   [TestClass]
   public class DistanceTransformTests {

      // Two arbitrary coordinates (Seattle -> Portland); the tests below assert
      // relationships between units and precision rather than hard-coded geographic constants.
      private const double FromLat = 47.6062;
      private const double FromLon = -122.3321;
      private const double ToLat = 45.5152;
      private const double ToLon = -122.6784;

      [TestMethod]
      public void SamePoint_IsZero() {
         Assert.AreEqual(0d, DistanceTransform.Get(FromLat, FromLon, FromLat, FromLon));
      }

      [TestMethod]
      public void Miles_IsTheDefaultUnit() {
         var miles = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6);
         var explicitMiles = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Miles);
         Assert.AreEqual(explicitMiles, miles);
         Assert.IsTrue(miles > 0);
      }

      [TestMethod]
      public void Kilometers_AreMilesTimesConversionFactor() {
         var miles = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Miles);
         var kilometers = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Kilometers);
         Assert.AreEqual(1.60934, kilometers / miles, 0.01);
      }

      [TestMethod]
      public void NauticalMiles_RelateToMilesByConversionFactor() {
         var miles = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Miles);
         var nautical = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.NauticalMiles);
         Assert.AreEqual(1.15078, miles / nautical, 0.01);
      }

      [TestMethod]
      public void Meters_AreKilometersTimesOneThousand() {
         var kilometers = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Kilometers);
         var meters = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 6, DistanceUnit.Meters);
         Assert.AreEqual(1000d, meters / kilometers, 1d);
      }

      [TestMethod]
      public void DecimalPlaces_ControlRounding() {
         var whole = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 0, DistanceUnit.Miles);
         Assert.AreEqual(whole, System.Math.Round(whole), "decimalPlaces:0 should return a whole number");

         var oneDecimal = DistanceTransform.Get(FromLat, FromLon, ToLat, ToLon, 1, DistanceUnit.Miles);
         Assert.AreEqual(oneDecimal, System.Math.Round(oneDecimal, 1));
      }

      [TestMethod]
      public void DistanceUnitAndDecimalPlaces_FlowThroughConfiguration() {

         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add FromLat='47.6062' FromLon='-122.3321' ToLat='45.5152' ToLon='-122.6784' />
          </rows>
          <fields>
            <add name='FromLat' type='double' />
            <add name='FromLon' type='double' />
            <add name='ToLat' type='double' />
            <add name='ToLon' type='double' />
          </fields>
          <calculated-fields>
            <add name='Miles' type='double' t='distance(FromLat,FromLon,ToLat,ToLon)' />
            <add name='Kilometers' type='double' t='distance(FromLat,FromLon,ToLat,ToLon,6,kilometers)' />
            <add name='Meters' type='double' t='distance(FromLat,FromLon,ToLat,ToLon,6,meters)' />
          </calculated-fields>
        </add>
      </entities>
    </add>
            ".Replace('\'', '"');

         var transform = new TransformHolder((c) => new DistanceTransform(c), new DistanceTransform().GetSignatures());

         using (var outer = new ConfigurationContainer(transform).CreateScope(xml, new DebugLogger())) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(transform).CreateScope(process, new DebugLogger())) {
               var output = inner.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();

               var miles = (double)output[0][cf[0]];
               var kilometers = (double)output[0][cf[1]];
               var meters = (double)output[0][cf[2]];

               Assert.IsTrue(miles > 0, "expected a positive distance in miles");

               // default (miles, 1 decimal) rounds to a single decimal place
               Assert.AreEqual(miles, System.Math.Round(miles, 1));

               // distance-unit=kilometers is larger than miles by the ~1.60934 factor
               Assert.AreEqual(1.60934, kilometers / miles, 0.02);

               // distance-unit=meters is kilometers * 1000
               Assert.AreEqual(1000d, meters / kilometers, 1d);
            }
         }
      }
   }
}
