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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Validate;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestValidators : EtlProcessHelper {

        [Test]
        public void ParseJson() {
            var input = new RowsBuilder()
                .Row().Field("f1", "{\"i\":\"am\", \"valid\":true}").Field("o1", "")
                .Row().Field("f1", "{\"i\":\"have\", \"a\":\"Prob\"lem\"}").Field("o1", "").ToOperation();
            var isJson = new JsonValidatorOperation("f1", "o1", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual("", rows[0]["o1"]);
            Assert.AreEqual("Could not find token at index 23.", rows[1]["o1"]);
        }

        [Test]
        public void ParseJson2() {
            const string goodJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", \"scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";
            const string badJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";

            var input = new RowsBuilder().Row().Field("f1", goodJson).Field("o1", "").Row().Field("f1", badJson).Field("o1", "").ToOperation();
            var isJson = new JsonValidatorOperation("f1", "o1", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual("", rows[0]["o1"]);
            Assert.AreEqual("Could not find token at index 800.", rows[1]["o1"]);
        }

        [Test]
        public void ContainsCharacters() {
            var input = new RowsBuilder().Row("f1", "test").Row("f1", "abcd").ToOperation();
            var containsCharacters = new ContainsCharactersValidatorOperation("f1", "o1", "abc", Libs.EnterpriseLibrary.Validation.Validators.ContainsCharacters.All, "{2} doesn't have abc in it! Your value of '{0}' sucks.", false, false);

            var output = TestOperation(input, containsCharacters);

            Assert.AreEqual("f1 doesn't have abc in it! Your value of 'test' sucks.", output[0]["o1"]);
            Assert.AreEqual(null, output[1]["o1"]);
        }

        [Test]
        public void DateTimeRange() {
            var goodDate = new DateTime(2013, 6, 2);
            var badDate = new DateTime(2011, 1, 13);
            var startDate = new DateTime(2013, 1, 1, 0, 0, 0, 001);
            var endDate = new DateTime(2013, 12, 31, 23, 59, 59, 998);

            var input = new RowsBuilder().Row("f1", goodDate).Row("f1", badDate).ToOperation();
            var dateTimeRange = new DateTimeRangeValidatorOperation("f1", "o1", startDate, RangeBoundaryType.Inclusive, endDate, RangeBoundaryType.Inclusive, "Bad!", false, true);

            var output = TestOperation(input, dateTimeRange);

            Assert.AreEqual(null, output[0]["o1"]);
            Assert.AreEqual("Bad!", output[1]["o1"]);
        }

        [Test]
        public void Domain() {
            var input = new RowsBuilder()
                .Row("in", "2").Field("out", "")
                .Row("in", "4").Field("out", "").ToOperation();

            var domainOperation = new DomainValidatorOperation("in", "out", new[] { "1", "2", "3" }, "{0} is wrong! {2} can't be {0}.", false, false);

            var output = TestOperation(input, domainOperation);

            Assert.AreEqual("", output[0]["out"]);
            Assert.AreEqual("4 is wrong! in can't be 4.", output[1]["out"]);

        }

        [Test]
        public void NotNull() {

            var input = new RowsBuilder()
                .Row("in", "x").Field("out", "")
                .Row("in", null).Field("out", "").ToOperation();

            var notNullOperation = new NotNullValidatorOperation("in", "out", "{2} can't be null.", false, false);

            var output = TestOperation(input, notNullOperation);

            Assert.AreEqual("", output[0]["out"]);
            Assert.AreEqual("in can't be null.", output[1]["out"]);

        }

        [Test]
        public void PropertyComparison() {
            var input = new RowsBuilder()
                .Row("in1", "77").Field("in2", null).Field("out", "")
                .Row("in1", "78").Field("in2", "78").Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "Equal", "Bad!", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual("Bad!", output[0]["out"]);
            Assert.AreEqual("", output[1]["out"]);
        }

        [Test]
        public void PropertyComparisonGreaterThanNumbers() {
            var input = new RowsBuilder()
                .Row("in1", 59).Field("in2", 57).Field("out", "")
                .Row("in1", 47).Field("in2", 47).Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "GreaterThan", "{0} Bad!", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual("", output[0]["out"]);
            Assert.AreEqual("47 Bad!", output[1]["out"]);
        }

        [Test]
        public void Range() {

            var input = new RowsBuilder()
                .Row("in", 5).Field("out", "")
                .Row("in", 2).Field("out", "").ToOperation();

            var validator = new RangeValidatorOperation("in", "out", 3, RangeBoundaryType.Inclusive, 9, RangeBoundaryType.Inclusive, "{0} is not between and {3} and {5}.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual("", output[0]["out"]);
            Assert.AreEqual("2 is not between and 3 and 9.", output[1]["out"]);

        }

        [Test]
        public void Regex() {
            var input = new RowsBuilder()
                .Row("in", "789A").Field("out", "")
                .Row("in", "hjd7").Field("out", "").ToOperation();

            var validator = new RegexValidatorOperation("in", "out", @"^7[0-9]{2}A$", "{0} is no match.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual("", output[0]["out"]);
            Assert.AreEqual("hjd7 is no match.", output[1]["out"]);

        }

    }
}