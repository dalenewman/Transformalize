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

using System.Linq;
using NUnit.Framework;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class OrderType
    {
        private const string FILE = @"c:\code\TflConfiguration\OrderType.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Info);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Init() {
            var options = new Options() { Mode = "init"};
            var process = ProcessFactory.Create(FILE, options)[0];
            process.Run();
        }

        [Test]
        public void Default() {
            var process = ProcessFactory.Create(FILE, new Options() { Mode = "test" })[0];
            process.GetField("Project","OrderType").Default = "Dale";
            process.GetField("Project", "HostMessageConfig").Default = "Dale";
            process.Entities[0].Input.First().Connection.Database = "ClevestDale";
            process.Entities[0].Input.First().Connection.Server = "localhost";
            process.Run();
        }

    }
}