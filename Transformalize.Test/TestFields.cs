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

using System.Collections.Generic;
using NUnit.Framework;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Test {
    [TestFixture]
    public class TestFields {

        private readonly List<Field> _fields = new List<Field> {
            new Field("system.string", "10", FieldType.PrimaryKey, true, "") {
                    Alias = "Field1",
                    Name = "f1",
                    Default = "x",
                    Parent = "p1"
                },
            new Field("system.int32", "8", FieldType.NonKey, false, "0")
                {
                    Alias = "Field2",
                    Name = "f2",
                    Default = 0,
                    Parent = "p2"
                }
        };

        [Test]
        public void TestWriteAlias() {

            const string expected = "[Field1], [Field2]";
            var actual = new FieldSqlWriter(_fields).Alias("[","]").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteAliasIsNull() {
            const string expected = "ISNULL([Field1], 'x'), ISNULL([Field2], 0)";
            var actual = new FieldSqlWriter(_fields).Alias("[", "]").IsNull().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteName() {
            const string expected = "[f1], [f2]";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataType() {
            const string expected = "[f1] NVARCHAR(10), [f2] INT";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").DataType(new SqlServerDataTypeService()).Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataTypeNotNull() {
            const string expected = "[f1] NVARCHAR(10) NOT NULL, [f2] INT NOT NULL";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").DataType(new SqlServerDataTypeService()).Append(" NOT NULL").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataTypeNull() {
            const string expected = "[f1] NVARCHAR(10) NULL, [f2] INT NULL";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").DataType(new SqlServerDataTypeService()).Append(" NULL").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNull() {
            const string expected = "ISNULL([f1], 'x'), ISNULL([f2], 0)";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").IsNull().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNullAsAlias() {
            const string expected = "ISNULL([f1], 'x') AS [Field1], ISNULL([f2], 0) AS [Field2]";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").IsNull().AsAlias("[", "]").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNullToAlias() {
            const string expected = "ISNULL([f1], 'x') AS [Field1], ISNULL([f2], 0) AS [Field2]";
            var actual = new FieldSqlWriter(_fields).Name("[", "]").IsNull().ToAlias("[", "]").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameOutput() {
            const string expected = "[f1]";
            var actual = new FieldSqlWriter(_fields).Output().Name("[","]").Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNothing() {
            const string expected = ", ";
            var actual = new FieldSqlWriter(_fields).Write();
            Assert.AreEqual(expected, actual);
        }

    }
}