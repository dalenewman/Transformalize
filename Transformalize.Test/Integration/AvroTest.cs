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
using Transformalize.Libs.Avro.File;
using Transformalize.Libs.Avro.Generic;
using Transformalize.Libs.Avro.Schema;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class AvroTest {

        private readonly Schema _schema = Schema.Parse(File.ReadAllText(@"C:\Code\Agile_Data_Code\ch03\gmail\email.avro.schema"));


        [Test]
        public void ReaObject() {

            IList<object> emails = new List<object>();

            using (var reader = DataFileReader<object>.OpenReader(@"C:\Temp\gmail\part-1.avro", _schema)) {
                foreach (var obj in reader.NextEntries) {
                    emails.Add(obj);
                }
            }

            var first = emails.First();
            var last = emails.Last();

            Assert.Less(0, emails.Count);

        }

        [Test]
        public void ReadGenericRecord() {

            IList<GenericRecord> emails = new List<GenericRecord>();

            using (var reader = DataFileReader<GenericRecord>.OpenReader(@"C:\Temp\gmail\part-1.avro", _schema)) {
                foreach (var obj in reader.NextEntries) {
                    emails.Add(obj);
                }
            }

            var first = emails.First();
            var last = emails.Last();

            Assert.Less(0, emails.Count);

        }

        [Test]
        public void TestInit() {
            var options = new Options { Mode = "init" };
            var process = ProcessFactory.Create("http://config.mwf.local/clevest-filter-updates.xml", options);
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var results = process.Run();
        }

        [Test]
        public void TestFirst() {
            var options = new Options { Mode = "first" };
            var process = ProcessFactory.Create("http://config.mwf.local/clevest-filter-updates.xml", options);
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var results = process.Run();
        }

        [Test]
        public void TestDefault() {
            var options = new Options { Mode = "default" };
            var process = ProcessFactory.Create("http://config.mwf.local/clevest-filter-updates.xml", options);
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var results = process.Run();
        }

        [Test]
        public void TestSqlOverride() {

            var config = new ProcessBuilder("TestSqlOverride")
                .Connection("input").Database("master")
                .Connection("output").Provider("console")
                .Entity("Databases").SqlOverride(@"
                    SELECT
	                    database_id AS Id,
	                    Name,
	                    recovery_model_desc as RecoveryModel,
	                    log_reuse_wait_desc AS LogReuseWait
                    FROM sys.databases WITH (NOLOCK);
                ")
                .Field("Id").Int32().PrimaryKey()
                .Field("Name").Length(128)
                .Field("RecoveryModel")
                .Field("LogReuseWait")
                .Process();

            ProcessFactory.Create(config, new Options("init") { LogLevel = LogLevel.Off }).Run();

            config.Connections["output"].Provider = "log";

            ProcessFactory.Create(config, new Options("init") { LogLevel = LogLevel.Off }).Run();

            //File.WriteAllText(@"c:\temp\sql-override.xml", config.Serialize());
            //System.Diagnostics.Process.Start(@"c:\temp\sql-override.xml");
            //System.Diagnostics.Process.Start(@"c:\temp\sql-override.txt");

        }

    }
}