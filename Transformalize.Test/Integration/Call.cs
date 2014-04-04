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

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class Calls {

        private const string CALLS = @"c:\code\TflConfiguration\Calls.xml";
        private const string CAMPAIGNS = @"c:\code\TflConfiguration\Campaigns.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Info);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Init() {
            ProcessFactory.Create(CALLS, new Options { Mode = "init" })[0].Run();
            ProcessFactory.Create(CAMPAIGNS, new Options { Mode = "init" })[0].Run();
        }

        [Test]
        public void Normal() {
            ProcessFactory.Create(CALLS)[0].Run();
            ProcessFactory.Create(CAMPAIGNS)[0].Run();
        }

        [Test]
        public void Test() {
            ProcessFactory.Create(CALLS, new Options("{'mode':'test','top':2,'loglevel':'trace'}"))[0].Run();
            ProcessFactory.Create(CAMPAIGNS, new Options("{'mode':'test','top':2,'loglevel':'trace'}"))[0].Run();
        }
    }
}