using System.Collections.Generic;
using NUnit.Framework;
using Transformalize.Model;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestFields {

        private readonly List<IField> _fields = new List<IField> {
            new Field("system.string", FieldType.PrimaryKey, true) {Alias = "Field1", Name="f1", Default="x", Length = 10, Parent="p1"},
            new Field("system.int32", FieldType.Field, false) {Alias = "Field2", Name="f2", Default=0, Length = 0, Parent="p2"}
        };

        private readonly List<IField> _xmlFields = new List<IField> {
            new Xml("system.string", true) {Alias = "Field1", Name="f1", Default="x", Length = 10, XPath = "/Properties/f1", Index=1, Parent="p1"},
            new Xml("system.int32", false) {Alias = "Field2", Name="f2", Default=0, Length = 0, XPath = "/Properties/f2", Index=1, Parent="p2"}
        };

        [Test]
        public void TestWriteNothing() {
            const string expected = ", ";
            var actual = new FieldSqlWriter(_fields).Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteName() {
            const string expected = "[f1], [f2]";
            var actual = new FieldSqlWriter(_fields).Name().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteAlias() {
            const string expected = "[Field1], [Field2]";
            var actual = new FieldSqlWriter(_fields).Alias().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNull() {
            const string expected = "ISNULL([f1], 'x'), ISNULL([f2], 0)";
            var actual = new FieldSqlWriter(_fields).Name().IsNull().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteAliasIsNull() {
            const string expected = "ISNULL([Field1], 'x'), ISNULL([Field2], 0)";
            var actual = new FieldSqlWriter(_fields).Alias().IsNull().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNullToAlias() {
            const string expected = "[Field1] = ISNULL([f1], 'x'), [Field2] = ISNULL([f2], 0)";
            var actual = new FieldSqlWriter(_fields).Name().IsNull().ToAlias().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameIsNullAsAlias() {
            const string expected = "ISNULL([f1], 'x') AS [Field1], ISNULL([f2], 0) AS [Field2]";
            var actual = new FieldSqlWriter(_fields).Name().IsNull().AsAlias().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataType() {
            const string expected = "[f1] NVARCHAR(10), [f2] INT";
            var actual = new FieldSqlWriter(_fields).Name().DataType().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataTypeNull() {
            const string expected = "[f1] NVARCHAR(10) NULL, [f2] INT NULL";
            var actual = new FieldSqlWriter(_fields).Name().DataType().Null().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameDataTypeNotNull() {
            const string expected = "[f1] NVARCHAR(10) NOT NULL, [f2] INT NOT NULL";
            var actual = new FieldSqlWriter(_fields).Name().DataType().NotNull().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteXmlValueToAlias() {
            const string expected = "[Field1] = t.[p1].value('(/Properties/f1)[1]', 'NVARCHAR(10)'), [Field2] = t.[p2].value('(/Properties/f2)[1]', 'INT')";
            var actual = new FieldSqlWriter(_xmlFields).XmlValue().Prepend("t.").ToAlias().Write();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteNameOutput() {
            const string expected = "[f1]";
            var actual = new FieldSqlWriter(_fields).Output().Name().Write();
            Assert.AreEqual(expected, actual);
        }

    }
}
