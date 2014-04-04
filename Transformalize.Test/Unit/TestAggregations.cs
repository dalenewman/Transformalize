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

using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Test.Unit.Builders;

namespace Transformalize.Test.Unit {

    [TestFixture]
    public class TestAggregations {

        [Test]
        public void TestCounts() {

            var input = new RowsBuilder()
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2001")
                .Row("order", 1).Field("year", "2002")
                .Row("order", 2).Field("year", "2000")
                .Row("order", 2).Field("year", "2000")
                .ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Group() //group means you need to aggregate all output fields
                    .Field("order")             .Aggregate("group")         .Int32().PrimaryKey()
                    .Field("year")              .Aggregate("countdistinct") .Int32().Alias("years")
                    .Field("year")              .Aggregate("max")
                    .CalculatedField("count")   .Aggregate("count")         .Int32()
                .Process();

            var output = ProcessFactory.Create(cfg)[0].Run()["entity"].ToList();

            Assert.AreEqual(2, output.Count);

            var r1 = output[0];
            Assert.AreEqual("2002", r1["year"]);
            Assert.AreEqual(1, r1["order"]);
            Assert.AreEqual(5, r1["count"]);
            Assert.AreEqual(3, r1["years"]);

            var r2 = output[1];
            Assert.AreEqual("2000", r2["year"]);
            Assert.AreEqual(2, r2["order"]);
            Assert.AreEqual(2, r2["count"]);
            Assert.AreEqual(1, r2["years"]);
        }

        [Test]
        public void TestCountsNew() {

            var input = new RowsBuilder()
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2000")
                .Row("order", 1).Field("year", "2001")
                .Row("order", 1).Field("year", "2002")
                .Row("order", 2).Field("year", "2000")
                .Row("order", 2).Field("year", "2000")
                .ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Field("order").Int32().PrimaryKey()
                    .Field("year")
                .Process();

            var output = ProcessFactory.Create(cfg)[0].Run()["entity"].ToList();

            //Assert.AreEqual(2, output.Count);

            //var r1 = output[0];
            //Assert.AreEqual("2002", r1["year"]);
            //Assert.AreEqual(1, r1["order"]);
            //Assert.AreEqual(5, r1["count"]);
            //Assert.AreEqual(3, r1["years"]);

            //var r2 = output[1];
            //Assert.AreEqual("2000", r2["year"]);
            //Assert.AreEqual(2, r2["order"]);
            //Assert.AreEqual(2, r2["count"]);
            //Assert.AreEqual(1, r2["years"]);
        }


    }
}