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
using Transformalize.Main;
using Transformalize.Runner;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class Es {
        private const string CONFIGURATION_FILE = @"C:\Code\TflConfiguration\Es.xml";

        [Test]
        public void Init() {
            var options = new Options { Mode = "init" };
            var process = ProcessFactory.Create(CONFIGURATION_FILE, options);
            process.Run();
        }

        [Test]
        public void MetaData() {
            var options = new Options("{'mode':'metadata'}");
            var process = ProcessFactory.Create(CONFIGURATION_FILE, options);
            process.Run();
        }

        [Test]
        public void First() {
            ProcessFactory.Create(CONFIGURATION_FILE, new Options() { Mode = "first"}).Run();
        }


        [Test]
        public void Normal() {
            ProcessFactory.Create(CONFIGURATION_FILE).Run();
        }

        [Test]
        public void Test() {
            var options = new Options("{'mode':'test','loglevel':'info'}");
            var process = ProcessFactory.Create(CONFIGURATION_FILE, options);
            process.Run();
        }
    }
}