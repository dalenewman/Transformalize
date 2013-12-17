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
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestNetInput {

        [Test]
        public void TestSpecifyDotNetInput() {

            var input = new TestInput();

            var process = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal).Input(input)
                .Connection("output").Database("TestOutput")
                .Entity("e1")
                    .Field("index").Int32()
                    .Field("indexString")
                .Process();

            ProcessFactory.Create(process, new Options() { Mode = "init" }).Run();
            ProcessFactory.Create(process, new Options() { Mode = "test" }).Run();
        }

        [Test]
        public void TestSpecifyDotNetDbInput()
        {

            var testDbInput = new TestDbInput();
            
            var process = new ProcessBuilder("process2")
                .Connection("input").Provider(ProviderType.Internal).Input(testDbInput)
                .Connection("output").Database("TestOutput")
                .Entity("e1")
                    .Field("name").Length(128)
                    .Field("database_id").Int32().PrimaryKey()
                .Process();

            ProcessFactory.Create(process, new Options() { Mode = "init" }).Run();
            ProcessFactory.Create(process, new Options() { Mode = "test" }).Run();
        }

        [Test]
        public void TestInternalOutput()
        {
            var input = new TestInput();

            var process = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal).Input(input)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("e")
                    .Field("index").Int32()
                    .Field("indexString")
                .Process();

            var rows = ProcessFactory.Create(process).Run().First();
            Assert.AreEqual(10, rows.Count());
        }

    }

    public class TestDbInput : AbstractOperation {

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = new SqlConnection("server=localhost;database=master;trusted_connection=true;")) {
                cn.Open();
                var cmd = new SqlCommand("SELECT name, database_id FROM sys.databases;", cn);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return Row.FromReader(reader);
                }
            }
        }
    }


    public class TestInput : AbstractOperation {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            for (var i = 0; i < 10; i++) {
                var row = new Row {
                    {"index", i},
                    {"indexString", i.ToString(CultureInfo.InvariantCulture)}
                };
                yield return row;
            }
        }
    }
}