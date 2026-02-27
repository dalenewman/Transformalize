using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Transform.GoogleMaps;

namespace Test {
   [TestClass]
   public class UnitTest1 {

      public const string GoogleKey = "<key>";

      [TestMethod]
      public void UsefulTest(){
         Assert.AreEqual(1,1, "Try this for Github Actions");
      }

      [Ignore]
      [TestMethod]
      public void TestGeoCodeWithAddress() {

         var logger = new MemoryLogger(LogLevel.Debug);
         var process = GetTestProcess("google-geocode");

         if (process.Errors().Any()) {
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            throw new Exception("The configuration has errors");
         }

         var input = new MasterRow(process.GetAllFields().Count());

         var address = process.Entities.First().Fields.First(f => f.Name == "Address");
         var latitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Latitude");
         var longitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Longitude");
         var formattedAddress = process.Entities.First().CalculatedFields.First(f => f.Name == "FormattedAddress");
         var state = process.Entities.First().CalculatedFields.First(f => f.Name == "State");
         var county = process.Entities.First().CalculatedFields.First(f => f.Name == "County");
         var partialMatch = process.Entities.First().CalculatedFields.First(f => f.Name == "PartialMatch");
         var locality = process.Entities.First().CalculatedFields.First(f => f.Name == "Locality");

         input[address] = "1009 Broad Street St. Joseph MI 49085";
         input[latitude] = null;
         input[longitude] = null;
         input[formattedAddress] = null;

         var context = new PipelineContext(logger, process, process.Entities.First(), address, address.Transforms.First());

         using (var gt = new GeocodeTransform(context)) {
            var output = gt.Operate(input);

            Assert.AreEqual("1009 Broad Street St. Joseph MI 49085", output[address]);
            Assert.AreEqual("MI", output[state]);
            Assert.AreEqual(false, output[partialMatch]);
            Assert.AreEqual("Berrien County", output[county]);
            Assert.AreEqual("St Joseph", output[locality]);
            Assert.IsNotNull(output[latitude]);
            Assert.IsNotNull(output[longitude]);
            Assert.IsNotNull(output[formattedAddress]);
         }

      }

      [TestMethod]
      [Ignore]
      public void TestPlace() {

         var logger = new MemoryLogger(LogLevel.Debug);
         var process = GetTestProcess("google-place");

         if (process.Errors().Any()) {
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            throw new Exception("The configuration has errors");
         }

         var input = new MasterRow(process.GetAllFields().Count());

         var address = process.Entities.First().Fields.First(f => f.Name == "Address");
         var latitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Latitude");
         var longitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Longitude");
         var formattedAddress = process.Entities.First().CalculatedFields.First(f => f.Name == "FormattedAddress");
         var place = process.Entities.First().CalculatedFields.First(f => f.Name == "PlaceId");

         input[address] = "1009 Broad St Joseph Michigan 49085";
         input[latitude] = null;
         input[longitude] = null;
         input[formattedAddress] = null;

         var context = new PipelineContext(logger, process, process.Entities.First(), address, address.Transforms.First());
         using (var gt = new PlaceTransform(context)) {
            var output = gt.Operate(input);

            Assert.AreEqual("1009 Broad St Joseph 49085", output[address]);
            Assert.AreEqual("ChIJsxyoG5_GEIgRMN8IWvngddA", output[place]);
            Assert.IsNotNull(output[latitude]);
            Assert.IsNotNull(output[longitude]);
            Assert.IsNotNull(output[formattedAddress]);
         }

      }

      [TestMethod]
      [Ignore]
      public void TestGeoCodeWithPlaceId() {

         var logger = new MemoryLogger(LogLevel.Debug);
         var process = GetTestProcess("google-geocode");

         if (process.Errors().Any()) {
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            throw new Exception("The configuration has errors");
         }

         var input = new MasterRow(process.GetAllFields().Count());

         var address = process.Entities.First().Fields.First(f => f.Name == "Address");
         var latitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Latitude");
         var longitude = process.Entities.First().CalculatedFields.First(f => f.Name == "Longitude");
         var formattedAddress = process.Entities.First().CalculatedFields.First(f => f.Name == "FormattedAddress");

         input[address] = "ChIJsxyoG5_GEIgRMN8IWvngddA";
         input[latitude] = null;
         input[longitude] = null;
         input[formattedAddress] = null;

         var context = new PipelineContext(logger, process, process.Entities.First(), address, address.Transforms.First());
         using (var gt = new GeocodeTransform(context)) {
            var output = gt.Operate(input);

            Assert.AreEqual("ChIJsxyoG5_GEIgRMN8IWvngddA", output[address]);
            Assert.IsNotNull(output[latitude]);
            Assert.IsNotNull(output[longitude]);
            Assert.IsNotNull(output[formattedAddress]);
         }

      }

      private static Process GetTestProcess(string method) {
         var process = new Process {
            Name = "Test",
            ReadOnly = true,
            Entities = new List<Entity>(1) {
               new Entity {
                  Name = "Test",
                  Alias = "Test",
                  Fields = new List<Field> {
                        new Field {
                           Name = "Address",
                           Alias = "Address",
                           Transforms = new List<Operation>(1){
                              new Operation {
                                    Method = method,
                                    ApiKey = GoogleKey,
                                    Fields = new List<Field> {
                                       new Field { Name = "Latitude", Type = "double" },
                                       new Field { Name = "Longitude", Type = "double" },
                                       new Field { Name = "FormattedAddress", Type = "string", Length = "128" },
                                       new Field { Name = "PlaceId" },
                                       new Field { Name = "LocationType" },
                                       new Field { Name = "State", Length="2" },
                                       new Field { Name = "County" },
                                       new Field { Name = "Zip", Length="10" },
                                       new Field { Name = "PartialMatch", Type = "bool" },
                                       new Field { Name = "Locality" }
                                    }
                              }
                           }
                        }
                  }
               }
            }
         };

         process.Load();

         return process;
      }
   }
}
