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
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations.Validate;
using Transformalize.Test.Builders;

namespace Transformalize.Test {
    [TestFixture]
    public class TestValidators : EtlProcessHelper {

        [Test]
        public void ParseJson() {
            var input = new RowsBuilder()
                .Row().Field("f1", "{\"i\":\"am\", \"valid\":true}").Field("o1", "")
                .Row().Field("f1", "{\"i\":\"have\", \"a\":\"Prob\"lem\"}").Field("o1", "").ToOperation();
            var isJson = new JsonValidatorOperation("f1", "o1", "o2", "{2} in {0}.", false, false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual(true, rows[0]["o1"]);
            Assert.AreEqual(null, rows[0]["o2"]);
            Assert.AreEqual(false, rows[1]["o1"]);
            Assert.AreEqual("Could not find token at index 23 in f1.", rows[1]["o2"]);
        }

        [Test]
        public void ParseJson2() {
            const string goodJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", \"scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";
            const string badJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";

            var input = new RowsBuilder().Row().Field("f1", goodJson).Field("o1", "").Row().Field("f1", badJson).Field("o1", "").ToOperation();
            var isJson = new JsonValidatorOperation("f1", "o1", "o2", "{2} in {0}.",false, false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual(null, rows[0]["o2"]);
            Assert.AreEqual("Could not find token at index 800 in f1.", rows[1]["o2"]);
        }

        [Test]
        public void ContainsCharacters() {
            var input = new RowsBuilder().Row("f1", "test").Row("f1", "abcd").ToOperation();
            var containsCharacters = new ContainsCharactersValidatorOperation("f1", "o1", "o2", "abc", Libs.EnterpriseLibrary.Validation.Validators.ContainsCharacters.All, "{2} doesn't have abc in it! Your value of '{0}' sucks.", false, false);

            var output = TestOperation(input, containsCharacters);

            Assert.AreEqual("f1 doesn't have abc in it! Your value of 'test' sucks.", output[0]["o2"]);
            Assert.AreEqual(null, output[1]["o2"]);
        }

        [Test]
        public void StartsWith() {
            var input = new RowsBuilder().Row("f1", "test").Row("f1", "abcd").ToOperation();
            var startsWith = new StartsWithValidatorOperation("f1", "abc", "result", "message", "{0} doesn't start with {2}.  It is {1}.",false, false);

            var output = TestOperation(input, startsWith);

            Assert.AreEqual("f1 doesn't start with abc.  It is test.", output[0]["message"]);
            Assert.IsFalse((bool) output[0]["result"]);
            Assert.IsTrue((bool)output[1]["result"]);
        }

        [Test]
        public void DateTimeRange() {
            var goodDate = new DateTime(2013, 6, 2);
            var badDate = new DateTime(2011, 1, 13);
            var startDate = new DateTime(2013, 1, 1, 0, 0, 0, 001);
            var endDate = new DateTime(2013, 12, 31, 23, 59, 59, 998);

            var input = new RowsBuilder().Row("f1", goodDate).Row("f1", badDate).ToOperation();
            var dateTimeRange = new DateTimeRangeValidatorOperation("f1", "o1", "o2", startDate, RangeBoundaryType.Inclusive, endDate, RangeBoundaryType.Inclusive, "Bad!", false, true);

            var output = TestOperation(input, dateTimeRange);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("Bad!", output[1]["o2"]);
        }

        [Test]
        public void Domain() {
            var input = new RowsBuilder()
                .Row("in", "2").Field("out", "")
                .Row("in", "4").Field("out", "").ToOperation();

            var domainOperation = new DomainValidatorOperation("in", "out", "o2", new[] { "1", "2", "3" }, "{0} is wrong! {2} can't be {0}.", false, false);

            var output = TestOperation(input, domainOperation);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("4 is wrong! in can't be 4.", output[1]["o2"]);

        }

        [Test]
        public void DomainReturningMessage() {

            var input = new RowsBuilder()
                .Row("in", "2")
                .Row("in", "4").ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Field("in").Int32()
                    .CalculatedField("out")
                        .Transform("domain")
                            .Domain("1,2,3")
                            .MessageTemplate("{0} is wrong! {2} can't be {0}.")
                            .Parameter("in")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];
            var output = process.Run()["entity"].ToList();

            Assert.AreEqual(true, output[0]["outResult"]);
            Assert.AreEqual("4 is wrong! in can't be 4.", output[1]["outMessage"]);
        }


        [Test]
        public void DomainReturningIsValid() {

            var input = new RowsBuilder()
                .Row("in", 2)
                .Row("in", 4).ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Field("in").Int32()
                        .Transform("domain")
                            .Domain("1,2,3")
                            .ResultField("result")
                            .MessageField("message")
                .Process();

            var process = ProcessFactory.Create(cfg)[0];
            var output = process.Run()["entity"].ToList();

            Assert.AreEqual(true, output[0]["result"]);
            Assert.AreEqual(false, output[1]["result"]);
        }

        [Test]
        public void DomainWithBranches() {

            var input = new RowsBuilder()
                .Row("name", "Dale")
                .Row("name", "Vlad")
                .Row("name","Tara").ToOperation();

            var cfg = new ProcessBuilder("process")
                .Connection("input").Provider(ProviderType.Internal)
                .Connection("output").Provider(ProviderType.Internal)
                .Entity("entity")
                    .InputOperation(input)
                    .Field("name")
                    .Field("gender").Input(false).Default("male")
                    .CalculatedField("new-name")
                        .Transform("domain")
                            .Domain("Dale")
                            .Parameter("name")
                            .ResultField("is-dale")
                            .Branch("DaleBranch")
                                .RunIf("is-dale", true)
                                    .Transform("toUpper")
                                        .Parameter("name")
                            .Branch("VladBranch")
                                .RunIf("is-dale", false)
                                    .Transform("toLower")
                                        .Parameter("name")

                .Process();

            var crap = cfg.Serialize();
            var process = ProcessFactory.Create(cfg, new Options() { Mode = "test" })[0];
            var output = process.Run()["entity"].ToList();

            Assert.AreNotEqual(string.Empty, crap);
            Assert.AreEqual("DALE", output[0]["newname"]);
            Assert.AreEqual("vlad", output[1]["newname"]);
            Assert.AreEqual("tara", output[2]["newname"]);
        }



        [Test]
        public void NotNull() {

            var input = new RowsBuilder()
                .Row("in", "x").Field("out", "")
                .Row("in", null).Field("out", "").ToOperation();

            var notNullOperation = new NotNullValidatorOperation("in", "out", "o2", "{2} can't be null.", false, false);

            var output = TestOperation(input, notNullOperation);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("in can't be null.", output[1]["o2"]);

        }

        [Test]
        public void PropertyComparison() {
            var input = new RowsBuilder()
                .Row("in1", "77").Field("in2", null).Field("out", "")
                .Row("in1", "78").Field("in2", "78").Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "o2", "Equal", "Bad!", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual("Bad!", output[0]["o2"]);
            Assert.AreEqual(null, output[1]["o2"]);
        }

        [Test]
        public void PropertyComparisonGreaterThanNumbers() {
            var input = new RowsBuilder()
                .Row("in1", 59).Field("in2", 57).Field("out", "")
                .Row("in1", 47).Field("in2", 47).Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "o2", "GreaterThan", "{0} Bad!", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("47 Bad!", output[1]["o2"]);
        }

        [Test]
        public void Range() {

            var input = new RowsBuilder()
                .Row("in", 5).Field("out", "")
                .Row("in", 2).Field("out", "").ToOperation();

            var validator = new RangeValidatorOperation("in", "out", "o2", 3, RangeBoundaryType.Inclusive, 9, RangeBoundaryType.Inclusive, "{0} is not between and {3} and {5}.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("2 is not between and 3 and 9.", output[1]["o2"]);

        }

        [Test]
        public void Regex() {
            var input = new RowsBuilder()
                .Row("in", "789A").Field("out", "")
                .Row("in", "hjd7").Field("out", "").ToOperation();

            var validator = new RegexValidatorOperation("in", "out", "o2", @"^7[0-9]{2}A$", "{0} is no match.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("hjd7 is no match.", output[1]["o2"]);

        }

        [Test]
        public void RelativeDateTime() {
            var date = DateTime.Now;
            var badDate = DateTime.Now.AddDays(3);
            var badDateString = badDate.ToString("yyyy-MM-dd");

            var input = new RowsBuilder()
                .Row("in", date).Field("out", string.Empty)
                .Row("in", badDate).Field("out", string.Empty).ToOperation();

            var validator = new RelativeDateTimeValidatorOperation(
                "in",
                "out",
                "o2",
                -1,
                DateTimeUnit.Day,
                RangeBoundaryType.Inclusive,
                1,
                DateTimeUnit.Day,
                RangeBoundaryType.Inclusive,
                "I don't like {0:yyyy-MM-dd}!",
                false,
                false
            );

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("I don't like " + badDateString + "!", output[1]["o2"]);
        }

        [Test]
        public void StringLength() {
            var input = new RowsBuilder()
                .Row("in", "something").Field("out", string.Empty)
                .Row("in", "some").Field("out", string.Empty).ToOperation();

            var validator = new StringLengthValidatorOperation("in", "out", "o2", 5, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, "Sorry, the length of '{0}' must be between {3} and {5} characters long.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("Sorry, the length of 'some' must be between 5 and 10 characters long.", output[1]["o2"]);
        }

        [Test]
        public void TypeConversion() {

            var input = new RowsBuilder()
                .Row("in", "9999-12-31").Field("out", string.Empty)
                .Row("in", "10/32/2001").Field("out", string.Empty).ToOperation();

            var type = Common.ToSystemType("datetime");

            var validator = new TypeConversionValidatorOperation("in", "out", "o2", type, "Can't parse {0} to a {3}.", false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(null, output[0]["o2"]);
            Assert.AreEqual("Can't parse 10/32/2001 to a System.DateTime.", output[1]["o2"]);

        }


    }
}