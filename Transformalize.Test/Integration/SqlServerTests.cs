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

using System;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class SqlServerTests {

        [Test]
        public void TestInit() {

            var config = new ProcessBuilder("SqlServerTest")
                .Connection("input").Database("Orchard171")
                .Connection("output").Provider("internal")
                .Template("dml")
                    .File("http://config.mwf.local/Templates/DukeInvoice.sql")
                        .Setting("BatchId", 2, "int")
                        .Action("open").Connection("input")
                .Entity("WorkOrder")
                    .DetectChanges(false)
                    .Connection("input")
                    .SqlOverride(@"
                        SELECT ActionValue
                        FROM SS_BatchRecord
                        WHERE BatchId = 2;"
                    )
                    .Field("ActionValue").PrimaryKey()
                .Process();

            var xml = config.Serialize();
            Console.WriteLine(xml);
            Console.WriteLine();

            ProcessFactory.Create(config, new Options() { LogLevel = LogLevel.Info })[0].Run();
        }

        [Test]
        public void TestDukeInvoice() {
            var processes = ProcessFactory.Create("http://config.mwf.local/DukeBilling.xml", new Options() { Mode = "default" });
            foreach (var process in processes) {
                process.Run();
            }
        }

        [Test]
        public void TestFail()
        {
            const string file = @"C:\Code\TransformalizeConfiguration\TransformalizeConfiguration\App_Data\Clevest35\Duke.xml";
            var process = ProcessFactory.Create(file, new Options() {Mode = "init", LogLevel = LogLevel.Debug})[0];
            process.Run();
        }


    }
}