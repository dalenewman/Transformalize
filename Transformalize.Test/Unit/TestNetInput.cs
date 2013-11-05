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
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Test.Unit {
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

            //ProcessFactory.Create(process, new Options() { Mode = "init" }).Run();
            ProcessFactory.Create(process, new Options() { Mode = "test" }).Run();

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