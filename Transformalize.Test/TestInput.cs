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

namespace Transformalize.Test {

    [TestFixture]
    public class TestInput {

        [Test]
        public void TestSingleInput()
        {
            var file1 = Path.GetTempFileName();
            File.WriteAllText(file1,"id,name\n1,one\n2,two\n3,three");

            var cfg = new ProcessBuilder("process")

                .Connection("input1").Provider("file").File(file1).Delimiter(",").Start(2)
                .Connection("output").Provider("internal")

                .Entity("entity")
                    .Connection("input1")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.CreateSingle(cfg);

            var output = process.Execute().ToArray();

            Assert.IsInstanceOf<IEnumerable<Row>>(output);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(true, output.Any(r => r["id"].Equals(1)));
            Assert.AreEqual(true, output.Any(r => r["id"].Equals(2)));
            Assert.AreEqual(true, output.Any(r => r["id"].Equals(3)));
            Assert.AreEqual(true, output.Any(r => r["name"].Equals("one")));
            Assert.AreEqual(true, output.Any(r => r["name"].Equals("two")));
            Assert.AreEqual(true, output.Any(r => r["name"].Equals("three")));

        }

        [Test]
        public void TestTwoInputs() {

            var file1 = Path.GetTempFileName();
            File.WriteAllText(file1, "id,name\n1,One\n2,Two\n3,Three");

            var file2 = Path.GetTempFileName();
            File.WriteAllText(file2, "id,name\n4,Four\n5,Five\n6,Six");

            var cfg = new ProcessBuilder("process")
                
                .Connection("input1").Provider("file").File(file1).Delimiter(",").Start(2)
                .Connection("input2").Provider("file").File(file2).Delimiter(",").Start(2)
                .Connection("output").Provider("internal")
                .Entity("entity")
                    .Input("i1","input1")
                    .Input("i2","input2")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                
                .Process();

            var process = ProcessFactory.CreateSingle(cfg);

            var output = process.Execute().ToArray();

            Assert.IsInstanceOf<IEnumerable<Row>>(output);

            Assert.AreEqual(6, output.Length);
            Assert.AreEqual(1+2+3+4+5+6, output.Sum(r => (int) r["id"]));
            Assert.AreEqual("OneTwoThreeFourFiveSix", string.Concat(output.OrderBy(r => (int)r["id"]).Select(r => r["name"])));

        }

    }
}