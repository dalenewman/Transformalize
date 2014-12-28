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
using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {

    [TestFixture]
    public class TestNewConfiguration {

        [Test]
        public void TestEmptyCfg() {
            var cfg = new NanoXmlDocument(@"<transformalize></transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            Assert.AreEqual(1, root.AllProblems().Count);
            Assert.AreEqual("The 'transformalize' element is missing a 'processes' element.", root.AllProblems()[0]);
        }

        [Test]
        public void TestEmptyProcesses() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <processes>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            Assert.AreEqual(1, root.AllProblems().Count);
            Assert.AreEqual("A 'processes' element is missing an 'add' element.", root.AllProblems()[0]);
        }

        [Test]
        public void TestInvalidProcess() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <processes>
        <add />
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            Assert.AreEqual(3, root.AllProblems().Count);
            foreach (var problem in root.AllProblems()) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual("A 'processes' 'add' element is missing a 'name' attribute.", root.AllProblems()[0]);
            Assert.AreEqual("A 'processes' 'add' element is missing a 'connections' element.", root.AllProblems()[1]);
            Assert.AreEqual("A 'processes' 'add' element is missing an 'entities' element.", root.AllProblems()[2]);
        }

        [Test]
        public void TestInvalidProcessAttribute() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <processes>
        <add name='dale' invalid='true'>
            <connections />
            <entities />
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            Assert.AreEqual(3, root.AllProblems().Count);
            foreach (var problem in root.AllProblems()) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual("A 'processes' 'add' element contains an invalid 'invalid' attribute.", root.AllProblems()[0]);
            Assert.AreEqual("A 'connections' element is missing an 'add' element.", root.AllProblems()[1]);
            Assert.AreEqual("An 'entities' element is missing an 'add' element.", root.AllProblems()[2]);
        }

    }


}