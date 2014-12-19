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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test {
    [TestFixture]
    public class TestBenchmark : EtlProcessHelper {

        [Test]
        public void RunCfgBenchmark() {

            var xDocWatch = new Stopwatch();
            var nanoWatch = new Stopwatch();
            var procWatch = new Stopwatch();
            var cfgWatch = new Stopwatch();

            xDocWatch.Start();
            var doc1 = XDocument.Load(File.OpenRead(@"NorthWind.xml"));
            var firstProcess1 = doc1.Root.Element("processes").Element("add");
            xDocWatch.Stop();

            nanoWatch.Start();
            var doc2 = new NanoXmlDocument(File.ReadAllText(@"NorthWind.xml"));
            var firstProcess2 = doc2.RootNode.SubNodes.First(n => n.Name == "processes").SubNodes.First(n => n.Name == "add");
            nanoWatch.Stop();

            cfgWatch.Start();
            var cfg = new ConfigurationFactory(@"Northwind.xml").CreateSingle();
            cfgWatch.Stop();

            procWatch.Start();
            var proc = ProcessFactory.CreateSingle(@"NorthWind.xml");
            procWatch.Stop();

            Console.WriteLine("Process: " + procWatch.ElapsedMilliseconds); // ~ 1928
            Console.WriteLine(".NET Cfg: " + cfgWatch.ElapsedMilliseconds); // ~ 341
            Console.WriteLine("XDocument: " + xDocWatch.ElapsedMilliseconds); // ~ 45ms
            Console.WriteLine("NanoXml: " + nanoWatch.ElapsedMilliseconds); // ~ 18 ms

            Assert.AreEqual("NorthWind", firstProcess2.GetAttribute("name").Value);
            Assert.AreEqual("NorthWind", firstProcess1.Attribute("name").Value);
            Assert.AreEqual("NorthWind", cfg.Name);
            Assert.AreEqual("NorthWind", proc.Name);

        }

        [Test]
        public void TestRoot() {
            var sw = new Stopwatch();
            sw.Start();
            var rootNode = new NanoXmlDocument(File.ReadAllText(@"NorthWind.xml")).RootNode;
            var root = new TflRoot(rootNode).Load();
            sw.Stop();
            Console.WriteLine("Root: {0}", sw.ElapsedMilliseconds);

            Assert.AreEqual(new object[] { "NorthWind", "string" }, root["processes", 0]["name"]);
            Assert.AreEqual(new object[] { "", "string" }, root["processes", 0]["mode"]);
            Assert.AreEqual(new object[] { string.Empty, "string" }, root["processes", 0]["pipeline-threading"]);
            Assert.AreEqual(new object[] { true, "boolean" }, root["processes", 0]["enabled"]);

            Assert.AreEqual(new object[] { "NorthWindStar", "string" }, root["processes", 0]["connections", 1]["database"]);

        }

    }

    public class TflRoot : TflNode {
        public TflRoot(NanoXmlNode node)
            : base(node) {
            Elements<TflEnvironment>("environments");
            Elements<TflProcess>("processes");
        }
    }

    public class TflEnvironment : TflNode {
        public TflEnvironment(NanoXmlNode node)
            : base(node) {
            Attribute(string.Empty, "name");
            Elements<TflParameter>("parameters");
        }
    }

    public class TflParameter : TflNode {
        public TflParameter(NanoXmlNode node)
            : base(node) {
            Attribute(string.Empty, "entity", "field", "name", "value");
            Attribute(true, "input");
            Attribute("string", "type");
        }
    }

    public class TflConnection : TflNode {
        public TflConnection(NanoXmlNode node)
            : base(node) {

            Attribute(500, "batch-size");
            Attribute(string.Empty, "connection-string", "content-type", "database", "error-mode", "file", "folder", "footer", "name", "password", "path", "url", "user");
            Attribute(Common.DefaultValue, "data", "header", "version");
            Attribute("MM/dd/yyyy h:mm:ss tt", "date-format");
            Attribute(",", "delimiter");
            Attribute(false, "direct", "enable-ssl");
            Attribute(true, "enabled");
            Attribute("utf-8", "encoding");
            Attribute(0, "end", "port");
            Attribute("SqlServer", "provider");
            Attribute("TopDirectoryOnly", "search-option");
            Attribute("*.*", "search-pattern");
            Attribute("localhost", "server");
            Attribute(1, "start");
            Attribute("GET", "web-method");
        }

    }

    public class TflProcess : TflNode {

        public TflProcess(NanoXmlNode node)
            : base(node) {

            Attribute(string.Empty, "name", "mode", "pipeline-threading", "inherit", "time-zone", "star", "view");
            Attribute(true, "enabled", "star-enabled", "view-enabled", "parallel");
            Attribute("raw", "template-content-type");

            Elements<TflParameter>("parameters");
            Elements<TflConnection>("connections");
        }

    }
}