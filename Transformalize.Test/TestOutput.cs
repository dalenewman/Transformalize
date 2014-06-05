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
using System.IO;
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
        public void TestSecondaryOutputs()
        {

            var file1 = Path.GetTempFileName();
            var file2 = Path.GetTempFileName();

            var input = new RowsBuilder()
                .Row("id", 1).Field("name", "one")
                .Row("id", 2).Field("name", "two")
                .Row("id", 3).Field("name", "three")
                .ToOperation();

            var cfg = new ProcessBuilder("process")
                
                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")

                .Connection("c1").Provider("file").File(file1)
                .Connection("c2").Provider("file").File(file2)

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

            var rows = process.ExecuteSingle();

            Assert.IsNotNull(rows);

            Assert.AreEqual(3, rows.Count());
            System.Diagnostics.Process.Start(file1);
            System.Diagnostics.Process.Start(file2);

            const int header = 1;
            Assert.AreEqual(1, File.ReadAllLines(file1).Length-header);
            Assert.AreEqual(2, File.ReadAllLines(file2).Length-header);

        }

    }
}