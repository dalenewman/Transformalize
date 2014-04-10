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
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Main.Providers.ElasticSearch;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class ElasticSearchTests {

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestConnectionChecker() {

            var cfg = new ProcessBuilder("est")
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Process();
            var process = ProcessFactory.Create(cfg, new Options() { LogLevel = LogLevel.Debug })[0];

            var checker = new ElasticSearchConnectionChecker();
            
            Assert.IsTrue(checker.Check(process.OutputConnection));
        }

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestEntityRecordsExists() {

            var cfg = new ProcessBuilder("est")
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            var result = new ElasticSearchEntityRecordsExist().RecordsExist(process.OutputConnection, process.Entities[0]);

            Assert.IsTrue(result);
        }

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestAddRecords() {

            var file = Path.GetTempFileName();
            File.WriteAllText(file, "id,name\n1,One\n2,Two\n3,Three\n4,Four\n5,Five\n6,six");

            var cfg = new ProcessBuilder("est")

                .Connection("input").Provider("file").File(file).Delimiter(",").Start(2)
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)

                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];
            var output = process.Run();
        }

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestDeleteRecords() {

            var cfg = new ProcessBuilder("est")
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            new ElasticSearchEntityDropper().Drop(process.OutputConnection, process.Entities[0]);
        }

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestGetNextBatchId() {

            var cfg = new ProcessBuilder("est")
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            Assert.AreEqual(3, process.Entities[0].TflBatchId);
        }

        //[Ignore("Because you need to have Elastic Search for this to pass.")]
        [Test]
        public void TestWriteEndVersion() {

            var file = Path.GetTempFileName();
            File.WriteAllText(file, "id,name\n1,One\n2,Two\n3,Three\n4,Four\n5,Five\n6,six");

            var cfg = new ProcessBuilder("est")
                .Connection("input").Provider("file").File(file).Delimiter(",").Start(2)
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Entity("entity")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                .Process();

            var process = ProcessFactory.Create(cfg, new Options() { Mode = "default"})[0];
            process.Run();

            process.OutputConnection.WriteEndVersion(process.Connections["input"], process.Entities[0]);

            Assert.AreEqual(2, process.Entities[0].TflBatchId);
        }

        [Test]
        public void TestInit() {

            var file = Path.GetTempFileName();
            File.WriteAllText(file, "id,name,number\n1,One,1.1\n2,Two,2.2\n3,Three,3.3\n4,Four,4.4\n5,Five,5.5\n6,six,6.6");

            var cfg = new ProcessBuilder("est")
                .Connection("input").Provider("file").File(file).Delimiter(",").Start(2)
                .Connection("output").Provider("elasticsearch").Server("localhost").Port(9200)
                .Entity("entity").Version("TflHashCode")
                    .Field("id").Int32().PrimaryKey()
                    .Field("name")
                    .Field("number").Double()
                    .CalculatedField("TflHashCode").Int32()
                        .Transform("concat")
                            .Parameter("name")
                            .Parameter("number")
                        .Transform("gethashcode")
                .Process();

            var process = ProcessFactory.Create(cfg, new Options() { Mode = "default" })[0];
            process.Run();

        }





    }
}