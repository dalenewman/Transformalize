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
    public class Dsl
    {
        private const string PJ = @"http://config.mwf.local/DslPj.xml";
        private const string PJB = @"http://config.mwf.local/DslPjBud.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Info);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Metadata() {
            ProcessFactory.Create(PJ, new Options() { Mode = "metadata" }).Run();
        }
        [Test]
        public void Init() {
            ProcessFactory.Create(PJ, new Options() { Mode = "init"}).Run();
        }

        [Test]
        public void Default() {
            ProcessFactory.Create(PJ).Run();
        }

        [Test]
        public void RunTest() {
            ProcessFactory.Create(PJ, new Options() { Top = 15, Mode = "test", LogLevel  = LogLevel.Info }).Run();
        }


        [Test]
        public void MetadataB() {
            ProcessFactory.Create(PJB, new Options() { Mode = "metadata" }).Run();
        }

        [Test]
        public void InitB() {
            ProcessFactory.Create(PJB, new Options() { Mode = "init" }).Run();
        }

        [Test]
        public void DefaultB()
        {
            ProcessFactory.Create(PJB).Run();
        }


    }
}