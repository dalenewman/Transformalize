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
using Transformalize.Main.Parameters;
using Transformalize.Operations.Transform;
using Transformalize.Operations.Validate;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestValidators : EtlProcessHelper {

        [Test]
        public void JsonParse() {
            var input = new RowsBuilder()
                .Row().Field("f1", "{\"i\":\"am\", \"valid\":true}").Field("o1", "")
                .Row().Field("f1", "{\"i\":\"have\", \"a\":\"Prob\"lem\"}").Field("o1", "").ToOperation();
            var isJson = new ParseJsonOperation("f1", "o1", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual("", rows[0]["o1"]);
            Assert.AreEqual("Could not find token at index 23.", rows[1]["o1"]);
        }

        [Test]
        public void JsonParse2()
        {
            const string goodJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", \"scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";
            const string badJson = "{\"updated\": \"Thu, 10 Sep 2009 08:45:12 +0000\", \"links\": [{\"href\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"type\": \"text/html\", \"rel\": \"alternate\"}], \"title\": \"Photoshop Text Effects\", \"author\": \"hotpants1\", \"comments\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb\", \"guidislink\": false, \"title_detail\": {\"base\": \"http://feeds.delicious.com/v2/rss/recent?min=1&count=100\", \"type\": \"text/plain\", \"language\": null, \"value\": \"Photoshop Text Effects\"}, \"link\": \"http://www.designvitality.com/blog/2007/09/photoshop-text-effect-tutorial/\", \"source\": {}, \"wfw_commentrss\": \"http://feeds.delicious.com/v2/rss/url/90c1b26e451a090452df8b947d6298cb\", \"id\": \"http://delicious.com/url/90c1b26e451a090452df8b947d6298cb#hotpants1\", \"tags\": [{\"term\": \"photoshop\", scheme\": \"http://delicious.com/hotpants1/\", \"label\": null}]}";

            var input = new RowsBuilder().Row().Field("f1", goodJson).Field("o1", "").Row().Field("f1", badJson).Field("o1", "").ToOperation();
            var isJson = new ParseJsonOperation("f1", "o1", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual("", rows[0]["o1"]);
            Assert.AreEqual("Could not find token at index 800.", rows[1]["o1"]);
        }

        [Test]
        public void ContainsCharacters() {
            var input = new RowsBuilder().Row("f1", "test").Row("f1", "abcd").ToOperation();
            var containsCharacters = new ContainsCharactersOperation("f1", "o1", "abc", "all", "{2} doesn't have abc in it! Your value of '{0}' sucks.", false, false);

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
            var dateTimeRange = new DateTimeRangeOperation("f1", "o1", startDate, "inclusive", endDate, "inclusive", "Bad!", false, true);

            var output = TestOperation(input, dateTimeRange);

            Assert.AreEqual(null, output[0]["o1"]);
            Assert.AreEqual("Bad!",output[1]["o1"]);
        }


    }
}