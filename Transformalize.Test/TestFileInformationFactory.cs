using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Transformalize.Main.Providers.File;

namespace Transformalize.Test {

    [TestFixture]
    public class TestFileInformationFactory {

        [Test]
        public void TestExcel() {
            const string fileName = @"TestFiles\Headers\Headers.xlsx";
            var request = new FileInspectionRequest(fileName);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header 2", actual.Fields[1].Name);
        }

        [Test]
        public void TestCommas() {
            const string fileName = @"TestFiles\Headers\Headers.csv";
            var request = new FileInspectionRequest(fileName) { LineLimit = 3 };
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(',', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header 2", actual.Fields[1].Name);
        }

        [Test]
        public void TestPipes() {
            const string fileName = @"TestFiles\Headers\Headers.psv";
            var request = new FileInspectionRequest(fileName);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual('|', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header 2", actual.Fields[1].Name);
        }

        [Test]
        public void TestTabs() {
            const string fileName = @"TestFiles\Headers\Headers.tsv";
            var request = new FileInspectionRequest(fileName);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual('\t', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header 2", actual.Fields[1].Name);
        }

        [Test]
        public void TestSingleColumn() {
            const string fileName = @"TestFiles\Headers\Single.txt";
            var request = new FileInspectionRequest(fileName);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(default(char), actual.Delimiter);
            Assert.AreEqual(1, actual.ColumnCount());
            Assert.AreEqual("Header 1", actual.Fields[0].Name);
            Assert.AreEqual("1024", actual.Fields[0].Length);
        }

        [Test]
        public void TestMultipleDelimiters() {

            var file = Path.GetTempFileName();
            File.WriteAllText(file, @"f|1,f|2,f|3,f|4,f|5
v|1,v|;2,v|3,v|4,v|5
v|6,v|;7,v|8,v9,v|10,
v|11,v|;12,v|13,v|14,v|15");

            var request = new FileInspectionRequest(file);
            var actual = FileInformationFactory.Create(request);

            foreach (
                var delimiter in
                    request.Delimiters.Select(p => p.Value)
                        .Where(d => d.AveragePerLine > 0)
                        .OrderBy(d => d.CoefficientOfVariance())) {
                Console.WriteLine("Delimiter: `{0}` CoV: {1} Average: {2} StdDev: {3}", delimiter.Character, delimiter.CoefficientOfVariance(), delimiter.AveragePerLine, delimiter.StandardDeviation);
            }

            Assert.AreEqual(6, actual.Fields.Count);
            Assert.AreEqual('|', actual.Delimiter);

            Assert.AreEqual("A", actual.Fields[0].Name);
            Assert.AreEqual("B", actual.Fields[1].Name);
            Assert.AreEqual("C", actual.Fields[2].Name);
            Assert.AreEqual("D", actual.Fields[3].Name);
            Assert.AreEqual("E", actual.Fields[4].Name);
            Assert.AreEqual("F", actual.Fields[5].Name);

        }


        [Test]
        public void TestFieldQuotedCsv() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"State,Population,Shape
MI,""10,000,000"",Mitten
CA,""20,000,000"",Sock,
KS,""9,000,000"",Rectangle");

            var request = new FileInspectionRequest(file);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(3, actual.Fields.Count);

            Assert.AreEqual("State", actual.Fields[0].Name);
            Assert.AreEqual("Population", actual.Fields[1].Name);
            Assert.AreEqual("Shape", actual.Fields[2].Name);

            Assert.AreEqual("string", actual.Fields[0].Type);
            Assert.AreEqual("string", actual.Fields[1].Type);
            Assert.AreEqual("string", actual.Fields[2].Type);

            Assert.IsTrue(actual.Fields[0].IsQuoted());
            Assert.IsTrue(actual.Fields[1].IsQuoted());
            Assert.AreEqual('\"', actual.Fields[1].QuotedWith);
            Assert.IsTrue(actual.Fields[2].IsQuoted());

            Assert.AreEqual("1024", actual.Fields[0].Length);
            Assert.AreEqual("1024", actual.Fields[1].Length);
            Assert.AreEqual("1024", actual.Fields[2].Length);
        }

        [Test]
        public void TestCsvWithJustHeaders() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"State,Population,Shape");

            var request = new FileInspectionRequest(file);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(3, actual.Fields.Count);

            Assert.AreEqual("State", actual.Fields[0].Name);
            Assert.AreEqual("Population", actual.Fields[1].Name);
            Assert.AreEqual("Shape", actual.Fields[2].Name);

            Assert.AreEqual("string", actual.Fields[0].Type);
            Assert.AreEqual("string", actual.Fields[1].Type);
            Assert.AreEqual("string", actual.Fields[2].Type);

            Assert.IsTrue(actual.Fields[0].IsQuoted());
            Assert.IsTrue(actual.Fields[1].IsQuoted());
            Assert.IsTrue(actual.Fields[2].IsQuoted());

            Assert.AreEqual("1024", actual.Fields[0].Length);
            Assert.AreEqual("1024", actual.Fields[1].Length);
            Assert.AreEqual("1024", actual.Fields[2].Length);
        }

        [Test]
        public void TestEmptyCsv() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, string.Empty);

            var request = new FileInspectionRequest(file);
            var actual = FileInformationFactory.Create(request);

            Assert.AreEqual(0, actual.Fields.Count);
        }

    }
}
