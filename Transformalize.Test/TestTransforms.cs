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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Test.Builders;

namespace Transformalize.Test {
    [TestFixture]
    public class TestTransforms : EtlProcessHelper {

        [Test]
        public void Sample() {
            var input = new RowsBuilder()
                .Row().Field("f1", 1)
                .Row().Field("f1", 2)
                .Row().Field("f1", 3)
                .Row().Field("f1", 4)
                .Row().Field("f1", 5)
                .Row().Field("f1", 6)
                .Row().Field("f1", 7)
                .Row().Field("f1", 8)
                .Row().Field("f1", 9)
                .Row().Field("f1", 10)
                .Row().Field("f1", 11)
                .Row().Field("f1", 12)
                .ToOperation();

            var sampler = new SampleOperation2(50);

            var rows = TestOperation(input, sampler);

            Console.WriteLine(rows.Count);
            Assert.Greater(11, rows.Count);
            Assert.Less(1, rows.Count);


        }

        [Test]
        public void ConcatStrings() {
            var input = new RowsBuilder().Row().Field("f1", "v1").Field("f2", "v2").ToOperation();
            var parameters = new ParametersBuilder().Parameters("f1", "f2").ToParameters();
            var concat = new ConcatOperation("o1", parameters);

            var rows = TestOperation(input, concat);

            Assert.AreEqual("v1v2", rows[0]["o1"]);
        }

        [Test]
        public void ConcatNumbers() {
            var input = new RowsBuilder().Row().Field("f1", 1).Field("f2", 2).ToOperation();
            var parameters = new ParametersBuilder().Parameter("f1").Type("int32").Parameter("f2").Type("int32").ToParameters();
            var concat = new ConcatOperation("o1", parameters);

            var rows = TestOperation(input, concat);

            Assert.AreEqual("12", rows[0]["o1"]);
        }

        [Test]
        public void ConvertStringToNumber() {
            var input = new RowsBuilder().Row().Field("f1", "1").ToOperation();
            var convert = new ConvertOperation("f1", "string", "o1", "int32");

            var rows = TestOperation(input, convert);

            Assert.AreEqual(1, rows[0]["o1"]);
        }

        [Test]
        public void ConvertStringToDateTime() {
            var input = new RowsBuilder().Row().Field("f1", "2013-11-07").ToOperation();
            var convert = new ConvertOperation("f1", "string", "o1", "datetime", "yyyy-MM-dd");

            var rows = TestOperation(input, convert);

            Assert.AreEqual(new DateTime(2013, 11, 7), rows[0]["o1"]);
        }

        [Test]
        public void Copy() {
            var input = new RowsBuilder().Row().Field("f1", 7).ToOperation();
            var copy = new CopyOperation("f1", "f2");
            var output = TestOperation(input, copy);
            Assert.AreEqual(7, output[0]["f2"]);
        }

        [Test]
        public void Distance() {

            var input = new RowsBuilder().Row().Field("toLat", 28.419385d).Field("toLong", -81.581234d).ToOperation();

            var fromLat = new ParametersBuilder().Parameter("fromLat", 42.101025d).Type("double").ToParameters()[0];
            var fromLong = new ParametersBuilder().Parameter("fromLong", -86.48423d).Type("double").ToParameters()[0];
            var toLat = new ParametersBuilder().Parameter("toLat").ToParameters()[0];
            var toLong = new ParametersBuilder().Parameter("toLong").ToParameters()[0];

            var distance = new DistanceOperation("o1", "miles", fromLat, fromLong, toLat, toLong);

            var rows = TestOperation(input, distance);

            Assert.AreEqual(985.32863773508757d, rows[0]["o1"]);
        }

        [Test]
        public void Format() {
            var input = new RowsBuilder().Row().Field("f1", true).Field("f2", 8).ToOperation();
            var parameters = new ParametersBuilder().Parameters("f1", "f2").ToParameters();
            var expression = new FormatOperation("o1", "{0} and {1:c}.", parameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual("True and $8.00.", rows[0]["o1"]);
        }

        [Test]
        public void SplitIndex() {
            var input = new RowsBuilder()
                .Row()
                    .Field("f1", "a couple words").Field("f2", "01:02:00")
                .ToOperation();
            var splindex1 = new SplitIndexOperation("f1", "f1", "string", " ", 0, 1);
            var splindex2 = new SplitIndexOperation("f2", "f2", "int", ":", 0, 1);

            var rows = TestOperation(input, splindex1, splindex2);

            Assert.AreEqual("couple", rows[0]["f1"]);
            Assert.AreEqual(2, rows[0]["f2"]);
        }


        [Test]
        public void FromJson() {
            var input = new RowsBuilder().Row().Field("f1", "{ \"j1\":\"v1\", \"j2\":7, \"array\":[{\"x\":1}] }").ToOperation();
            var outParameters = new ParametersBuilder().Parameter("j1").Parameter("j2").Type("int32").Parameter("array").ToParameters();
            var expression = new FromJsonOperation("f1", outParameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual("v1", rows[0]["j1"]);
            Assert.AreEqual(7, rows[0]["j2"]);
        }

        [Test]
        public void FromRegex() {
            var input = new RowsBuilder().Row().Field("f1", "991.1 #Something INFO and a rambling message.").ToOperation();
            var outParameters = new ParametersBuilder().Parameter("p1").Type("decimal").Parameter("p2").Parameter("p3").ToParameters();
            var fromRegex = new FromRegexOperation("f1", @"(?<p1>^[\d\.]+).*(?<p2> [A-Z]{4,5} )(?<p3>.*$)", outParameters);

            var rows = TestOperation(input, fromRegex);

            Assert.AreEqual(991.1M, rows[0]["p1"]);
            Assert.AreEqual(" INFO ", rows[0]["p2"]);
            Assert.AreEqual("and a rambling message.", rows[0]["p3"]);
        }

        [Test]
        public void FromXml() {
            var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line product=\"1\"/><line product=\"2\"/></lines></order>").ToOperation();

            var outFields = new FieldsBuilder()
                .Field("id").Type("int32")
                .Field("total").Type("decimal")
                .Field("lines")
                .ToFields();

            var fromXml = new FromXmlOperation("f1", outFields);

            var rows = TestOperation(input, fromXml);

            Assert.AreEqual(1, rows[0]["id"]);
            Assert.AreEqual(7.25M, rows[0]["total"]);
            Assert.AreEqual("<line product=\"1\" /><line product=\"2\" />", rows[0]["lines"]);
        }

        [Test]
        public void FromNanoXmlInner() {
            var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line product=\"1\"/><line product=\"2\"/></lines></order>").ToOperation();

            var outFields = new FieldsBuilder()
                .Field("id").Type("int32")
                .Field("total").Type("decimal")
                .Field("product").Type("int32").NodeType("attribute")
                .Field("lines")
                .ToFields();

            var fromXml = new FromNanoXmlOperation("f1", outFields);

            var rows = TestOperation(input, fromXml);

            Assert.AreEqual(1, rows[0]["id"]);
            Assert.AreEqual(7.25M, rows[0]["total"]);
            Assert.AreEqual("<line product=\"1\"></line><line product=\"2\"></line>", rows[0]["lines"]);
            Assert.AreEqual(2, rows[0]["product"]);
        }

        [Test]
        public void FromNanoXmlOuter() {
            var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line product=\"1\"/><line product=\"2\"/></lines></order>").ToOperation();

            var outFields = new FieldsBuilder()
                .Field("id").Type("int32")
                .Field("total").Type("decimal")
                .Field("product").Type("int32").NodeType("attribute")
                .Field("lines").ReadInnerXml(false)
                .ToFields();

            var fromXml = new FromNanoXmlOperation("f1", outFields);

            var rows = TestOperation(input, fromXml);

            Assert.AreEqual(1, rows[0]["id"]);
            Assert.AreEqual(7.25M, rows[0]["total"]);
            Assert.AreEqual("<lines><line product=\"1\"></line><line product=\"2\"></line></lines>", rows[0]["lines"]);
            Assert.AreEqual(2, rows[0]["product"]);
        }


        [Test]
        public void FromFirstXmlOuter() {
            var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line product=\"1\"/><line product=\"2\"/></lines></order>").ToOperation();

            var outFields = new FieldsBuilder()
                .Field("id").Type("int32")
                .Field("total").Type("decimal")
                .Field("product").Type("int32").NodeType("attribute")
                .ToFields();

            var fromXml = new FromFirstXmlOperation("f1", outFields);

            var rows = TestOperation(input, fromXml);

            Assert.AreEqual(1, rows[0]["id"]);
            Assert.AreEqual(7.25M, rows[0]["total"]);
            Assert.AreEqual(1, rows[0]["product"]);
        }


        [Test]
        public void FromXmlDoublePass() {
            var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line product=\"1\"/><line product=\"2\"/></lines></order>").ToOperation();

            var outFields = new FieldsBuilder()
                .Field("id").Type("int32")
                .Field("total").Type("decimal")
                .Field("lines").ReadInnerXml(false)
                .ToFields();

            var fromXml = new FromXmlOperation("f1", outFields);

            var output = TestOperation(input, fromXml);

            Assert.AreEqual(1, output[0]["id"]);
            Assert.AreEqual(7.25M, output[0]["total"]);
            Assert.AreEqual("<lines><line product=\"1\" /><line product=\"2\" /></lines>", output[0]["lines"]);

            //second pass to get lines
            input = new RowsBuilder(output).ToOperation();
            outFields = new FieldsBuilder().Field("product").Type("int32").NodeType("attribute").ToFields();
            fromXml = new FromXmlOperation("lines", outFields);
            output = TestOperation(input, fromXml);

            Assert.AreEqual(1, output[0]["id"]);
            Assert.AreEqual(7.25M, output[0]["total"]);
            Assert.AreEqual(1, output[0]["product"]);

            Assert.AreEqual(1, output[1]["id"]);
            Assert.AreEqual(7.25M, output[1]["total"]);
            Assert.AreEqual(2, output[1]["product"]);
        }

        [Test]
        public void FromXmlWithMultipleRecords() {

            const string xml = @"
                <order>
                    <detail product-id=""1"" quantity=""2"" color=""red"" />
                    <detail product-id=""3"" quantity=""1"" color=""pink"" />
                </order>
            ";

            var input = new RowsBuilder().Row().Field("xml", xml).ToOperation();

            var outFields = new FieldsBuilder()
                .Field("product-id").Type("int32").NodeType("attribute")
                .Field("quantity").Type("int32").NodeType("attribute")
                .Field("color").Type("string").NodeType("attribute")
                .ToFields();

            var fromXmlTransform = new FromXmlOperation("xml", outFields);

            var output = TestOperation(input, fromXmlTransform);

            Assert.AreEqual(1, output[0]["product-id"]);
            Assert.AreEqual(2, output[0]["quantity"]);
            Assert.AreEqual("red", output[0]["color"]);

            Assert.AreEqual(3, output[1]["product-id"]);
            Assert.AreEqual(1, output[1]["quantity"]);
            Assert.AreEqual("pink", output[1]["color"]);

        }

        [Test]
        public void GetHashCodeTest() {
            var expected = "test".GetHashCode();

            var input = new RowsBuilder().Row("f1", "test").ToOperation();
            var getHashCode = new GetHashCodeOperation("f1", "o1");
            var output = TestOperation(input, getHashCode);

            Assert.AreEqual(expected, output[0]["o1"]);
        }

        [Test]
        public void If() {

            var input = new RowsBuilder()
                .Row("x", 5).Field("y", 7).Field("z", 10).Field("out", 0)
                .Row("x", 5).Field("y", 5).Field("z", 11).Field("out", 0).ToOperation();

            var parameters = new ParametersBuilder()
                .Parameter("x").Type("int32")
                .Parameter("y").Type("int32")
                .Parameter("z").Type("int32")
                .Parameter("v", "1").Name("v").Type("int32")
                .ToParameters();

            var ifTransform = new IfOperation(parameters["x"], ComparisonOperator.Equal, parameters["y"], parameters["z"], parameters["v"], "out", "int32");

            var output = TestOperation(input, ifTransform);

            Assert.AreEqual(1, output[0]["out"]);
            Assert.AreEqual(11, output[1]["out"]);
        }

        [Test]
        public void IfEmpty() {
            var input = new RowsBuilder()
                .Row("x", "x").Field("y", "").Field("out", "")
                .Row("x", "").Field("y", "y").Field("out", "").ToOperation();

            var parameters = new ParametersBuilder()
                .Parameter("x")
                .Parameter("y")
                .Parameter("empty", string.Empty).Name("empty")
                .ToParameters();

            var ifTransform = new IfOperation(parameters["x"], ComparisonOperator.Equal, parameters["empty"], parameters["y"], parameters["x"], "out", "string");

            var output = TestOperation(input, ifTransform);

            Assert.AreEqual("x", output[0]["out"]);
            Assert.AreEqual("y", output[1]["out"]);
        }

        [Test]
        public void Insert() {
            var input = new RowsBuilder().Row("f1", "Insertere").ToOperation();
            var insert = new InsertOperation("f1", "o1", 6, " H");
            var output = TestOperation(input, insert);

            Assert.AreEqual("Insert Here", output[0]["o1"]);
        }

        [Test]
        public void InsertInterval() {
            var input = new RowsBuilder().Row("date", "140607").ToOperation();
            var insertInterval = new InsertIntervalOperation("date", "date", 2, "-");
            var insert = new InsertOperation("date", "date", 0, "20");
            var output = TestOperation(input, insertInterval, insert);

            Assert.AreEqual("2014-06-07", output[0]["date"]);
        }

        [Test]
        public void Javascript() {
            const int expected = 12;

            var input = new RowsBuilder().Row("x", 3).Field("y", 4).ToOperation();
            var scripts = new Dictionary<string, Script>() { { "script", new Script("script", "function multiply(x,y) { return x*y; }", "") } };
            var parameters = new ParametersBuilder().Parameters("x", "y").ToParameters();
            var javascript = new JavascriptOperation("o1", "multiply(x,y)", scripts, parameters);
            var output = TestOperation(input, javascript);

            Assert.AreEqual(expected, output[0]["o1"]);
        }

        [Test]
        public void JavascriptWithDates() {
            const string minuteDiff = @"
                function minuteDiff(orderStatus, start, end) {
                    if (orderStatus == 'Completed' || orderStatus == 'Problematic') {

                        var answer = 0;
                        if (start.getFullYear() != 9999 && end.getFullYear() != 9999) {
                            var ms = Math.abs(start - end);
                            answer = Math.round((ms / 1000) / 60);
                        }

                        return answer > 60 ? 60 : answer;

                    } else {
                        return 0;
                    }
                }";

            var input = new RowsBuilder()
                .Row("OrderStatus", "Completed")
                    .Field("StartDate", DateTime.Now.AddMinutes(-30.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-29.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-78.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "x")
                    .Field("StartDate", DateTime.Now.AddMinutes(-20.0))
                    .Field("EndDate", DateTime.Now)
                .ToOperation();

            var scripts = new Dictionary<string, Script>() { { "script", new Script("script", minuteDiff, "") } };
            var parameters = new ParametersBuilder().Parameters("OrderStatus", "StartDate", "EndDate").ToParameters();
            var javascript = new JavascriptOperation("o1", "minuteDiff(OrderStatus,StartDate,EndDate);", scripts, parameters);
            var output = TestOperation(input, javascript);

            Assert.AreEqual(30, output[0]["o1"]);
            Assert.AreEqual(29, output[1]["o1"]);
            Assert.AreEqual(60, output[2]["o1"]);
            Assert.AreEqual(0, output[3]["o1"]);
        }

        [Test]
        public void JavascriptInProcess() {
            var script = Path.GetTempFileName();
            File.WriteAllText(script, @"
                function minuteDiff(orderStatus, start, end) {
                    if (orderStatus == 'Completed' || orderStatus == 'Problematic') {

                        var answer = 0;

                        if (start.getFullYear() != 9999 && end.getFullYear() != 9999) {
                            var ms = Math.abs(start - end);
                            answer = Math.round((ms / 1000) / 60);
                        }

                        return answer > 60 ? 60 : answer;

                    } else {
                        return 0;
                    }
                }");

            var input = new RowsBuilder()
                .Row("OrderStatus", "Completed")
                    .Field("StartDate", DateTime.Now.AddMinutes(-30.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-29.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-78.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "x")
                    .Field("StartDate", DateTime.Now.AddMinutes(-20.0))
                    .Field("EndDate", DateTime.Now)
                .ToOperation();

            var config = new ProcessBuilder("test")
                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")
                .Script("script", script)
                .Entity("test").InputOperation(input)
                    .Field("OrderStatus")
                    .Field("StartDate").DateTime()
                    .Field("EndDate").DateTime()
                    .CalculatedField("MinuteDiff").Int()
                        .Transform("javascript")
                            .ExternalScript("script")
                            .Script("minuteDiff(OrderStatus,StartDate,EndDate)")
                                .Parameter("OrderStatus")
                                .Parameter("StartDate")
                                .Parameter("EndDate")
            .Process();

            var process = ProcessFactory.Create(config, new Options() { LogLevel = LogLevel.Debug })[0];

            var output = process.Execute()["test"].ToList();

            Assert.AreEqual(30, output[0]["MinuteDiff"]);
            Assert.AreEqual(29, output[1]["MinuteDiff"]);
            Assert.AreEqual(60, output[2]["MinuteDiff"]);
            Assert.AreEqual(0, output[3]["MinuteDiff"]);
        }


        [Test]
        public void CSharpScript() {
            var input = new RowsBuilder()
                .Row("first", "dale").Field("last", "newman").Field("full", "")
                .Row("first", "adam").Field("last", "newman").Field("full", "")
                .ToOperation();

            var parameters = new ParametersBuilder().Parameters("first", "last").ToParameters();
            var scripts = new Dictionary<string, Script>();

            const string code = @"
                return first + "" "" + last;
            ";

            var cSharp = new CSharpOperation("full", "string", code, scripts, parameters);
            var output = TestOperation(input, cSharp);
            Assert.AreEqual("dale newman", output[0]["full"].ToString());
            Assert.AreEqual("adam newman", output[1]["full"].ToString());

        }

        [Test]
        public void CSharpInProcess() {
            var script = Path.GetTempFileName();
            File.WriteAllText(script, @"
                public static class F {
                    public static int MinuteDiff(string orderStatus, DateTime start, DateTime end) {
                        if (orderStatus != ""Completed"" && orderStatus != ""Problematic"")
                            return 0;
                        var answer = Convert.ToInt32(Math.Round(end.Subtract(start).TotalMinutes, 0));
                        return answer > 60 ? 60 : answer;
                    }
                }
            ");

            var input = new RowsBuilder()
                .Row("OrderStatus", "Completed")
                    .Field("StartDate", DateTime.Now.AddMinutes(-30.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-29.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-78.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "x")
                    .Field("StartDate", DateTime.Now.AddMinutes(-20.0))
                    .Field("EndDate", DateTime.Now)
                .ToOperation();

            var config = new ProcessBuilder("test")
                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")
                .Script("script", script)
                .Entity("test").InputOperation(input)
                    .Field("OrderStatus")
                    .Field("StartDate").DateTime()
                    .Field("EndDate").DateTime()
                    .CalculatedField("MinuteDiff").Int()
                        .Transform("csharp")
                            .ExternalScript("script")
                            .Script("return F.MinuteDiff(OrderStatus,StartDate,EndDate);")
                                .Parameter("OrderStatus")
                                .Parameter("StartDate")
                                .Parameter("EndDate")
            .Process();

            var process = ProcessFactory.Create(config, new Options() { LogLevel = LogLevel.Debug })[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var output = process.Execute()["test"].ToList();

            Assert.AreEqual(30, output[0]["MinuteDiff"]);
            Assert.AreEqual(29, output[1]["MinuteDiff"]);
            Assert.AreEqual(60, output[2]["MinuteDiff"]);
            Assert.AreEqual(0, output[3]["MinuteDiff"]);
        }

        [Test]
        public void CSharpInProcessInLine() {

            var input = new RowsBuilder()
                .Row("OrderStatus", "Completed")
                    .Field("StartDate", DateTime.Now.AddMinutes(-30.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-29.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "Problematic")
                    .Field("StartDate", DateTime.Now.AddMinutes(-78.0))
                    .Field("EndDate", DateTime.Now)
                .Row("OrderStatus", "x")
                    .Field("StartDate", DateTime.Now.AddMinutes(-20.0))
                    .Field("EndDate", DateTime.Now)
                .ToOperation();

            var config = new ProcessBuilder("test")
                .Connection("input").Provider("internal")
                .Connection("output").Provider("internal")
                .Entity("test").InputOperation(input)
                    .Field("OrderStatus")
                    .Field("StartDate").DateTime()
                    .Field("EndDate").DateTime()
                    .CalculatedField("MinuteDiff").Int()
                        .Transform("csharp")
                            .Script(@"
                                var answer = 0;
                                if (OrderStatus == 'Completed' || OrderStatus == 'Problematic'){
                                    answer = Convert.ToInt32(Math.Round(EndDate.Subtract(StartDate).TotalMinutes, 0));
                                }
                                return answer > 60 ? 60 : answer;
                            ")
                            .Parameter("OrderStatus")
                            .Parameter("StartDate")
                            .Parameter("EndDate")
            .Process();

            var process = ProcessFactory.Create(config, new Options() { LogLevel = LogLevel.Debug })[0];
            process.PipelineThreading = PipelineThreading.SingleThreaded;
            var output = process.Execute()["test"].ToList();

            Assert.AreEqual(30, output[0]["MinuteDiff"]);
            Assert.AreEqual(29, output[1]["MinuteDiff"]);
            Assert.AreEqual(60, output[2]["MinuteDiff"]);
            Assert.AreEqual(0, output[3]["MinuteDiff"]);
        }


        [Test]
        public void Join() {
            var input = new RowsBuilder().Row("x", "X").Field("y", "Y").ToOperation();
            var parameters = new ParametersBuilder().Parameters("x", "y").ToParameters();
            var join = new JoinTransformOperation("o1", "|", parameters);
            var output = TestOperation(input, join);
            Assert.AreEqual("X|Y", output[0]["o1"]);
        }

        [Test]
        public void Left() {
            var input = new RowsBuilder().Row("left", "left").ToOperation();
            var left = new LeftOperation("left", "o1", 3);
            var output = TestOperation(input, left);
            Assert.AreEqual("lef", output[0]["o1"]);
        }

        [Test]
        public void XPathByAttributeValue() {
            var input = new RowsBuilder().Row("xml", "<items><item id=\"1\">one</item><item id=\"2\">two</item><item id=\"3\">three</item></items>").ToOperation();
            var transform = new XPathOperation("xml", "value", "string", "items/item[@id = \"2\"]");
            var output = TestOperation(input, transform);
            Assert.AreEqual("two", output[0]["value"]);
        }

        [Test]
        public void XPathByElementValue() {
            var input = new RowsBuilder().Row("xml", "<items><item id=\"1\">one</item><item id=\"2\">two</item><item id=\"3\">three</item></items>").ToOperation();
            var transform = new XPathOperation("xml", "value", "int", "items/item[. = \"one\"]/@id");
            var output = TestOperation(input, transform);
            Assert.AreEqual(1, output[0]["value"]);
        }

        [Test]
        public void XPathSample() {
            var input = new RowsBuilder().Row("xml", "<items><item ro=\"False\"><FcThMtrTypeCodeOld>KW</FcThMtrTypeCodeOld><FcThMtrRdgOld>0.39</FcThMtrRdgOld></item><item ro=\"False\"><FcThMtrTypeCodeOld>KWH</FcThMtrTypeCodeOld><FcThMtrRdgOld>66448</FcThMtrRdgOld></item></items>").ToOperation();
            var transform = new XPathOperation("xml", "value", "string", "items/item[FcThMtrTypeCodeOld = \"KWH\"]/FcThMtrRdgOld");
            var output = TestOperation(input, transform);
            Assert.AreEqual("66448", output[0]["value"]);
        }


        [Test]
        public void Length() {
            var input = new RowsBuilder().Row("left", "left").ToOperation();
            var length = new LengthOperation("left", "o1");
            var output = TestOperation(input, length);
            Assert.AreEqual(4, output[0]["o1"]);
        }

        [Test]
        public void MapEquals() {
            var input = new RowsBuilder().Row("f1", "x").Row("f1", "a").Row("f1", "d").ToOperation();
            var maps = new MapsBuilder()
                .Equals().Item("x", "y").Item("a", "b")
                .StartsWith()
                .EndsWith().ToMaps();
            var map = new MapOperation("f1", "o1", "string", maps);
            var output = TestOperation(input, map);
            Assert.AreEqual("y", output[0]["o1"], "x maps to y");
            Assert.AreEqual("b", output[1]["o1"], "a maps to b");
            Assert.AreEqual("d", output[2]["o1"], "d stays d");
        }

        [Test]
        public void MapStartsWith() {

            var input = new RowsBuilder().Row("f1", "test1").Row("f1", "test2").Row("f1", "tes").ToOperation();

            var maps = new MapsBuilder()
                .Equals().Item("*", "no")
                .StartsWith().Item("test", "yes")
                .EndsWith().ToMaps();

            var map = new MapOperation("f1", "o1", "string", maps);

            var output = TestOperation(input, map);

            Assert.AreEqual("yes", output[0]["o1"], "test1 maps to yes");
            Assert.AreEqual("yes", output[1]["o1"], "test2 maps to yes");
            Assert.AreEqual("no", output[2]["o1"], "test maps to no (via catch-all)");
        }

        [Test]
        public void MapEndsWith() {

            var input = new RowsBuilder()
                .Row("f1", "1end")
                .Row("f1", "2end")
                .Row("f1", "start").ToOperation();

            var maps = new MapsBuilder()
                .Equals().Item("*", "no")
                .StartsWith()
                .EndsWith().Item("end", "yes").ToMaps();

            var mapTransform = new MapOperation("f1", "o1", "string", maps);

            var output = TestOperation(input, mapTransform);

            Assert.AreEqual("yes", output[0]["o1"]);
            Assert.AreEqual("yes", output[1]["o1"]);
            Assert.AreEqual("no", output[2]["o1"]);

        }

        [Test]
        public void MapEqualsWithParameter() {

            var input = new RowsBuilder()
                .Row("f1", "v1").Field("p1", 1).Field("p2", 2).Field("p3", 3)
                .Row("f1", "v2").Field("p1", 1).Field("p2", 2).Field("p3", 3)
                .Row("f1", "v3").Field("p1", 1).Field("p2", 2).Field("p3", 3)
                .ToOperation();

            var maps = new MapsBuilder()
                .Equals().Item("v1", null, "p1").Item("v2", null, "p2").Item("*", null, "p3")
                .StartsWith()
                .EndsWith()
                .ToMaps();

            var mapTransform = new MapOperation("f1", "o1", "int32", maps);

            var output = TestOperation(input, mapTransform);

            Assert.AreEqual(1, output[0]["o1"], "v1 maps to 1");
            Assert.AreEqual(2, output[1]["o1"], "v2 maps to 2");
            Assert.AreEqual(3, output[2]["o1"], "v3 maps to 3 (via catch-all)");
        }

        [Test]
        public void PadLeft() {
            var input = new RowsBuilder().Row("in", "23").Field("out", "").ToOperation();

            var padLeftTransform = new PadLeftOperation("in", "out", 5, "X");

            var output = TestOperation(input, padLeftTransform);

            Assert.AreEqual("XXX23", output[0]["out"]);

        }

        [Test]
        public void PadRight() {
            var input = new RowsBuilder().Row("in", "23").Field("out", "").ToOperation();

            var padRightTransform = new PadRightOperation("in", "out", 5, "X");

            var output = TestOperation(input, padRightTransform);

            Assert.AreEqual("23XXX", output[0]["out"]);

        }

        [Test]
        public void RegexReplace() {
            const int all = 0;
            var input = new RowsBuilder().Row("in", "int32").Field("out", "").ToOperation();

            const string digitPattern = @"\d";
            var regexReplaceTransform = new RegexReplaceOperation("in", "out", digitPattern, "X", all);

            var output = TestOperation(input, regexReplaceTransform);

            Assert.AreEqual("intXX", output[0]["out"]);
        }

        [Test]
        public void Remove() {
            var input = new RowsBuilder().Row("in", "sdfkj2334").Field("out", "").ToOperation();
            var removeTransform = new RemoveOperation("in", "out", 4, 2);
            var output = TestOperation(input, removeTransform);
            Assert.AreEqual("sdfk334", output[0]["out"]);
        }

        [Test]
        public void Replace() {
            var input = new RowsBuilder().Row("in", "sdfkj2334").Field("out", "").ToOperation();
            var replaceTransform = new ReplaceOperation("in", "out", "fkj", ".");
            var output = TestOperation(input, replaceTransform);
            Assert.AreEqual("sd.2334", output[0]["out"]);
        }

        [Test]
        public void Right() {
            var input = new RowsBuilder().Row("in", "sdfkj2334").Field("out", "").ToOperation();
            var rightOperation = new RightOperation("in", "out", 3);
            var output = TestOperation(input, rightOperation);
            Assert.AreEqual("334", output[0]["out"]);

        }

        [Test]
        public void Substring() {
            var input = new RowsBuilder().Row("in", "sdfkj2334").Field("out", "").ToOperation();
            var substringOperation = new SubstringOperation("in", "out", 3, 0);
            var output = TestOperation(input, substringOperation);
            Assert.AreEqual("kj2334", output[0]["out"]);
        }

        [Test]
        public void SubstringWithLength() {
            var input = new RowsBuilder().Row("in", "sdfkj2334").Field("out", "").ToOperation();
            var substringOperation = new SubstringOperation("in", "out", 3, 4);
            var output = TestOperation(input, substringOperation);
            Assert.AreEqual("kj23", output[0]["out"]);
        }

        [Test]
        public void Template() {
            var input = new RowsBuilder().Row("input", 2).Field("out", "").ToOperation();
            var templates = new List<KeyValuePair<string, Template>>();
            var parameters = new ParametersBuilder().Parameter("x", 3).Parameter("input").ToParameters();
            var templateOperation = new TemplateOperation("out", "string", "@{var result = Model.input * Model.x;}@result", "dynamic", templates, parameters);
            var output = TestOperation(input, templateOperation);
            Assert.AreEqual("6", output[0]["out"]);
        }

        [Test]
        public void TemplateInt() {
            var input = new RowsBuilder().Row("input", 2).Field("out", "").ToOperation();
            var templates = new List<KeyValuePair<string, Template>>();
            var parameters = new ParametersBuilder().Parameter("x", 3).Parameter("input").ToParameters();
            var templateOperation = new TemplateOperation("out", "int", "@{var result = Model.input * Model.x;}@result", "dynamic", templates, parameters);
            var output = TestOperation(input, templateOperation);
            Assert.AreEqual(6, output[0]["out"]);
        }

        [Test]
        public void ToJson() {
            var input = new RowsBuilder().Row("f1", 1).Field("f2", "2").Field("out", "").ToOperation();
            var parameters = new ParametersBuilder().Parameters("f1", "f2").ToParameters();
            var toJsonOperation = new ToJsonOperation("out", parameters);
            var output = TestOperation(input, toJsonOperation);
            Assert.AreEqual("{\"f1\":1,\"f2\":\"2\"}", output[0]["out"]);
        }

        [Test]
        public void ToLocalTime() {
            var now = DateTime.UtcNow;
            var local = DateTime.Now;

            var input = new RowsBuilder().Row("time", now).ToOperation();
            var transform = new ToLocalTimeOperation("time", "time", "UTC", "Eastern Standard Time");
            var output = TestOperation(input, transform);
            Assert.AreEqual(local, output[0]["time"], "Change to your time zone to get this to pass.");
        }

        [Test]
        public void ToLocalTimeDefaults() {
            var now = DateTime.UtcNow;
            var local = DateTime.Now;

            var input = new RowsBuilder().Row("time", now).ToOperation();
            var transform = new ToLocalTimeOperation("time", "time", "", "");
            var output = TestOperation(input, transform);
            Assert.AreEqual(local, output[0]["time"], "Change to your time zone to get this to pass.");
        }

        [Test]
        public void ToLower() {
            var input = new RowsBuilder().Row("name", "DalE").ToOperation();
            var transform = new ToLowerOperation("name", "name");
            var output = TestOperation(input, transform);
            Assert.AreEqual("dale", output[0]["name"]);
        }

        [Test]
        public void ToStringFromDate() {
            var date = new DateTime(2013, 10, 30);
            var input = new RowsBuilder().Row("date", date).ToOperation();
            var transform = new ToStringOperation("date", "datetime", "date", "yyyy-MM-dd");
            var output = TestOperation(input, transform);
            Assert.AreEqual("2013-10-30", output[0]["date"]);
        }

        [Test]
        public void ToStringFromInt() {
            var input = new RowsBuilder().Row("number", 43).ToOperation();
            var transform = new ToStringOperation("number", "int32", "number", "C");
            var output = TestOperation(input, transform);
            Assert.AreEqual("$43.00", output[0]["number"]);
        }

        [Test]
        public void ToUpper() {
            var input = new RowsBuilder().Row("name", "DalE").ToOperation();
            var transform = new ToUpperOperation("name", "name");
            var output = TestOperation(input, transform);
            Assert.AreEqual("DALE", output[0]["name"]);
        }

        [Test]
        public void ToTitleCase() {
            var input = new RowsBuilder().Row("x", "TRANSFORMALIZE").ToOperation();
            var transform = new ToTitleCaseOperation("x", "x");
            var output = TestOperation(input, transform);
            Assert.AreEqual("Transformalize", output[0]["x"]);

        }

        [Test]
        public void TrimEnd() {
            var input = new RowsBuilder().Row("y", "SomeTH").ToOperation();
            var transform = new TrimEndOperation("y", "y", "TH");
            var output = TestOperation(input, transform);
            Assert.AreEqual("Some", output[0]["y"]);
        }

        [Test]
        public void Trim() {
            var input = new RowsBuilder().Row("y", "..Some,").ToOperation();
            var transform = new TrimOperation("y", "y", ",.");
            var output = TestOperation(input, transform);
            Assert.AreEqual("Some", output[0]["y"]);
        }

        [Test]
        public void TrimStart() {
            var input = new RowsBuilder().Row("y", "&dTest").ToOperation();
            var transform = new TrimStartOperation("y", "y", "&d");
            var output = TestOperation(input, transform);
            Assert.AreEqual("Test", output[0]["y"]);
        }

        [Test]
        public void TrimStartAppend() {
            var input = new RowsBuilder().Row("OrderNumber", "C000012").ToOperation();
            var transform = new TrimStartAppendOperation("OrderNumber", "OrderNumber", "C0", " ");
            var output = TestOperation(input, transform);
            Assert.AreEqual("C000012 12", output[0]["OrderNumber"]);
        }

        [Test]
        public void TimeOfDaySeconds() {
            var input = new RowsBuilder().Row("d", DateTime.Parse("1/1/2013 00:01:07.000")).ToOperation();
            var transform = new TimeOfDayOperation("d", "datetime", "d", "double", "seconds");
            var output = TestOperation(input, transform);
            Assert.AreEqual(67, output[0]["d"]);
        }

        [Test]
        public void TimeOfDayMinutes() {
            var input = new RowsBuilder().Row("d", DateTime.Parse("1/1/2013 01:19:00.000")).ToOperation();
            var transform = new TimeOfDayOperation("d", "datetime", "d", "double", "minutes");
            var output = TestOperation(input, transform);
            Assert.AreEqual(79, output[0]["d"]);
        }

        [Test]
        public void TestCompress() {
            var input = new RowsBuilder().Row("in", "Hello World Hello World Hello World Hello World Hello World Hello World Hello World").ToOperation();
            var compress = new CompressOperation("in", "out");
            var output = TestOperation(input, compress);
            var normal = output[0]["in"].ToString();
            var compressed = output[0]["out"].ToString();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("UwAAAB+LCAAAAAAABADzSM3JyVcIzy/KSVHwoA4bAOnjsbBTAAAA", compressed);
            Assert.Less(compressed.Length, normal.Length);
        }

        [Test]
        public void TestDecompress() {
            var input = new RowsBuilder().Row("in", "UwAAAB+LCAAAAAAABADzSM3JyVcIzy/KSVHwoA4bAOnjsbBTAAAA").ToOperation();
            var decompress = new DecompressOperation("in", "out");
            var output = TestOperation(input, decompress);
            var compressed = output[0]["in"].ToString();
            var normal = output[0]["out"].ToString();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("Hello World Hello World Hello World Hello World Hello World Hello World Hello World", normal);
            Assert.Greater(normal.Length, compressed.Length);
        }


        [Test]
        public void TestCombo() {
            var input = new RowsBuilder()
                .Row("MeterNumber", "R00001")
                .Row("MeterNumber", "000002").ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Field("MeterNumber")
                    .CalculatedField("MeterCategory").Default("None")
                        .Transform("left").Length(1).Parameter("MeterNumber")
                        .Transform("if").Left("MeterCategory").Right("R").Then("Reclaim").Else("Domestic")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];
            var output = process.Execute()["entity"].ToList();

            Assert.AreEqual("Reclaim", output[0]["MeterCategory"]);
            Assert.AreEqual("Domestic", output[1]["MeterCategory"]);
        }

    }
}