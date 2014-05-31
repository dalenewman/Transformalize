using System.IO;
using NUnit.Framework;
using Transformalize.Main.Providers.File;

namespace Transformalize.Test {

    [TestFixture]
    public class TestFileInformationFactory {

        [Test]
        public void TestExcel()
        {
            var request = new FileInspectionRequest();
            var fileInfo = new FileInfo(@"TestFiles\Headers\Headers.xlsx");
            var actual = FileInformationFactory.Create(fileInfo, request);

            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header2", actual.Fields[1].Name);
        }

        [Test]
        public void TestCommas() {
            var request = new FileInspectionRequest();
            var fileInfo = new FileInfo(@"TestFiles\Headers\Headers.csv");
            var actual = FileInformationFactory.Create(fileInfo, request);

            Assert.AreEqual(',', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header2", actual.Fields[1].Name);
        }

        [Test]
        public void TestPipes() {
            var request = new FileInspectionRequest();
            var fileInfo = new FileInfo(@"TestFiles\Headers\Headers.psv");
            var actual = FileInformationFactory.Create(fileInfo, request);

            Assert.AreEqual('|', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header2", actual.Fields[1].Name);
        }

        [Test]
        public void TestTabs() {
            var request = new FileInspectionRequest();
            var fileInfo = new FileInfo(@"TestFiles\Headers\Headers.tsv");
            var actual = FileInformationFactory.Create(fileInfo, request);

            Assert.AreEqual('\t', actual.Delimiter);
            Assert.AreEqual(3, actual.ColumnCount());
            Assert.AreEqual("Header2", actual.Fields[1].Name);
        }

        [Test]
        public void TestSingleColumn() {
            var request = new FileInspectionRequest();
            var fileInfo = new FileInfo(@"TestFiles\Headers\Single.txt");
            var actual = FileInformationFactory.Create(fileInfo, request);

            Assert.AreEqual(default(char), actual.Delimiter);
            Assert.AreEqual(1, actual.ColumnCount());
            Assert.AreEqual("Header1", actual.Fields[0].Name);
            Assert.AreEqual("1024", actual.Fields[0].Length);
        }

        [Test]
        public void TestFieldQuotedCsv() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"State,Population,Shape
MI,""10,000,000"",Mitten
CA,""20,000,000"",Sock
KS,""9,000,000"",Rectangle");

            var request = new FileInspectionRequest();
            var actual = FileInformationFactory.Create(new FileInfo(file), request);

            Assert.AreEqual(3, actual.Fields.Count);

            Assert.AreEqual("State", actual.Fields[0].Name);
            Assert.AreEqual("Population", actual.Fields[1].Name);
            Assert.AreEqual("Shape", actual.Fields[2].Name);

            Assert.AreEqual("string", actual.Fields[0].Type);
            Assert.AreEqual("string", actual.Fields[1].Type);
            Assert.AreEqual("string", actual.Fields[2].Type);

            Assert.IsFalse(actual.Fields[0].IsQuoted());
            Assert.IsTrue(actual.Fields[1].IsQuoted());
            Assert.AreEqual('\"', actual.Fields[1].Quote);
            Assert.IsFalse(actual.Fields[2].IsQuoted());

            Assert.AreEqual("1024", actual.Fields[0].Length);
            Assert.AreEqual("1024", actual.Fields[1].Length);
            Assert.AreEqual("1024", actual.Fields[2].Length);
        }

    }
}
