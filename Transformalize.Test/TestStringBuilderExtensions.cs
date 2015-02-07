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

using System.Text;
using NUnit.Framework;
using Transformalize.Extensions;

namespace Transformalize.Test {
    [TestFixture]
    public class TestStringBuilderExtensions {
        [Test]
        public void TestToLower() {
            var sb = new StringBuilder(".Net");
            sb.ToLower();
            Assert.AreEqual(".net", sb.ToString());
        }

        [Test]
        public void TestLastIndexOf() {
            var sb = new StringBuilder("<strong>emphasis</strong>");
            Assert.AreEqual(16, sb.LastIndexOf('<'));
        }

        [Test]
        public void TestLastIndexOfNotThere() {
            var sb = new StringBuilder("<strong>emphasis</strong>");
            Assert.AreEqual(-1, sb.LastIndexOf('!'));
        }

        [Test]
        public void TestInsertFormat()
        {
            var sb = new StringBuilder("<strong>emphasis</strong>");
            sb.InsertFormat(sb.LastIndexOf('<')," and other {0}.", "things");
            Assert.AreEqual("<strong>emphasis and other things.</strong>", sb.ToString());
        }

        [Test]
        public void TestInsertFormatLikeAppend() {
            var sb = new StringBuilder("<strong>emphasis</strong>");
            sb.InsertFormat(26, " and other {0}.", "things");
            Assert.AreEqual("<strong>emphasis</strong> and other things.", sb.ToString());
        }

        [Test]
        public void TestInsertAt() {
            var sb = new StringBuilder("<strong>emphasis</strong>");
            var index = sb.LastIndexOf('<');
            sb.Insert(index, "!!!");
            Assert.AreEqual("<strong>emphasis!!!</strong>", sb.ToString());
        }

        [Test]
        public void TestToUpper() {
            var sb = new StringBuilder(".Net");
            sb.ToUpper();
            Assert.AreEqual(".NET", sb.ToString());
        }

        [Test]
        public void TestPush() {
            var expected = "GM250";
            var test = "250GM";
            var sb = new StringBuilder(test);
            sb.Push(char.IsNumber);
            var actual = sb.ToString();
            Assert.AreEqual(expected, actual);
        }

    }
}