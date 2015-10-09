using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestDataSets {

        [Test]
        public void Test2() {

            var xml = @"
<cfg>
    <processes>
        <add name='test'>
            <data-sets>
                <add name='one'>
                    <rows>
                        <add f1='1' f2='1' f3='2001-01-01' />
                        <add f1='2' f2='2' f3='2002-02-02' />
                        <add f1='3' f2='3' f3='2003-03-03' />
                    </rows>
                </add>
            </data-sets>

            <connections>
                <add name='input' provider='internal' />
                <add name='output' provider='internal' />
            </connections>

            <entities>
                <add name='one'>
                    <fields>
                        <add name='f1' primary-key='true' />
                        <add name='f2' type='int' />
                        <add name='f3' type='datetime' />
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</cfg>
".Replace("'","\"");
            var root = new TflRoot(xml);

            var problems = root.Errors();
            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }

            Assert.AreEqual(0, problems.Length);

            Assert.AreEqual(3, root.Processes.First().DataSets.First().Rows.Count);

            var rows = ProcessFactory.CreateSingle(root.Processes[0], new TestLogger()).Execute().ToList();

            Assert.AreEqual(3, rows.Count());

        }

        [Test]
        public void Test1() {

            var data = new List<Dictionary<string, string>>{
                new Dictionary<string, string> {{"f1", "1"}, {"f2", "1"}, {"f3", new DateTime(2001, 1, 1).ToString()}},
                new Dictionary<string, string> {{"f1", "2"}, {"f2", "2"}, {"f3", new DateTime(2002, 2, 2).ToString()}},
                new Dictionary<string, string> {{"f1", "3"}, {"f2", "3"}, {"f3", new DateTime(2003, 3, 3).ToString()}}
            };

            var process = new ProcessBuilder("test")
                .DataSet("one", data)
                .Entity("one")
                    .Field("f1").PrimaryKey()
                    .Field("f2").Type("int")
                    .Field("f3").Type("datetime")
                .Process();

            var problems = process.Errors();

            foreach (var problem in problems) {
                Console.WriteLine(problem);
            }

            Assert.AreEqual(0, problems.Length);
            Assert.AreEqual(3, process.DataSets.First().Rows.Count);

            var rows = ProcessFactory.CreateSingle(process, new TestLogger()).Execute().ToList();

            Assert.AreEqual(3, rows.Count());
        }
    }
}
