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
using System.Xml.Linq;
using NUnit.Framework;
using Transformalize.Main;
using Transformalize.Runner;

namespace Transformalize.Test
{
    [TestFixture]
    public class TestConfiguration
    {
        [Test]
        [Ignore("Because this requires a NorthWind database.")]
        public void TestBase()
        {
            var northWind = ProcessFactory.Create("NorthWind.xml")[0];
            Assert.AreEqual("int16", northWind.Entities[0].Fields["OrderDetailsQuantity"].Type);
            Assert.AreEqual(8, northWind.Entities.Count);
            Assert.AreEqual(3, northWind.Entities[1].CalculatedFields.Count);
            Assert.AreEqual(2, northWind.FileInspectionRequest.DataTypes.Count);
        }

        [Test]
        public void TestSimpleExpansion() {
            var northWind = ProcessFactory.Create("NorthWindExpanded.xml")[0];
            Assert.AreEqual("System.Int32", northWind.Entities[0].Fields["OrderDetailsQuantity"].Type);
            Assert.AreEqual(8, northWind.Entities.Count);
            Assert.AreEqual(4, northWind.Entities[1].CalculatedFields.Count);
        }

        [Test]
        public void TestDefaultParameters()
        {
            const string xml = @"<transformalize>
    <processes>
        <add name=""test1"">
            <parameters>
                <add name=""t1"" value=""v1"" />
            </parameters>
            <actions>
                <add action=""@(t1)"" />
            </actions>
        </add>
        <add name=""test2"">
            <parameters>
                <add name=""t1"" value=""v2"" />
                <add name=""t2"" value=""v3"" />
            </parameters>
            <actions>
                <add action=""@(t1)"" />
                <add action=""@(t2)"" />
            </actions>
        </add>
    </processes>
</transformalize>";
            var output = ProcessXmlConfigurationReader.DefaultParameters(xml);
            var doc = XDocument.Parse(output);
            var actions = doc.Descendants("add").Where(n=>n.Attributes("action").Any()).ToArray();
            
            Assert.AreEqual("v1", actions[0].Attribute("action").Value);
            Assert.AreEqual("v2", actions[1].Attribute("action").Value);
            Assert.AreEqual("v3", actions[2].Attribute("action").Value);

        }

        [Test]
        public void TestEnvironmentParameters() {
            const string xml = @"<transformalize>
    <environments>
        <add name=""e1"">
            <parameters>
                <add name=""t1"" value=""v1"" />
                <add name=""t2"" value=""v2"" />
                <add name=""t3"" value=""v3"" />
            </parameters>
        </add>
    </environments>
    <processes>
        <add name=""test1"">
            <actions>
                <add action=""@(t1)"" />
            </actions>
        </add>
        <add name=""test2"">
            <actions>
                <add action=""@(t2)"" />
                <add action=""@(t3)"" />
            </actions>
        </add>
    </processes>
</transformalize>";
            var output = ProcessXmlConfigurationReader.DefaultParameters(xml);
            var doc = XDocument.Parse(output);
            var actions = doc.Descendants("add").Where(n => n.Attributes("action").Any()).ToArray();

            Assert.AreEqual("v1", actions[0].Attribute("action").Value);
            Assert.AreEqual("v2", actions[1].Attribute("action").Value);
            Assert.AreEqual("v3", actions[2].Attribute("action").Value);

        }

    }
}