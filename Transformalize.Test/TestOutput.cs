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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Test.Builders;

namespace Transformalize.Test {

    [TestFixture]
    public class TestOutput {

        [Test]
        public void TestPrimaryOutput() {

            var input = new RowsBuilder()
                .Row("id", 1).Field("name", "one")
                .Row("id", 2).Field("name", "two")
                .Row("id", 3).Field("name", "three")
                .ToOperation();

            var cfg = new ProcessBuilder("process")

                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")

                .Entity("entity")
                    .InputOperation(input)
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            var output = process.Execute();

            Assert.IsInstanceOf<Dictionary<string, IEnumerable<Row>>>(output);
            Assert.AreEqual(3, output["entity"].Count());
        }

        [Test]
        public void TestSecondaryOutputs() {

            var input = new RowsBuilder()
                .Row("id", 1).Field("name", "one")
                .Row("id", 2).Field("name", "two")
                .Row("id", 3).Field("name", "three")
                .ToOperation();

            var cfg = new ProcessBuilder("process")
                
                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")

                .Connection("c1").Provider("internal")
                .Connection("c2").Provider("internal")

                .Entity("entity")
                    .InputOperation(input)
                    .Output("o1", "c1")
                        .RunField("name")
                        .RunValue("two")
                    .Output("o2", "c2")
                        .RunField("id")
                        .RunOperator("GreaterThan")
                        .RunType("int")
                        .RunValue(1)
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            var output = process.Execute();

            Assert.IsNotNull(output);

            Assert.AreEqual(3, output["entity"].Count());
            Assert.AreEqual(1, process["entity"].InternalOutput["o1"].Count());
            Assert.AreEqual(2, process["entity"].InternalOutput["o2"].Count());
        }

    }
}