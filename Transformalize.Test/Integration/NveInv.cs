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

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class NveInv {
        private const string FILE = @"c:\code\TflConfiguration\NveInv.xml";

        [Test]
        public void Init() {
            var options = new Options { Mode = "init" };
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
        }

        [Test]
        public void First() {
            var options = new Options { Mode = "first" };
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
        }

        [Test]
        public void Metadata() {
            var options = new Options { Mode = "metadata" };
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
        }

        [Test]
        public void Normal() {
            ProcessFactory.Create(FILE).Run();
        }

        [Test]
        public void Test() {
            var options = new Options { Mode = "test", Top = 3 };
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
        }
    }
}