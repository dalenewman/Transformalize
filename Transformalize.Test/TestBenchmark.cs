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
            var root = new TflRoot().Load(new NanoXmlDocument(File.ReadAllText(@"NorthWind.xml")).RootNode);
            sw.Stop();
            Console.WriteLine("Load: {0}", sw.ElapsedMilliseconds);

            sw = new Stopwatch();
            sw.Start();
            root.Populate();
            sw.Start();
            Console.WriteLine("Populate: {0}", sw.ElapsedMilliseconds);

            var cfg = (TflRoot)root;

            var problems = root.AllProblems();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }

            Assert.AreEqual("NorthWind", root["processes", 0]["name"].Value);
            Assert.AreEqual(string.Empty, root["processes", 0]["mode"].Value);
            Assert.AreEqual(string.Empty, root["processes", 0]["pipeline-threading"].Value);
            Assert.AreEqual(true, root["processes", 0]["enabled"].Value);

            Assert.AreEqual("prod", root["environments", 0]["name"].Value);
            Assert.AreEqual("test", root["environments", 1]["name"].Value);
            Assert.AreEqual("prod", root["environments", 1]["default"].Value);

            Assert.AreEqual(new CfgProperty("database", "NorthWindStar").Value, root["processes", 0]["connections", 1]["database"].Value);

        }

    }
}