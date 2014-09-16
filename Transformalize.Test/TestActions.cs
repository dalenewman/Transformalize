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
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Dapper;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestActions {

        [Test]
        public void TestCopyFile() {

            var file1 = Path.GetTempFileName();
            var file2 = Path.GetTempFileName();

            File.WriteAllText(file1, "f1,f2,f3\nv1,v2,v3");

            var process = new ProcessBuilder("CopyFile")
                .Action("Copy")
                    .From(file1)
                    .To(file2)
                .Process();

            ProcessFactory.CreateSingle(process).ExecuteScaler();

            Assert.AreEqual("f1,f2,f3\nv1,v2,v3", File.ReadAllText(file2));
        }

        [Test]
        public void TestCopyTemplateOutputToFile() {

            var template = Path.GetTempFileName();
            var file = Path.GetTempFileName();

            File.WriteAllText(template, "Setting1: @Model.Parameters.Setting1, Setting2: @Model.Parameters.Setting2;");

            var process = new ProcessBuilder("TestCopyTemplateOutputToFile")
                .Template("template")
                    .File(template)
                .Parameter("Setting1", 1)
                .Parameter("Setting2", 2)
                .Action("Copy")
                    .To(file)
                .Process();

            ProcessFactory.CreateSingle(process).ExecuteScaler();

            Assert.AreEqual("Setting1: 1, Setting2: 2;", File.ReadAllText(file));

        }

        [Test]
        [Ignore("requires a local SQL Server and TestOutput database")]
        public void TestCopyFileToConnection() {

            var file1 = Path.GetTempFileName();

            File.WriteAllText(file1, "f1,f2,f3\nv1,v2,v3");

            var element = new ProcessBuilder("CopyFile")
                .Connection("output")
                    .Database("TestOutput")  //default provider is SqlServer, localhost
                .Action("Copy")
                    .From(file1)
                    .To("output")
                .Process();

            var process = ProcessFactory.CreateSingle(element);
            process.ExecuteScaler();
            var expected = Common.CleanIdentifier(Path.GetFileNameWithoutExtension(file1));
            var sqlServer = new ConnectionFactory(process).Create(new ConnectionConfigurationElement() { Name = "test", Provider = "sqlserver", Database = "TestOutput" });

            var rows = sqlServer.GetConnection().Query(string.Format("SELECT f1, f2, f3 FROM {0}", expected)).ToArray();
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("v1", rows[0].f1);
            Assert.AreEqual("v2", rows[0].f2);
            Assert.AreEqual("v3", rows[0].f3);

        }


    }
}