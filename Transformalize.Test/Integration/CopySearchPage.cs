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
    public class CopySearchPage {

        [Test]
        public void Run() {
            var options = new Options() { Mode="default", LogLevel = LogLevel.Info };
            var process = ProcessFactory.Create(@"http://config.mwf.local/CopySearchPage.xml", options)[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var entities = process.Run();
            Assert.AreEqual(1, entities.Count());
        }

    }
}