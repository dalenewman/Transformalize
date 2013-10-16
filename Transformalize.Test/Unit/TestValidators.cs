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

using NUnit.Framework;
using Transformalize.Operations.Validate;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestValidators : EtlProcessHelper {

        [Test]
        public void JsonParse() {
            var input = new RowsBuilder()
                .Row().Field("f1", "{\"i\":\"am\", \"valid\":true}").Field("o1","")
                .Row().Field("f1", "{\"i\":\"have\", \"a\":\"Prob\"lem\"}").Field("o1","").ToOperation();
            var isJson = new JsonParseOperation("f1", "o1", false);

            var rows = TestOperation(input, isJson);

            Assert.AreEqual("", rows[0]["o1"]);
            Assert.AreEqual("Could not find token at index 23 in f1's contents: {\"i\":\"have\", \"a\":\"Prob\"lem\"}.", rows[1]["o1"]);
        }

    }
}