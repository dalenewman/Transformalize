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

using NUnit.Framework;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class Bcp : EtlProcessHelper {

        private const string FILE = @"C:\Code\TflConfiguration\Es.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Debug);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void ExportDataWithBcp() {
            var process = ProcessFactory.Create(FILE, new Options { Mode = "test" });
            var results = TestOperation(
                new BcpExtract(process, process.MasterEntity)
            );
            Assert.AreEqual(137011, results.Count);
        }
    }
}