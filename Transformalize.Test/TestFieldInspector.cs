using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Main.Providers.File;

namespace Transformalize.Test {

    [TestFixture]
    public class TestFieldInspector {

        [Test]
        public void TestFieldQuotedCsv() {

            var file = Path.GetTempFileName().Replace(".tmp", ".csv");
            File.WriteAllText(file, @"State,Population,Shape
MI,""10,000,000"",Mitten
CA,""20,000,000"",Sock
KS,""9,000,000"",Rectangle");

            var request = new FileInspectionRequest {DataTypes = new List<string> {"decimal"}};
            var fileInformation = FileInformationFactory.Create(new FileInfo(file), request);
            var actual = new FieldInspector().Inspect(fileInformation, request);

            Assert.AreEqual(3, actual.Count);

            Assert.AreEqual("State", actual[0].Name);
            Assert.AreEqual("Population", actual[1].Name);
            Assert.AreEqual("Shape", actual[2].Name);

            Assert.AreEqual("string", actual[0].Type);
            Assert.AreEqual("decimal", actual[1].Type);
            Assert.AreEqual("string", actual[2].Type);

            Assert.AreEqual(default(char), actual[0].Quote);
            Assert.AreEqual('\"', actual[1].Quote);
            Assert.AreEqual(default(char), actual[2].Quote);

            Assert.AreEqual("3", actual[0].Length);
            Assert.AreEqual(string.Empty, actual[1].Length);
            Assert.AreEqual("10", actual[2].Length);
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


            var request = new FileInspectionRequest {DataTypes = new List<string> {"int32", "double", "datetime"}};
            var information = FileInformationFactory.Create(new FileInfo(file), request);
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

            var request = new FileInspectionRequest {DataTypes = new List<string> {"int32", "datetime"}};
            var information = FileInformationFactory.Create(new FileInfo(file), request);
            var fields = new FieldInspector().Inspect(information, request).ToArray();

            Assert.AreEqual("string", fields[0].Type);
            Assert.AreEqual("int32", fields[1].Type);
            Assert.AreEqual("string", fields[2].Type);
            Assert.AreEqual("datetime", fields[3].Type);

        }

        [Test]
        public void TestFalseBoolean()
        {
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

            var request = new FileInspectionRequest();
            var information = FileInformationFactory.Create(file, request);
            var fields = new FieldInspector().Inspect(information, request).ToArray();

            Assert.AreEqual("string", fields[0].Type);

        }


    }
}
