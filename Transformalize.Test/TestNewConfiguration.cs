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
using Transformalize.Configuration;
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

            var problems = root.AllProblems();

            Assert.AreEqual(3, problems.Count);

            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.IsTrue(problems.Contains("A 'processes' 'add' element is missing a 'name' attribute."));
            Assert.IsTrue(problems.Contains("A 'processes' 'add' element is missing a 'connections' element."));
            Assert.IsTrue(problems.Contains("A 'processes' 'add' element is missing an 'entities' element."));

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
            var problems = root.AllProblems();
            Assert.AreEqual(3, problems.Count);
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual("A 'processes' 'add' element contains an invalid 'invalid' attribute.  Valid attributes are: name, enabled, mode, parallel, pipeline-threading, star, star-enabled, template-content-type, time-zone, view, view-enabled.", problems[0]);
            Assert.AreEqual("A 'connections' element is missing an 'add' element.", problems[1]);
            Assert.AreEqual("An 'entities' element is missing an 'add' element.", problems[2]);
        }


        [Test]
        public void TestProcessEntityFieldNames() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' />
            </connections>
            <entities>
                <add name='entity' >
                    <fields>
                        <add name='field' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual(0, problems.Count);
            Assert.AreEqual("process", root["processes", 0]["name"].Value);
            Assert.AreEqual("entity", root["processes", 0]["entities", 0]["name"].Value);
            Assert.AreEqual("field", root["processes", 0]["entities", 0]["fields", 0]["name"].Value);

        }

        [Test]
        public void TestClassProperty() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <environments default='one'>
        <add name='one'>
            <parameters>
                <add name='one' />
            </parameters>
        </add>
        <add name='two'>
            <parameters>
                <add name='two' />
            </parameters>
        </add>
    </environments>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' />
            </connections>
            <entities>
                <add name='entity' >
                    <fields>
                        <add name='field' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual(0, problems.Count);
            Assert.AreEqual("one", root["environments", 0]["default"].Value);
        }


        [Test]
        public void TestFileInspection() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <processes>
        <add name='process'>
            <file-inspection>
                <add name='default' sample='99' max-length='128' min-length='64'>
                    <types>
                    </types>
                    <delimiters>
                        <add name='comma' character=',' />
                    </delimiters>
                </add>
            </file-inspection>
            <connections>
                <add name='input' />
            </connections>
            <entities>
                <add name='entity' >
                    <fields>
                        <add name='field' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual(0, problems.Count);
            Assert.AreEqual("process", root["processes", 0]["name"].Value);
            Assert.AreEqual("entity", root["processes", 0]["entities", 0]["name"].Value);
            Assert.AreEqual("field", root["processes", 0]["entities", 0]["fields", 0]["name"].Value);
            Assert.AreEqual("default", root["processes", 0]["file-inspection", 0]["name"].Value);
            Assert.AreEqual(99, root["processes", 0]["file-inspection", 0]["sample"].Value);
            Assert.AreEqual(128, root["processes", 0]["file-inspection", 0]["max-length"].Value);
            Assert.AreEqual(64, root["processes", 0]["file-inspection", 0]["min-length"].Value);
        }

        [Test]
        public void TestParameterToObject() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <environments default='one'>
        <add name='one'>
            <parameters>
                <add name='one' value='1' />
            </parameters>
        </add>
        <add name='two'>
            <parameters>
                <add name='two' />
            </parameters>
        </add>
    </environments>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' />
            </connections>
            <entities>
                <add name='entity' >
                    <fields>
                        <add name='field' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual(0, problems.Count);
            root.Populate();

            var x = (TflRoot) root;

            var testParameter = x.Environments[0].Parameters[0];
            Assert.AreEqual("one", testParameter.Name);
            Assert.AreEqual("1", testParameter.Value);
        }

        [Test]
        public void TestEnvironmentToObject() {
            var cfg = new NanoXmlDocument(@"<transformalize>
    <environments default='one'>
        <add name='one'>
            <parameters>
                <add name='one' value='1' />
            </parameters>
        </add>
        <add name='two'>
            <parameters>
                <add name='two' />
            </parameters>
        </add>
    </environments>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' />
            </connections>
            <entities>
                <add name='entity' >
                    <fields>
                        <add name='field' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>".Replace("'", "\"")).RootNode;
            var root = new TflRoot().Load(cfg);
            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }
            Assert.AreEqual(0, problems.Count);

            root.Populate();
            var x = (TflRoot) root;
            Assert.AreEqual("one", x.Environments[0].Name);
            Assert.AreEqual("one", x.Environments[0].Default);
            Assert.AreEqual(1, x.Environments[0].Parameters.Count);
        }
    }
}