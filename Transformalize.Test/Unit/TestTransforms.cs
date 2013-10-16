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
using NUnit.Framework;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestTransforms : EtlProcessHelper {
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
        public void Distance() {

            var input = new RowsBuilder().Row().Field("toLat", 28.419385d).Field("toLong", -81.581234d).ToOperation();
            var parameters = new ParametersBuilder()
                .Parameter("fromLat").Type("double").Value(42.101025d).Parameter("fromLong").Type("double").Value(-86.48423d)
                .Parameter("toLat").Type("double").Parameter("toLong").Type("double")
                .ToParameters();

            var distance = new DistanceOperation("o1", "miles", parameters);

            var rows = TestOperation(input, distance);

            Assert.AreEqual(985.32863773508757d, rows[0]["o1"]);
        }

        [Test]
        public void ExpressionFunction() {
            var input = new RowsBuilder().Row().Field("f1", 4).ToOperation();
            var parameters = new ParametersBuilder().Parameter("f1").ToParameters();
            var expression = new ExpressionOperation("o1", "Sqrt(f1)", parameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual(2d, rows[0]["o1"]);
        }

        [Test]
        public void ExpressionIf() {
            var input = new RowsBuilder().Row().Field("f1", 4).ToOperation();
            var parameters = new ParametersBuilder().Parameter("f1").ToParameters();
            var expression = new ExpressionOperation("o1", "if(f1 = 4, true, false)", parameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual(true, rows[0]["o1"]);
        }

        [Test]
        public void Format() {
            var input = new RowsBuilder().Row().Field("f1", true).Field("f2", 8).ToOperation();
            var parameters = new ParametersBuilder().Parameters("f1", "f2").ToParameters();
            var expression = new FormatOperation("o1", "{0} and {1}.", parameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual("True and 8.", rows[0]["o1"]);
        }

        [Test]
        public void FromJson() {
            var input = new RowsBuilder().Row().Field("f1", "{ \"j1\":\"v1\", \"j2\":7 }").ToOperation();
            var outParameters = new ParametersBuilder().Parameter("j1").Parameter("j2").Type("int32").ToParameters();
            var expression = new FromJsonOperation("f1", false, outParameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual("v1", rows[0]["j1"]);
            Assert.AreEqual(7, rows[0]["j2"]);
        }

        [Test]
        public void FromJsonWithExtraDoubleQuotes() {
            var input = new RowsBuilder().Row().Field("f1", "{ \"j1\":\"v\"1\", \"j2\"\":7 }").ToOperation();
            var outParameters = new ParametersBuilder().Parameter("j1").Parameter("j2").Type("int32").ToParameters();
            var expression = new FromJsonOperation("f1", true, outParameters);

            var rows = TestOperation(input, expression);

            Assert.AreEqual("v1", rows[0]["j1"]);
            Assert.AreEqual(7, rows[0]["j2"]);
        }

        [Test]
        public void FromRegex()
        {
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
            var outParameters = new ParametersBuilder().Parameter("id").Type("int32").Parameter("total").Type("decimal").Parameter("lines").ToParameters();
            var fromXml = new FromXmlOperation("f1", outParameters);

            var rows = TestOperation(input, fromXml);

            Assert.AreEqual(1, rows[0]["id"]);
            Assert.AreEqual(7.25M, rows[0]["total"]);
            Assert.AreEqual("<line product=\"1\" /><line product=\"2\" />", rows[0]["lines"]);
        }

        //[Test]
        //public void FromDeeperXml() {
        //    var input = new RowsBuilder().Row().Field("f1", "<order><id>1</id><total>7.25</total><lines><line>1</line><line>2</line></lines></order>").ToOperation();
        //    var outParameters = new ParametersBuilder().Parameter("id").Type("int32").Parameter("total").Type("decimal").Parameter("lines").ToParameters();
        //    var fromXml = new FromXmlOperation("f1", outParameters);

        //    var outParametersDeeper = new ParametersBuilder().Parameter("line").ToParameters();
        //    var format = new FormatOperation("lines", "<lines>{0}</lines>", new ParametersBuilder().Parameter("lines").ToParameters());
        //    var fromXmlDeeper = new FromXmlOperation("lines", outParametersDeeper);


        //    var rows = TestOperation(input, fromXml, format, fromXmlDeeper);

        //    Assert.AreEqual(1, rows[0]["id"]);
        //    Assert.AreEqual(7.25M, rows[0]["total"]);
        //    Assert.AreEqual("<line product=\"1\" /><line product=\"2\" />", rows[0]["lines"]);
        //}

    }
}