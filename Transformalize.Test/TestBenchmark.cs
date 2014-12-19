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
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;
using Transformalize.Libs.NanoXml;
using Transformalize.Libs.Nest.Domain.Mapping.Descriptors;
using Transformalize.Libs.SolrNet.Utils;
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

    public class TflRoot : TflAttributes {
        public TflRoot(NanoXmlNode node)
            : base(node) {
            Element<TflProcess>("processes");
            Element<TflEnvironment>("environments");
        }
    }

    public class TflEnvironment : TflAttributes {
        public TflEnvironment(NanoXmlNode node)
            : base(node) {
            Attributes["default"] = Empty();
        }
    }

    public class TflConnection : TflAttributes {
        public TflConnection(NanoXmlNode node)
            : base(node) {

            Attribute("batch-size", 500);
            Attribute("connection-string", string.Empty);
            Attribute("content-type", string.Empty);
            Attribute("data", Common.DefaultValue);
            Attribute("database", string.Empty);
            Attribute("date-format", "MM/dd/yyyy h:mm:ss tt");
            Attribute("delimiter", ",");
            Attribute("direct", false);
            Attribute("enabled", true);
            Attribute("enable-ssl", false);
            Attribute("encoding", "utf-8");
            Attribute("end", 0);
            Attribute("error-mode", string.Empty);
            Attribute("file", string.Empty);
            Attribute("folder", string.Empty);
            Attribute("footer", string.Empty);
            Attribute("header", Common.DefaultValue);
            Attribute("name", string.Empty);
            Attribute("password", string.Empty);
            Attribute("path", string.Empty);
            Attribute("port", 0);
            Attribute("provider", "SqlServer");
            Attribute("schema", string.Empty);
            Attribute("search-option", "TopDirectoryOnly");
            Attribute("search-pattern", "*.*");
            Attribute("server", "localhost");
            Attribute("start", 1);
            Attribute("table", string.Empty);
            Attribute("url", string.Empty);
            Attribute("user", string.Empty);
            Attribute("version", Common.DefaultValue);
            Attribute("view", string.Empty);
            Attribute("web-method", "GET");
        }

    }

    public class TflProcess : TflAttributes {

        public TflProcess(NanoXmlNode node)
            : base(node) {

            Attribute("name", string.Empty);
            Attribute("mode", string.Empty);
            Attribute("pipeline-threading", string.Empty);
            Attribute("inherit", string.Empty);
            Attribute("time-zone", string.Empty);
            Attribute("enabled", true);
            Attribute("star-enabled", true);
            Attribute("star", string.Empty);
            Attribute("view-enabled", true);
            Attribute("view", string.Empty);
            Attribute("template-content-type", string.Empty);
            Attribute("parallel", true);

            Element<TflConnection>("connections");
        }

    }
}