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

            Assert.AreEqual(new TflMeta("NorthWind").Value, root["processes", 0]["name"].Value);
            Assert.AreEqual(new TflMeta("").Value, root["processes", 0]["mode"].Value);
            Assert.AreEqual(new TflMeta(string.Empty).Value, root["processes", 0]["pipeline-threading"].Value);
            Assert.AreEqual(new TflMeta(true).Value, root["processes", 0]["enabled"].Value);

            Assert.AreEqual(new TflMeta("NorthWindStar").Value, root["processes", 0]["connections", 1]["database"].Value);

        }

    }

    public class TflRoot : TflNode {

        internal class TflEnvironment : TflNode {
            public TflEnvironment(NanoXmlNode node)
                : base(node) {
                Attribute("name", string.Empty, true, true);
                Elements<TflParameter>("parameters");
            }
        }

        internal class TflProcess : TflNode {

            internal class TflConnection : TflNode {
                public TflConnection(NanoXmlNode node)
                    : base(node) {

                    Attribute("batch-size", 500);
                    Attribute("connection-string", string.Empty);
                    Attribute("content-type", string.Empty);
                    Attribute("database", string.Empty);
                    Attribute("error-mode", string.Empty);
                    Attribute("file", string.Empty);
                    Attribute("folder", string.Empty);
                    Attribute("footer", string.Empty);
                    Attribute("name", string.Empty);
                    Attribute("password", string.Empty);
                    Attribute("path", string.Empty);
                    Attribute("url", string.Empty);
                    Attribute("user", string.Empty);
                    Attribute("data", Common.DefaultValue);
                    Attribute("header", Common.DefaultValue);
                    Attribute("version", Common.DefaultValue);
                    Attribute("date-format", "MM/dd/yyyy h:mm:ss tt");
                    Attribute("delimiter", ",");
                    Attribute("direct", false);
                    Attribute("enable-ssl", false);
                    Attribute("enabled", true);
                    Attribute("encoding", "utf-8");
                    Attribute("end", 0);
                    Attribute("port", 0);
                    Attribute("provider", "SqlServer");
                    Attribute("search-option", "TopDirectoryOnly");
                    Attribute("search-pattern", "*.*");
                    Attribute("server", "localhost");
                    Attribute("start", 1);
                    Attribute("web-method", "GET");
                }
            }

            public TflProcess(NanoXmlNode node)
                : base(node) {

                Attribute("name", string.Empty);
                Attribute("mode", string.Empty);
                Attribute("pipeline-threading", string.Empty);
                Attribute("inherit", string.Empty);
                Attribute("time-zone", string.Empty);
                Attribute("star", string.Empty);
                Attribute("view", string.Empty);

                Attribute("enabled", true);
                Attribute("star-enabled", true);
                Attribute("view-enabled", true);
                Attribute("parallel", true);

                Attribute("template-content-type", "raw");

                Elements<TflParameter>("parameters");
                Elements<TflConnection>("connections");
                Elements<TflProvider>("providers");
                Elements<TflLog>("log");
            }

            internal class TflProvider : TflNode {
                public TflProvider(NanoXmlNode node)
                    : base(node) {
                    Attribute("name", string.Empty, true, true);
                    Attribute("type", "SqlServer", true);
                }
            }

            internal class TflLog : TflNode {
                public TflLog(NanoXmlNode node)
                    : base(node) {
                    Attribute("name", string.Empty, true, true);
                    Attribute("provider", Common.DefaultValue);
                    Attribute("layout", Common.DefaultValue);
                    Attribute("level", "Informational");
                    Attribute("connection", Common.DefaultValue);
                    Attribute("from", Common.DefaultValue);
                    Attribute("to", Common.DefaultValue);
                    Attribute("subject", Common.DefaultValue);
                    Attribute("file", Common.DefaultValue);
                    Attribute("folder", Common.DefaultValue);
                    Attribute("async", false);
                }
            }
        }

        public TflRoot(NanoXmlNode node)
            : base(node) {
            Elements<TflEnvironment>("environments");
            Elements<TflProcess>("processes");
        }
    }

    public class TflParameter : TflNode {
        public TflParameter(NanoXmlNode node)
            : base(node) {
            Attribute("entity", string.Empty);
            Attribute("field", string.Empty);
            Attribute("name", string.Empty);
            Attribute("value", string.Empty);
            Attribute("input", true);
            Attribute("type", "string");
        }
    }

}