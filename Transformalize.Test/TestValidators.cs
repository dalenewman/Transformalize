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
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
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
                .Row().Field("f1", "{\"i\":\"am\", \"valid\":true}").Field("out", null)
                .Row().Field("f1", "{\"i\":\"have\", \"a\":\"Prob\"lem\"}").Field("out", null).ToOperation();
            var isJson = new JsonValidatorOperation("f1", "out", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual(true, rows[0]["out"]);
            Assert.AreEqual(false, rows[1]["out"]);
        }

        [Test]
        public void ParseJson2() {
            const string goodJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", \"scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";
            const string badJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";

            var input = new RowsBuilder().Row().Field("f1", goodJson).Field("o1", "").Row().Field("f1", badJson).Field("o1", "").ToOperation();
            var isJson = new JsonValidatorOperation("f1", "out", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual(true, rows[0]["out"]);
            Assert.AreEqual(false, rows[1]["out"]);
        }

        [Test]
        public void ContainsCharacters() {
            var input = new RowsBuilder()
                .Row("f1", "test")
                .Row("f1", "abcd").ToOperation();

            var containsCharacters = new ContainsCharactersValidatorOperation("f1", "o1", "abc", Libs.EnterpriseLibrary.Validation.Validators.ContainsCharacters.All, false);

            var output = TestOperation(input, containsCharacters);

            Assert.AreEqual(false, output[0]["o1"]);
            Assert.AreEqual(true, output[1]["o1"]);
        }

        [Test]
        public void StartsWith() {
            var input = new RowsBuilder().Row("f1", "test").Row("f1", "abcd").ToOperation();
            var startsWith = new StartsWithValidatorOperation("f1", "abc", "out", false);

            var output = TestOperation(input, startsWith);

            Assert.IsFalse((bool) output[0]["out"]);
            Assert.IsTrue((bool)output[1]["out"]);
        }

        [Test]
        public void DateTimeRange() {
            var goodDate = new DateTime(2013, 6, 2);
            var badDate = new DateTime(2011, 1, 13);
            var startDate = new DateTime(2013, 1, 1, 0, 0, 0, 001);
            var endDate = new DateTime(2013, 12, 31, 23, 59, 59, 998);

            var input = new RowsBuilder()
                .Row("f1", goodDate)
                .Row("f1", badDate).ToOperation();
            var dateTimeRange = new DateTimeRangeValidatorOperation("f1", "o1", startDate, RangeBoundaryType.Inclusive, endDate, RangeBoundaryType.Inclusive, false);

            var output = TestOperation(input, dateTimeRange);

            Assert.AreEqual(true, output[0]["o1"]);
            Assert.AreEqual(false, output[1]["o1"]);
        }

        [Test]
        public void Domain() {
            var input = new RowsBuilder()
                .Row("in", "2").Field("out", null)
                .Row("in", "4").Field("out", null).ToOperation();

            var domainOperation = new DomainValidatorOperation("in", "out", new[] { "1", "2", "3" }, false);

            var output = TestOperation(input, domainOperation);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);

        }

        [Test]
        public void DomainReturningMessage() {

            var input = new RowsBuilder()
                .Row("in", 2)
                .Row("in", "3").ToOperation();

            var xml = @"
<cfg>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' provider='internal' />
                <add name='output' provider='internal' />
            </connections>
            <entities>
                <add name='entity'>
                    <fields>
                        <add name='in' type='int' />
                    </fields>
                    <calculated-fields>
                        <add name='out'>
                            <transforms>
                                <add method='domain' domain='1,2,3' parameter='in' />
                            </transforms>
                        </add>
                    </calculated-fields>
                </add>
            </entities>
        </add>
    </processes>    
</cfg>
".Replace('\'', '"');

            var process = ProcessFactory.Create(xml)[0];
            process.Entities[0].InputOperation = input;
            var output = process.Execute().ToArray();

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);
        }


        [Test]
        public void DomainReturningIsValid() {

            var input = new RowsBuilder()
                .Row("in", 2)
                .Row("in", 4).ToOperation();

            var xml = @"
<cfg>
    <processes>
        <add name='process'>
            <connections>
                <add name='input' provider='internal' />
                <add name='output' provider='internal' />
            </connections>
            <entities>
                <add name='entity'>
                    <fields>
                        <add name='in' type='int' />
                    </fields>
                    <calculated-fields>
                        <add name='out' type='bool'>
                            <transforms>
                                <add method='domain' domain='1,2,3' parameter='in' />
                            </transforms>
                        </add>
                    </calculated-fields>
                </add>
            </entities>
        </add>
    </processes>    
</cfg>
".Replace('\'', '"');

            var process = ProcessFactory.Create(xml)[0];
            process.Entities[0].InputOperation = input;
            var output = process.Execute().ToArray();

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);
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
                    .CalculatedField("is-dale")
                        .Type("bool")
                        .Transform("domain")
                            .Domain("Dale")
                            .Parameter("name")
                    .CalculatedField("new-name")
                        .Transform("copy")
                            .Parameter("name")
                            .Branch("DaleBranch")
                                .RunIf("is-dale", true)
                                    .Transform("toUpper")
                                        .Parameter("name")
                            .Branch("VladBranch")
                                .RunIf("is-dale", false)
                                    .Transform("toLower")
                                        .Parameter("name")

                .Process();

            var process = ProcessFactory.Create(cfg)[0];
            var output = process.Execute().ToArray();

            Assert.AreEqual("DALE", output[0]["new-name"]);
            Assert.AreEqual("vlad", output[1]["new-name"]);
            Assert.AreEqual("tara", output[2]["new-name"]);
        }



        [Test]
        public void NotNull() {

            var input = new RowsBuilder()
                .Row("in", "x").Field("out", null)
                .Row("in", null).Field("out", null).ToOperation();

            var notNullOperation = new NotNullValidatorOperation("in", "out", false);

            var output = TestOperation(input, notNullOperation);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);

        }

        [Test]
        public void PropertyComparison() {
            var input = new RowsBuilder()
                .Row("in1", "77").Field("in2", null).Field("out", "")
                .Row("in1", "78").Field("in2", "78").Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "Equal", false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(false, output[0]["out"]);
            Assert.AreEqual(true, output[1]["out"]);
        }

        [Test]
        public void PropertyComparisonGreaterThanNumbers() {
            var input = new RowsBuilder()
                .Row("in1", 59).Field("in2", 57).Field("out", "")
                .Row("in1", 47).Field("in2", 47).Field("out", "").ToOperation();

            var validator = new PropertyComparisonValidatorOperation("in1", "in2", "out", "GreaterThan", false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);
        }

        [Test]
        public void Range() {

            var input = new RowsBuilder()
                .Row("in", 5).Field("out", "")
                .Row("in", 2).Field("out", "").ToOperation();

            var validator = new RangeValidatorOperation("in", "out", 3, RangeBoundaryType.Inclusive, 9, RangeBoundaryType.Inclusive, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);

        }

        [Test]
        public void Regex() {
            var input = new RowsBuilder()
                .Row("in", "789A").Field("out", "")
                .Row("in", "hjd7").Field("out", "").ToOperation();

            var validator = new RegexValidatorOperation("in", "out", @"^7[0-9]{2}A$", false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);

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
                -1,
                DateTimeUnit.Day,
                RangeBoundaryType.Inclusive,
                1,
                DateTimeUnit.Day,
                RangeBoundaryType.Inclusive,
                false
            );

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);
        }

        [Test]
        public void StringLength() {
            var input = new RowsBuilder()
                .Row("in", "something").Field("out", string.Empty)
                .Row("in", "some").Field("out", string.Empty).ToOperation();

            var validator = new StringLengthValidatorOperation("in", "out", 5, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);
        }

        [Test]
        public void TypeConversion() {

            var input = new RowsBuilder()
                .Row("in", "9999-12-31").Field("out", string.Empty)
                .Row("in", "10/32/2001").Field("out", string.Empty).ToOperation();

            var type = Common.ToSystemType("datetime");

            var validator = new TypeConversionValidatorOperation("in", "out", type, false, false);

            var output = TestOperation(input, validator);

            Assert.AreEqual(true, output[0]["out"]);
            Assert.AreEqual(false, output[1]["out"]);

        }


    }
}