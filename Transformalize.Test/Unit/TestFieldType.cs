/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using NUnit.Framework;
using Transformalize.Core.Field_;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestFieldType {

        [Test]
        public void TestEquality() {
            var x = FieldType.PrimaryKey;
            var y = FieldType.PrimaryKey;
            Assert.AreEqual(x, y);
        }

        [Test]
        public void TestInequality() {
            var x = FieldType.ForeignKey;
            var y = FieldType.PrimaryKey;
            Assert.AreNotEqual(x, y);
        }

        [Test]
        public void TestCombination() {
            var x = FieldType.ForeignKey | FieldType.MasterKey;
            var y = FieldType.ForeignKey;
            Assert.AreNotEqual(x, y);
            Assert.IsTrue(x.HasFlag(FieldType.ForeignKey));
            Assert.IsTrue(x.HasFlag(FieldType.MasterKey));
            Assert.IsTrue(y.HasFlag(FieldType.ForeignKey));
            Assert.IsFalse(y.HasFlag(FieldType.MasterKey));
            Assert.AreNotEqual(x, FieldType.ForeignKey);

        }

        [Test]
        public void TestAdd() {
            var x = FieldType.ForeignKey;
            x |= FieldType.MasterKey;
            Assert.IsTrue(x.HasFlag(FieldType.ForeignKey));
            Assert.IsTrue(x.HasFlag(FieldType.MasterKey));

        }

        [Test]
        public void TestNone() {
            var x = FieldType.None;
            x |= FieldType.ForeignKey;
            Assert.AreEqual(x, FieldType.ForeignKey);
            Assert.AreNotEqual(x, FieldType.None);
        }




    }
}
