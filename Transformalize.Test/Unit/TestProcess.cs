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
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Main;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestProcess {

        [Test]
        public void TestDefaultProviders() {
            var process = new ProcessBuilder("p1").Process();

            Assert.IsNotNull(process);
            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual("p1Star", process.Star);
            Assert.AreEqual(3, process.Providers.Count);

        }

        [Test]
        public void TestConnection() {
            var process = new ProcessBuilder("p1")
                .Connection("input").Server("localhost").Database("Test")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(1, process.Connections.Count);
            Assert.AreEqual(500, process.Connections[0].BatchSize);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("localhost", process.Connections[0].Server);
            Assert.AreEqual("Test", process.Connections[0].Database);
        }

        [Test]
        public void TestTwoConnections() {
            var process = new ProcessBuilder("p1")
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
        public void TestMap() {
            var process = new ProcessBuilder("p1")
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
        public void TestEntity() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail").Version("OrderDetailVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("Order").Version("OrderVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("OrderDate").Type("System.DateTime").Default("9999-12-31")
                .Process();

            Assert.AreEqual(2, process.Entities.Count);

            var orderDetail = process.Entities[0];

            Assert.AreEqual(2, orderDetail.Fields.Count);

            Assert.AreEqual("OrderDetail", orderDetail.Name);
            Assert.AreEqual("OrderDetailVersion", orderDetail.Version);

            var orderId = orderDetail.Fields[0];
            Assert.AreEqual("OrderId", orderId.Name);
            Assert.AreEqual("int", orderId.Type);
            Assert.IsTrue(orderId.PrimaryKey);

            var productId = orderDetail.Fields[1];
            Assert.AreEqual("ProductId", productId.Name);
            Assert.AreEqual("int", productId.Type);
            Assert.IsTrue(productId.PrimaryKey);
            Assert.AreEqual("0", productId.Default);

            var order = process.Entities[1];

            Assert.AreEqual("Order", order.Name);
            Assert.AreEqual("OrderVersion", order.Version);

            orderId = order.Fields[0];
            Assert.AreEqual("OrderId", orderId.Name);
            Assert.AreEqual("int", orderId.Type);
            Assert.IsTrue(orderId.PrimaryKey);

            var orderDate = order.Fields[1];
            Assert.AreEqual("OrderDate", orderDate.Name);
            Assert.AreEqual("System.DateTime", orderDate.Type);
            Assert.IsFalse(orderDate.PrimaryKey);
            Assert.AreEqual("9999-12-31", orderDate.Default);

        }

        [Test]
        public void TestRelationship() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail").Version("OrderDetailVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("Order").Version("OrderVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("OrderDate").Type("System.DateTime").Default("9999-12-31")
                .Relationship()
                    .LeftEntity("OrderDetail").LeftField("OrderId")
                    .RightEntity("Order").RightField("OrderId")
                .Process();

            Assert.AreEqual(1, process.Relationships.Count);

            var relationship = process.Relationships[0];
            Assert.AreEqual("OrderDetail", relationship.LeftEntity);
            Assert.AreEqual("OrderId", relationship.LeftField);
            Assert.AreEqual("Order", relationship.RightEntity);
            Assert.AreEqual("OrderId", relationship.RightField);
        }

        [Test]
        public void TestJoin() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("OrderDetailOptions")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                    .Field("Color").Default("Silver")
                .Relationship().LeftEntity("OrderDetail").RightEntity("Order")
                    .Join().LeftField("OrderId").RightField("OrderId")
                    .Join().LeftField("ProductId").RightField("ProductId")
                .Process();

            Assert.AreEqual(1, process.Relationships.Count);

            var relationship = process.Relationships[0];
            Assert.AreEqual("OrderDetail", relationship.LeftEntity);
            Assert.AreEqual("OrderId", relationship.Join[0].LeftField);
            Assert.AreEqual("Order", relationship.RightEntity);
            Assert.AreEqual("OrderId", relationship.Join[0].RightField);
        }

        [Test]
        public void TestFieldTransform() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("Something1")
                        .Transform().Method("right").Length(4)
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                    .Field("Something2")
                        .Transform().Method("left").Length(5)
                        .Transform().Method("trim").TrimChars("x")
                .Process();

            Assert.AreEqual(0, process.Entities[0].Fields[0].Transforms.Count);
            Assert.AreEqual(1, process.Entities[0].Fields[1].Transforms.Count);
            Assert.AreEqual(0, process.Entities[0].Fields[2].Transforms.Count);
            Assert.AreEqual(2, process.Entities[0].Fields[3].Transforms.Count);

            var left = process.Entities[0].Fields[1].Transforms[0];
            Assert.AreEqual("right", left.Method);
            Assert.AreEqual(4, left.Length);

            var right = process.Entities[0].Fields[3].Transforms[0];
            Assert.AreEqual("left", right.Method);
            Assert.AreEqual(5, right.Length);

            var trim = process.Entities[0].Fields[3].Transforms[1];
            Assert.AreEqual("trim", trim.Method);
            Assert.AreEqual("x", trim.TrimChars);
        }

    }
}