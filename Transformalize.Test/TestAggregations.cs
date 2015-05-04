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

using System;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Test.Builders;

namespace Transformalize.Test {

    [TestFixture]
    public class TestAggregations {

        private readonly IOperation _testInput = new RowsBuilder()
            .Row("order", 1).Field("year", "2000").Field("int32", 2000).Field("int64", null).Field("guid", Guid.Parse("e2cf0d95-1d29-4bed-9b31-ce5b5628466b")).Field("bytes", new byte[] { 0, 0, 0 })
            .Row("order", 1).Field("year", "2000").Field("int32", 2000).Field("int64", (Int64)2000).Field("guid", Guid.Parse("c4eb4889-b3ff-47ed-8b7a-7f0374fac7f6")).Field("bytes", new byte[] { 0, 0, 0 })
            .Row("order", 1).Field("year", "2000").Field("int32", 2000).Field("int64", (Int64)2000).Field("guid", null).Field("bytes", new byte[] { 0, 0, 0 })
            .Row("order", 1).Field("year", "2001").Field("int32", 2001).Field("int64", (Int64)2001).Field("guid", null).Field("bytes", new byte[] { 0, 0, 1 })
            .Row("order", 1).Field("year", "2002").Field("int32", 2002).Field("int64", (Int64)2002).Field("guid", null).Field("bytes", new byte[] { 0, 0, 2 })
            .Row("order", 2).Field("year", "2000").Field("int32", 2000).Field("int64", (Int64)2000).Field("guid", Guid.Parse("e2cf0d95-1d29-4bed-9b31-ce5b5628466b")).Field("bytes", new byte[] { 0, 0, 0 })
            .Row("order", 2).Field("year", "2000").Field("int32", 2000).Field("int64", (Int64)2000).Field("guid", Guid.Parse("cd5cb6c8-4cbe-40c5-b17d-c17aee4196a2")).Field("bytes", new byte[] { 0, 0, 0 })
            .ToOperation();

        [Test]
        public void TestCount() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("count").Input(false).Aggregate("count").Int32()
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(5, r1["count"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(2, r2["count"]);
        }

        [Test]
        public void TestMax() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("max")
                    .Field("int32").Aggregate("max").Int32()
                    .Field("int64").Aggregate("max").Int64()
                    .Field("guid").Aggregate("max").Type("guid")
                    .Field("bytes").Aggregate("max").Type("byte[]")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2002", r1["year"]);
            Assert.AreEqual(2002, r1["int32"]);
            Assert.AreEqual(2002, r1["int64"]);
            Assert.AreEqual(Guid.Parse("e2cf0d95-1d29-4bed-9b31-ce5b5628466b"), r1["guid"]);
            Assert.AreEqual(new Byte[] { 0, 0, 2 }, r1["bytes"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000", r2["year"]);
            Assert.AreEqual(2000, r2["int32"]);
            Assert.AreEqual(2000, r2["int64"]);
            Assert.AreEqual(Guid.Parse("e2cf0d95-1d29-4bed-9b31-ce5b5628466b"), r2["guid"]);
            Assert.AreEqual(new Byte[] { 0, 0, 0 }, r2["bytes"]);
        }

        [Test]
        public void TestMaxLength() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("maxlength")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg,new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(4, r1["year"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(4, r2["year"]);
        }

        [Test]
        public void TestMinLength() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("minlength")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(4, r1["year"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(4, r2["year"]);
        }

        [Test]
        public void TestSum() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("int32").Aggregate("sum").Int32()
                    .Field("int64").Aggregate("sum").Int64()
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(10003, r1["int32"]);
            Assert.AreEqual(8003, r1["int64"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(4000, r2["int32"]);
            Assert.AreEqual(4000, r2["int64"]);
        }

        [Test]
        public void TestMin() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("min")
                    .Field("int32").Aggregate("min").Int32()
                    .Field("int64").Aggregate("min").Int64()
                    .Field("guid").Aggregate("min").Type("guid")
                    .Field("bytes").Aggregate("min").Type("byte[]")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2000", r1["year"]);
            Assert.AreEqual(2000, r1["int32"]);
            Assert.AreEqual(0, r1["int64"]); //because of null
            Assert.AreEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"), r1["guid"]); // because of null
            Assert.AreEqual(new Byte[] { 0, 0, 0 }, r1["bytes"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000", r2["year"]);
            Assert.AreEqual(2000, r2["int32"]);
            Assert.AreEqual(2000, r2["int64"]);
            Assert.AreEqual(Guid.Parse("cd5cb6c8-4cbe-40c5-b17d-c17aee4196a2"), r2["guid"]);
            Assert.AreEqual(new Byte[] { 0, 0, 0 }, r2["bytes"]);
        }

        [Test]
        public void TestLast() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("last")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2002", r1["year"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000", r2["year"]);
        }

        [Test]
        public void TestFirst() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("first")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2000", r1["year"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000", r2["year"]);
        }

        [Test]
        public void TestCountDistinct() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Aggregate("count").Distinct().Int32().Alias("years")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg,new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(3, r1["years"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(1, r2["years"]);
        }

        [Test]
        public void TestJoin() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Alias("years").Aggregate("join")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2000, 2000, 2000, 2001, 2002", r1["years"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000, 2000", r2["years"]);
        }

        [Test]
        public void TestJoinDistinct() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Alias("years").Aggregate("join").Distinct()
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual("2000, 2001, 2002", r1["years"]);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual("2000", r2["years"]);
        }

        [Test]
        public void TestArray() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Alias("years").Aggregate("array")
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(5, ((object[])r1["years"]).Length);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(2, ((object[])r2["years"]).Length);
        }

        [Test]
        public void TestArrayDistinct() {

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .Group() //group means you need to aggregate all output fields
                    .Field("order").Aggregate("group").Int32().PrimaryKey()
                    .Field("year").Alias("years").Aggregate("array").Distinct()
                .Process();

            cfg.Entities.First().InputOperation = _testInput;

            var output = ProcessFactory.CreateSingle(cfg, new TestLogger()).Execute().ToArray();

            Assert.AreEqual(2, output.Length);

            var r1 = output[0];
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(3, ((object[])r1["years"]).Length);

            var r2 = output[1];
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(1, ((object[])r2["years"]).Length);
        }

    }
}