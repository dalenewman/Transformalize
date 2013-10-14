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
using NUnit.Framework;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Test.Unit
{
    [TestFixture]
    public class TestParameters
    {
        [Test]
        public void TestAdding1()
        {
            var actual = new Parameters
                             {
                                 {"Key", "Name", "Value", "string"}
                             };
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Name", actual.First().Value.Name);
        }

        [Test]
        public void TestAdding2()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name1", "Value", "string"},
                                 {"Key2", "Name2", "Value", "string"},
                             };
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Name1", actual.First().Value.Name);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TestAddingSameKey()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name", "Value", "string"},
                                 {"Key1", "Name", "Value", "string"},
                             };
        }

        [Test]
        public void TestConstruction()
        {
            var actual = new Parameters();
            Assert.IsNotNull(actual);
            Assert.False(actual.Any());
        }

        [Test]
        public void TestEnumerate()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name1", "Value1", "string"},
                                 {"Key2", "Name2", "Value2", "string"},
                             };
            var count = 0;
            foreach (var pair in actual)
            {
                if (count == 0)
                {
                    Assert.AreEqual("Key1", pair.Key);
                    Assert.AreEqual("Value1", pair.Value.Value);
                }
                else
                {
                    Assert.AreEqual("Key2", pair.Key);
                }
                count++;
            }
        }

        [Test]
        public void TestGet1()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name1", "Value2", "string"},
                                 {"Key2", "Name2", "Value2", "string"},
                             };
            Assert.AreEqual("Name2", actual["Key2"].Name);
        }

        [Test]
        [ExpectedException(typeof (KeyNotFoundException))]
        public void TestGetBad()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name1", "Value2", "string"},
                                 {"Key2", "Name2", "Value2", "string"},
                             };
            var test = actual["Key3"];
        }

        [Test]
        public void TestValueType()
        {
            var actual = new Parameters
                             {
                                 {"Key1", "Name1", "0", "System.Int32"},
                                 {"Key2", "Name2", "false", "System.Boolean"},
                             };
            Assert.IsInstanceOf<Int32>(actual["Key1"].Value);
            Assert.IsInstanceOf<Boolean>(actual["Key2"].Value);
        }
    }
}