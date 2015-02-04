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
using Transformalize.Main;

namespace Transformalize.Test {
    [TestFixture]
    public class TestBenchmark : EtlProcessHelper {

        [Test]
        public void RunCfgBenchmark() {

            var xml = File.ReadAllText(@"NorthWind.xml");

            var xDocWatch = new Stopwatch();
            var procWatch = new Stopwatch();
            var cfgWatch = new Stopwatch();

            xDocWatch.Start();
            var xDocProcess = XDocument.Parse(xml).Root.Element("processes").Element("add");
            xDocWatch.Stop();

            cfgWatch.Start();
            var cfgProcess = new ConfigurationFactory(xml).CreateSingle();
            cfgWatch.Stop();

            procWatch.Start();
            var bigBloatedProcess = ProcessFactory.CreateSingle(xml);
            procWatch.Stop();

            Console.WriteLine("Process: " + procWatch.ElapsedMilliseconds); // ~ 1928 to 1564
            Console.WriteLine("Config: " + cfgWatch.ElapsedMilliseconds); // ~ 341 to 177
            Console.WriteLine("XDocument: " + xDocWatch.ElapsedMilliseconds); // ~ 45ms to 40

            Assert.AreEqual("NorthWind", xDocProcess.Attribute("name").Value);
            Assert.AreEqual("NorthWind", cfgProcess.Name);
            Assert.AreEqual("NorthWind", bigBloatedProcess.Name);

        }

    }
}