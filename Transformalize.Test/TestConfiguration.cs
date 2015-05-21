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

using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.Cfg.Net.Loggers;
using Transformalize.Main;

namespace Transformalize.Test {
    [TestFixture]
    public class TestConfiguration {

        [Test]
        [Ignore("Because this requires a NorthWind database.")]
        public void TestBase() {
            var northWind = ProcessFactory.Create("NorthWind.xml", new TestLogger())[0];
            Assert.AreEqual("int16", northWind.Entities[0].Fields["OrderDetailsQuantity"].Type);
            Assert.AreEqual(8, northWind.Entities.Count);
            Assert.AreEqual(3, northWind.Entities[1].CalculatedFields.Count);
            Assert.AreEqual(2, northWind.FileInspectionRequest.DataTypes.Count);
        }

        [Test]
        public void TestDefaultParameters() {
            const string xml = @"<transformalize>
    <environments default='@(Environment)'>
        <add name='one'>
            <parameters>
                <add name=""t1"" value=""v1"" />
                <add name=""t2"" value=""v2"" />
            </parameters>
        </add>
        <add name='two'>
            <parameters>
                <add name=""t1"" value=""v2"" />
                <add name=""t2"" value=""v3"" />
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
                <add action=""@(t1)"" />
                <add action=""@(t2)"" />
            </actions>
        </add>
    </processes>
</transformalize>";
            var root1 = new TflRoot(xml, new Dictionary<string, string>() { { "Environment", "one" } });
            Assert.AreEqual("v1", root1.Processes[0].Actions[0].Action);
            Assert.AreEqual("v1", root1.Processes[1].Actions[0].Action);
            Assert.AreEqual("v2", root1.Processes[1].Actions[1].Action);

            var root2 = new TflRoot(xml, new Dictionary<string, string>() { { "Environment", "two" } });
            Assert.AreEqual("v2", root2.Processes[0].Actions[0].Action);
            Assert.AreEqual("v2", root2.Processes[1].Actions[0].Action);
            Assert.AreEqual("v3", root2.Processes[1].Actions[1].Action);

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
            var processes = new TflRoot(xml, null).Processes;

            Assert.AreEqual("v1", processes[0].Actions[0].Action);
            Assert.AreEqual("v2", processes[1].Actions[0].Action);
            Assert.AreEqual("v3", processes[1].Actions[1].Action);

        }

        [Test]
        public void TestIntegratedEntityFilters() {
            const string xml = @"<transformalize>
    <processes>
        <add name=""process"">
            <connections>
                <add name=""input"" database=""master"" />
            </connections>
            <entities>
                <add name=""entity"" version=""version"">
                    <filter>
                        <add left=""field1"" right=""literal1"" operator=""NotEqual"" continuation=""AND"" />
                        <add left=""field2"" right=""6"" operator=""GreaterThan"" continuation=""OR"" />
                        <add expression=""field3 != 'literal3'"" />
                    </filter>
                    <fields>
                        <add name=""field1"" primary-key=""true"" />
                        <add name=""field2"" type=""int"" />
                        <add name=""field3"" />
                        <add name=""version"" type=""byte[]"" length=""8"" />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>";
            var process = ProcessFactory.CreateSingle(xml, new TestLogger());

            Assert.AreEqual("process", process.Name);
            Assert.AreEqual("entity", process.Entities.First().Name);

            var filters = process.Entities.First().Filters;

            Assert.AreEqual(3, filters.Count);
            Assert.AreEqual("field1 != 'literal1'", filters[0].ResolveExpression("'"));
            Assert.AreEqual("field2 > 6", filters[1].ResolveExpression("'"));
            Assert.AreEqual("field3 != 'literal3'", filters[2].ResolveExpression("'"));

            Assert.AreEqual("field1 != 'literal1' AND field2 > 6 OR field3 != 'literal3'", filters.ResolveExpression("'"));

            var sql = process.Entities[0].Input[0].Connection.KeyAllQuery(process.Entities[0]);

            Assert.AreEqual(@"
                SELECT [field1] FROM [dbo].[entity] WHERE field1 != 'literal1' AND field2 > 6 OR field3 != 'literal3'", sql);
        }

        [Test()]
        [Ignore("requires db.")]
        public void TestError() {
            var cfg = new TflRoot(File.ReadAllText(@"C:\Temp\test.xml"), null);
            Assert.AreEqual(0, cfg.Errors().Length);

            var process = ProcessFactory.CreateSingle(@"C:\Temp\test.xml", new TestLogger());
            Assert.IsNotNull(process);

            process.ExecuteScaler();
        }


    }
}