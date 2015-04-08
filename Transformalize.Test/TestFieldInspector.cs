using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Transformalize.Main.Providers.File;

namespace Transformalize.Test {

    [TestFixture]
    public class TestFieldInspector {

        [Test]
        public void TestFieldQuotedCsv() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"""State"",""Population"",""Shape""
MI,""10,000,000"",Mitten
CA,""20,000,000"",Sock
KS,""9,000,000"",Rectangle");

            var request = new FileInspectionRequest(file) { DataTypes = new List<string> { "decimal" } };
            var fileInformation = FileInformationFactory.Create(request);
            var actual = new FieldInspector().Inspect(fileInformation, request);

            Assert.AreEqual(3, actual.Count);

            Assert.AreEqual("State", actual[0].Name);
            Assert.AreEqual("Population", actual[1].Name);
            Assert.AreEqual("Shape", actual[2].Name);

            Assert.AreEqual("string", actual[0].Type);
            Assert.AreEqual("decimal", actual[1].Type);
            Assert.AreEqual("string", actual[2].Type);

            Assert.AreEqual('\"', actual[0].QuotedWith);
            Assert.AreEqual('\"', actual[1].QuotedWith);
            Assert.AreEqual('\"', actual[2].QuotedWith);

            Assert.AreEqual("3", actual[0].Length);
            Assert.AreEqual("1024", actual[1].Length);
            Assert.AreEqual("10", actual[2].Length);
        }

        [Test]
        public void TestFieldQuotedCsv2() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"Field 1,Field 2,Field 3,Field 4,Field 5,Field 6
""0007282100"",""O721"",20,""1111 COUNTY RD 1 SOUTH POINT OH 11111"",,""1""
""0007382201"",""O722"",25,""2222 COUNTY RD 1 SOUTH POINT OH 22222"",,""1""
""0007482302"",""O723"",30,""3333 COUNTY RD 1 SOUTH POINT OH 33333"",,""1""
");
            var info = FileInformationFactory.Create(file);
            var actual = new FieldInspector().Inspect(info);

            Assert.AreEqual(6, actual.Count);

            Assert.AreEqual("Field 1", actual[0].Name);
            Assert.AreEqual("Field 2", actual[1].Name);
            Assert.AreEqual("Field 3", actual[2].Name);
            Assert.AreEqual("Field 4", actual[3].Name);
            Assert.AreEqual("Field 5", actual[4].Name);
            Assert.AreEqual("Field 6", actual[5].Name);

            Assert.AreEqual("int", actual[0].Type);
            Assert.AreEqual("string", actual[1].Type);
            Assert.AreEqual("int", actual[2].Type);
            Assert.AreEqual("string", actual[3].Type);
            Assert.AreEqual("string", actual[4].Type);
            Assert.AreEqual("int", actual[5].Type);
        }


        [Test]
        public void TestIssue001A() {
            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"t1,t2,t3,t4
Monday,10,1.1,1/1/2014
Tuesday,11,2.2,2/1/2014
Wednesday,12,3.3,3/1/2014
Thursday,13,4.4,4/1/2014
Friday,14,5.5,5/1/2014
Saturday,15,6.6,6/1/2014");


            var request = new FileInspectionRequest(file) { DataTypes = new List<string> { "int32", "double", "datetime" } };
            var information = FileInformationFactory.Create(request);
            var fields = new FieldInspector().Inspect(information, request).ToArray();

            Assert.AreEqual("string", fields[0].Type);
            Assert.AreEqual("int32", fields[1].Type);
            Assert.AreEqual("double", fields[2].Type);
            Assert.AreEqual("datetime", fields[3].Type);

            //really do it
            //new FileImporter().Import(new FileInfo(file), request);

        }

        [Test]
        public void TestIssue002B() {

            const string file = @"TestFiles\Headers\Issue002.xlsx";

            var request = new FileInspectionRequest(file) { DataTypes = new List<string> { "int32", "datetime" } };
            var information = FileInformationFactory.Create(request);
            var fields = new FieldInspector().Inspect(information, request).ToArray();

            Assert.AreEqual("string", fields[0].Type);
            Assert.AreEqual("int32", fields[1].Type);
            Assert.AreEqual("string", fields[2].Type);
            Assert.AreEqual("datetime", fields[3].Type);

        }

        [Test]
        public void TestCsvBlanks() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"t1,t2,t3,t4,t5
""Monday"",10,""1.1"",1/1/2014,
""Tuesday"",11,""2.2"",2/1/2014,
""Wednesday"",12,""3.3"",3/1/2014,
""Thursday"",13,""4.4"",4/1/2014,
""Friday"",14,,5/1/2014,
""Saturday"",15,,6/1/2014,");

            var request = new FileInspectionRequest(file) {
                DataTypes = new List<string> { "int32", "double", "datetime" },
                IgnoreEmpty = true
            };
            var information = FileInformationFactory.Create(request);
            var fields = new FieldInspector().Inspect(information, request).ToArray();

            Assert.AreEqual('"', fields[0].QuotedWith);
            Assert.AreEqual("string", fields[0].Type);
            Assert.AreEqual("int32", fields[1].Type);
            Assert.AreEqual('"', fields[2].QuotedWith);
            Assert.AreEqual("double", fields[2].Type);
            Assert.AreEqual("datetime", fields[3].Type);
            Assert.AreEqual("string", fields[4].Type);
            Assert.AreEqual("1", fields[4].Length);

        }


        [Test]
        public void TestFalseBoolean() {
            var file = Path.GetTempFileName();
            const string contents = @"34149771
34150506
34148432
39844261
34149561
65313203
76674170
83513345
85501135
55975869
89604649
72561004
94461202
71436264
39905508
29692312
a
b
c";
            File.WriteAllText(file, contents);

            var fields = new FieldInspector().Inspect(file).ToArray();
            Assert.AreEqual("string", fields[0].Type);
        }

    }
}
