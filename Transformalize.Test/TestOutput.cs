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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Test.Builders;

namespace Transformalize.Test {

    [TestFixture]
    public class TestOutput {

        [Test]
        public void TestPrimaryOutput() {

            var input = new RowsBuilder()
                .Row("id", 1).Field("name", "one")
                .Row("id", 2).Field("name", "two")
                .Row("id", 3).Field("name", "three")
                .ToOperation();

            var xml = @"<tfl><processes>
<add name='process'>
    <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='internal' />
    </connections>
    <entities>
        <add name='entity'>
            <fields>
                <add name='id' type='int' primary-key='true' />
                <add name='name' />
            </fields>
        </add>
    </entities>
</add>
</processes></tfl>".Replace('\'', '"');

            var process = ProcessFactory.Create(xml)[0];
            process.Entities[0].InputOperation = input;
            var output = process.Execute();

            Assert.IsInstanceOf<IEnumerable<Row>>(output);
            Assert.AreEqual(3, output.Count());
        }

        [Test]
        public void TestSecondaryOutputs()
        {

            var file1 = Path.GetTempFileName();
            var file2 = Path.GetTempFileName();

            var input = new RowsBuilder()
                .Row("id", 1).Field("name", "one")
                .Row("id", 2).Field("name", "two")
                .Row("id", 3).Field("name", "three")
                .ToOperation();

            var xml = string.Format(@"<tfl><processes>
<add name='process'>
    <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='internal' />
        <add name='c1' provider='file' file='{0}' />
        <add name='c2' provider='file' file='{1}' />
    </connections>
    <entities>
        <add name='name'>
            <output>
                <add name='o1' connection='c1' run-field='name' run-value='two' />
                <add name='o2' connection='c2' run-field='id' run-operator='GreaterThan' run-type='int' run-value='1' />
            </output>
            <fields>
                <add name='id' type='int' primary-key='true' />
                <add name='name' />
            </fields>
        </add>
    </entities>
</add>
</processes></tfl>", file1,file2).Replace('\'', '"');

            var process = ProcessFactory.Create(xml)[0];
            process.Entities[0].InputOperation = input;
            var rows = process.Execute();

            Assert.IsNotNull(rows);

            Assert.AreEqual(3, rows.Count());
            const int header = 1;
            Assert.AreEqual(1, File.ReadAllLines(file1).Length-header);
            Assert.AreEqual(2, File.ReadAllLines(file2).Length-header);

        }

    }
}