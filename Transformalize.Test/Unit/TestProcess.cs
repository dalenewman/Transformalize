#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Main;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestProcess {

        [Test]
        public void TestDefaultProviders() {
            var builder = new ProcessBuilder("p1");
            var process = builder.Process();

            Assert.IsNotNull(process);
            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual("p1Star", process.Star);
            Assert.AreEqual(3, process.Providers.Count);
            
        }

        [Test]
        public void TestConnection() {
            var builder = new ProcessBuilder("p1");
            var process = builder.Connection("input").Server("localhost").Database("Test").Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(1, process.Connections.Count);
            Assert.AreEqual(500, process.Connections[0].BatchSize);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("localhost", process.Connections[0].Server);
            Assert.AreEqual("Test", process.Connections[0].Database);
        }

        [Test]
        public void TestTwoConnections() {
            var builder = new ProcessBuilder("p1");
            var process = builder
                .Connection("input").Database("I")
                .Connection("output").Database("O")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(2, process.Connections.Count);

            Assert.AreEqual(500, process.Connections[0].BatchSize);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("localhost", process.Connections[0].Server);
            Assert.AreEqual("I", process.Connections[0].Database);

            Assert.AreEqual(500, process.Connections[1].BatchSize);
            Assert.AreEqual("output", process.Connections[1].Name);
            Assert.AreEqual("localhost", process.Connections[1].Server);
            Assert.AreEqual("O", process.Connections[1].Database);

        }

        [Test]
        public void TestMap()
        {
            var builder = new ProcessBuilder("p1");
            var process = builder
                .Connection("input").Database("I")
                .Map("m1")
                    .Item().From("one").To("1")
                    .Item().From("two").To("2").StartsWith()
                .Map("m2", "SELECT [From], [To] FROM [Table];").Connection("input")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(2, process.Maps.Count);

            Assert.AreEqual(2, process.Maps[0].Items.Count);
            Assert.AreEqual("one", process.Maps[0].Items[0].From);
            Assert.AreEqual("1", process.Maps[0].Items[0].To);
            Assert.AreEqual("equals", process.Maps[0].Items[0].Operator);
            Assert.AreEqual("two", process.Maps[0].Items[1].From);
            Assert.AreEqual("2", process.Maps[0].Items[1].To);
            Assert.AreEqual("startswith", process.Maps[0].Items[1].Operator);

            Assert.AreEqual("input", process.Maps[1].Connection);
            Assert.AreEqual("SELECT [From], [To] FROM [Table];", process.Maps[1].Items.Sql);
        }

        [Test]
        public void TestEntity()
        {
            var builder = new ProcessBuilder("p1");
            var process = builder
                .Entity("e1").Version("v1")
                    .Field("f1").Alias("a1").Type("int").Default("2")
                    .Field("f2").Alias("a2").Default(".")
                .Entity("e2").Version("v2")
                    .Field("f1").Alias("a1").Type("int").Default("2")
                    .Field("f2").Alias("a2").Default(".")
                .Process();

            Assert.AreEqual(2, process.Entities.Count);
            Assert.AreEqual(2, process.Entities[0].Fields.Count);
                                  
        }


    }
}