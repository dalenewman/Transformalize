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

using System.IO;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Main;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class ElasticSearchTests {

        [Test]
        public void TestInit() {

            var file = Path.GetTempFileName();
            File.WriteAllText(file, "id,name\n1,One\n2,Two\n3,Three\n4,Four\n5,Five\n6,six");

            var cfg = new ProcessBuilder("est")

                .Connection("input").Provider("file").File(file).Delimiter(",").Start(2)
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)

                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg);
            var output = process.Run();
        }

    }
}