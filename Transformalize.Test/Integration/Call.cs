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
using Transformalize.Runner;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class Calls {
        private const string CALLS = @"c:\etl\rhinoetl\tfl\Calls.xml";
        private const string CAMPAIGNS = @"c:\etl\rhinoetl\tfl\Campaigns.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Debug);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Init() {
            var options1 = new Options { Mode = "init" };
            var process1 = new ProcessReader(new ProcessXmlConfigurationReader(CALLS).Read(), options1).Read();
            process1.Run();

            var options2 = new Options { Mode = "init" };
            var process2 = new ProcessReader(new ProcessXmlConfigurationReader(CAMPAIGNS).Read(), options2).Read();
            process2.Run();
        }

        [Test]
        public void Normal() {
            var options1 = new Options();
            var process1 = new ProcessReader(new ProcessXmlConfigurationReader(CALLS).Read(), options1).Read();
            process1.Run();

            var options2 = new Options();
            var process2 = new ProcessReader(new ProcessXmlConfigurationReader(CAMPAIGNS).Read(), options2).Read();
            process2.Run();
        }

        [Test]
        public void Test() {
            var options1 = new Options("{'mode':'test','top':2,'loglevel':'trace'}");
            var process1 = new ProcessReader(new ProcessXmlConfigurationReader(CALLS).Read(), options1).Read();
            process1.Run();

            var options2 = new Options("{'mode':'test','top':2,'loglevel':'trace'}");
            var process2 = new ProcessReader(new ProcessXmlConfigurationReader(CAMPAIGNS).Read(), options2).Read();
            process2.Run();
        }
    }
}