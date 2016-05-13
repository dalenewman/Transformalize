#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Core.Activators.Reflection;
using Cfg.Net.Loggers;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using Nest;
using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Scripting.Jint;

namespace Pipeline.Test {

    [TestFixture]
    public class Configuration {

        [Test(Description = @"Cfg-Net can read Files\PersonAndPet.xml")]
        public void ConfurationIsGood() {
            var composer = new CompositionRoot();
            var controller = composer.Compose(@"Files\PersonAndPet.xml");

            Assert.AreEqual(0, composer.Process.Errors().Count());
            Assert.AreEqual(1, composer.Process.Warnings().Count());
        }


        [Test(Description = "Process populates index for each field")]
        public void FieldsAreIndexed() {

            var composer = new CompositionRoot();
            composer.Compose(@"Files\PersonAndPet.xml");

            var pet = composer.Process.Entities.First();
            var person = composer.Process.Entities.Last();

            // system fields
            Assert.AreEqual(0, pet.Fields[0].Index);
            Assert.AreEqual(1, pet.Fields[1].Index);
            Assert.AreEqual(2, pet.Fields[2].Index);
            Assert.AreEqual(3, pet.Fields[3].Index);

            // fields
            Assert.AreEqual(4, pet.Fields[4].Index);
            Assert.AreEqual(5, pet.Fields[5].Index);
            Assert.AreEqual(6, pet.Fields[6].Index);
            Assert.AreEqual(7, pet.Fields[7].Index);
            Assert.AreEqual(8, pet.Fields[8].Index);

            // calc fields produced from xml
            Assert.AreEqual(9, pet.CalculatedFields[0].Index);
            Assert.AreEqual(10, pet.CalculatedFields[1].Index);


            Assert.AreEqual("Id", person.Fields[4].Name);

            // system fields
            Assert.AreEqual(0, person.Fields[0].Index);
            Assert.AreEqual(1, person.Fields[1].Index);
            Assert.AreEqual(2, person.Fields[2].Index);
            Assert.AreEqual(3, person.Fields[3].Index);

            // fields
            Assert.AreEqual(4, person.Fields[4].Index);
            Assert.AreEqual(5, person.Fields[5].Index);
            Assert.AreEqual(6, person.Fields[6].Index);
            Assert.AreEqual(7, person.Fields[7].Index);

            Assert.AreEqual(8, person.CalculatedFields[0].Index);

        }

        [Test(Description = "Process populates key types")]
        public void KeysTypesSet() {

            var parameters = new Dictionary<string, string>();
            var cfg = new FileReader().Read(@"Files\PersonAndPet.xml", parameters, new NullLogger());
            var sh = new ShorthandRoot(@"Files\Shorthand.xml", new FileReader());

            var process = new Process(cfg, new JintValidator("js"), new ShorthandModifier(sh, "sh"));

            foreach (var error in process.Errors()) {
                Console.WriteLine(error);
            }

            foreach (var warning in process.Warnings()) {
                Console.WriteLine(warning);
            }

            var pet = process.Entities.First();
            Assert.AreEqual(KeyType.Primary, pet.Fields[4].KeyType);
            Assert.AreEqual(KeyType.None, pet.Fields[5].KeyType);
            Assert.IsTrue(pet.Fields[8].KeyType.HasFlag(KeyType.Foreign));

            var person = process.Entities.Last();
            Assert.IsTrue(person.Fields[4].KeyType == KeyType.Primary, "Person Id is a primary key on Person table.");
            Assert.IsTrue(person.Fields.Where(f => f.Name != "Id").All(f => f.KeyType == KeyType.None), "All other Person fields are KeyType.None.");

        }

        [Test]
        public void TestRelationshipToMaster() {
            var cfg = new FileReader().Read(@"Files\PersonAndPet.xml", null, new NullLogger());
            var sh = new ShorthandRoot(@"Files\Shorthand.xml", new FileReader());
            var process = new Process(cfg, new JintValidator("js"), new ShorthandModifier(sh, "sh"));
            var rtm = process.Entities[1].RelationshipToMaster;

            Assert.AreEqual(1, rtm.Count());

        }

    }
}
