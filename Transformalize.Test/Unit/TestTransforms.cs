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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;
using Transformalize.Transforms;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestTransforms : EtlProcessHelper {

        private readonly Mock<IOperation> _testInput = new Mock<IOperation>();

        [SetUp]
        public void SetUp() {
            _testInput.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "A b C d E f G"} },
                new Row { {"Field1", "1 2 3 4 5 6 7"} },
                new Row { {"Field1", "    "}},
                new Row { {"Field1", null }}
            });
        }

        [Test]
        public void TestReplaceTransform() {
            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new ReplaceTransform("b", "B"), new ReplaceTransform("2", "Two") } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A B C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 Two 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestRegexReplaceTransform() {
            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new RegexReplaceTransform("[bd]", "X", 0), new RegexReplaceTransform(@"[\d]{1}$", "DIGIT", 0) } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A X C X E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 2 3 4 5 6 DIGIT", rows[1]["Field1"]);

        }


        [Test]
        public void TestInsertTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new InsertTransform(1, ".") } };

            var rows = TestOperation(
                _testInput.Object,
                new EntityDefaults(entity),
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual(4, rows.Count);
            Assert.AreEqual("A. b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1. 2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestRemoveTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new RemoveTransform(2, 2) } };

            var rows = TestOperation(
                _testInput.Object,
                new EntityDefaults(entity),
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestTrimStartTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new TrimStartTransform("1 ") } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestTrimEndTransform1() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new TrimEndTransform(" ") }, Input = true };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("", rows[2]["Field1"]);

        }


        [Test]
        public void TestTrimEndTransform2() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new TrimEndTransform("G ") }, Input = true };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A b C d E f", rows[0]["Field1"]);
            Assert.AreEqual("1 2 3 4 5 6 7", rows[1]["Field1"]);
            Assert.AreEqual("", rows[2]["Field1"]);

        }

        [Test]
        public void TestTrimTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new TrimTransform("1G") } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A b C d E f ", rows[0]["Field1"]);
            Assert.AreEqual(" 2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestSubStringTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new SubstringTransform(4, 3) }, Input = true };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("C d", rows[0]["Field1"]);
            Assert.AreEqual("3 4", rows[1]["Field1"]);

        }

        [Test]
        public void TestLeftTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new LeftTransform(4) }, Input = true };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("A b ", rows[0]["Field1"]);
            Assert.AreEqual("1 2 ", rows[1]["Field1"]);

        }

        [Test]
        public void TestRightTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Transforms = new[] { new RightTransform(3) }, Input = true };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("f G", rows[0]["Field1"]);
            Assert.AreEqual("6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestMapTransformStartsWith() {
            var mapEquals = new Dictionary<string, object>();
            var mapStartsWith = new Dictionary<string, object>();
            var mapEndsWith = new Dictionary<string, object>();

            mapEquals["A b C d E f G"] = "They're Just Letters!";
            mapStartsWith["1"] = "I used to start with 1.";

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new MapTransform(new[] { mapEquals, mapStartsWith, mapEndsWith }) } };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("They're Just Letters!", rows[0]["Field1"]);
            Assert.AreEqual("I used to start with 1.", rows[1]["Field1"]);

        }

        [Test]
        public void TestMapTransformEndsWith() {
            var mapEquals = new Dictionary<string, object>();
            var mapStartsWith = new Dictionary<string, object>();
            var mapEndsWith = new Dictionary<string, object>();

            mapEquals["A b C d E f G"] = "They're Just Letters!";
            mapEndsWith["7"] = "I used to end with 7.";

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new MapTransform(new[] { mapEquals, mapStartsWith, mapEndsWith }) } };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("They're Just Letters!", rows[0]["Field1"]);
            Assert.AreEqual("I used to end with 7.", rows[1]["Field1"]);

        }

        [Test]
        public void TestMapTransformMore() {
            var mapEquals = new Dictionary<string, object>();
            var mapStartsWith = new Dictionary<string, object>();
            var mapEndsWith = new Dictionary<string, object>();

            mapStartsWith["A b C"] = "abc";
            mapEndsWith["6 7"] = "67";

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new MapTransform(new[] { mapEquals, mapStartsWith, mapEndsWith }) } };

            var rows = TestOperation(
                _testInput.Object,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("abc", rows[0]["Field1"]);
            Assert.AreEqual("67", rows[1]["Field1"]);

        }

        [Test]
        public void TestJavscriptStringTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new JavascriptTransform("field.length;", null, null) } };

            var rows = TestOperation(
                _testInput.Object,
                new EntityDefaults(entity),
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("13", rows[0]["Field1"]);
            Assert.AreEqual("13", rows[1]["Field1"]);
            Assert.AreEqual("4", rows[2]["Field1"]);
            Assert.AreEqual("0", rows[3]["Field1"]);

        }

        [Test]
        public void TestJavscriptInt32Transform() {

            var numbersMock = new Mock<IOperation>();
            numbersMock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", 10} },
                new Row { {"Field1", 20} },
                new Row { {"Field1", 0}},
                new Row { {"Field1", null }}
            });
            var numbers = numbersMock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field("System.Int32", 8, FieldType.Field, true, 0) { Input = true, Transforms = new[] { new JavascriptTransform("field * 2;", null, null) }, Default = 0 };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                numbers,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual(20, rows[0]["Field1"]);
            Assert.AreEqual(40, rows[1]["Field1"]);
            Assert.AreEqual(0, rows[2]["Field1"]);
            Assert.AreEqual(0, rows[3]["Field1"]);
        }

        [Test]
        public void TestPadLeftTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "345"} },
                new Row { {"Field1", ""} },
                new Row { {"Field1", "x"}},
                new Row { {"Field1", null }}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new PadLeftTransform(5, '0') }, Default = "00000" };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                input,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("00345", rows[0]["Field1"]);
            Assert.AreEqual("00000", rows[1]["Field1"]);
            Assert.AreEqual("0000x", rows[2]["Field1"]);
            Assert.AreEqual("00000", rows[3]["Field1"]);
        }

        [Test]
        public void TestPadRightTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "345"} },
                new Row { {"Field1", ""} },
                new Row { {"Field1", "x"}},
                new Row { {"Field1", null }}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new PadRightTransform(5, '0') }, Default = "00000" };
            var fields = new Dictionary<string, Field> { { "", null } };

            var rows = TestOperation(
                input,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("34500", rows[0]["Field1"]);
            Assert.AreEqual("00000", rows[1]["Field1"]);
            Assert.AreEqual("x0000", rows[2]["Field1"]);
            Assert.AreEqual("00000", rows[3]["Field1"]);
        }

        [Test]
        public void TestToUpperTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "345!"} },
                new Row { {"Field1", ""} },
                new Row { {"Field1", "abcDe"}},
                new Row { {"Field1", null }}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new ToUpperTransform() }, Default = "" };

            var rows = TestOperation(
                input,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("345!", rows[0]["Field1"]);
            Assert.AreEqual("", rows[1]["Field1"]);
            Assert.AreEqual("ABCDE", rows[2]["Field1"]);
            Assert.AreEqual("", rows[3]["Field1"]);
        }

        [Test]
        public void TestToLowerTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "345!"} },
                new Row { {"Field1", ""} },
                new Row { {"Field1", "abcDe"}},
                new Row { {"Field1", null }}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true, Transforms = new[] { new ToLowerTransform() }, Default = "" };

            var rows = TestOperation(
                input,
                new FieldTransform(entity),
                new LogOperation()
            );

            Assert.AreEqual("345!", rows[0]["Field1"]);
            Assert.AreEqual("", rows[1]["Field1"]);
            Assert.AreEqual("abcde", rows[2]["Field1"]);
            Assert.AreEqual("", rows[3]["Field1"]);
        }

        [Test]
        public void TestFormatTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "F1"}, {"Field2", "F2"} },
                new Row { {"Field1", ""}, {"Field2", ""} },
                new Row { {"Field1", " f1 "}, {"Field2", " f2 "}},
                new Row { {"Field1", null }, {"Field2", null}}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true };
            entity.All["Field2"] = new Field(FieldType.Field) { Input = true };

            var process = new Process();
            process.Transforms = new AbstractTransform[] {
                new FormatTransform("{0}+{1}", new Parameters { {"Field1", new Parameter("Field1", null)}, {"Field2", new Parameter("Field2", null)}},new Dictionary<string, Field>{{"result", new Field(FieldType.Field)}} )
            };

            var rows = TestOperation(
                input,
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("F1+F2", rows[0]["result"]);
            Assert.AreEqual("+", rows[1]["result"]);
            Assert.AreEqual(" f1 + f2 ", rows[2]["result"]);
            Assert.AreEqual("+", rows[3]["result"]);
        }

        [Test]
        public void TestConcatTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "F1"}, {"Field2", "F2"} },
                new Row { {"Field1", ""}, {"Field2", ""} },
                new Row { {"Field1", " f1 "}, {"Field2", " f2 "}},
                new Row { {"Field1", null }, {"Field2", null}}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true };
            entity.All["Field2"] = new Field(FieldType.Field) { Input = true };

            var process = new Process();
            process.Transforms = new AbstractTransform[] { new ConcatTransform(new Parameters { { "Field1", new Parameter() }, { "Field2", new Parameter("Field2", null) } }, new Dictionary<string, Field> { { "result", new Field(FieldType.Field) } }) };

            var rows = TestOperation(
                input,
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("F1F2", rows[0]["result"]);
            Assert.AreEqual("", rows[1]["result"]);
            Assert.AreEqual(" f1  f2 ", rows[2]["result"]);
            Assert.AreEqual("", rows[3]["result"]);
        }

        [Test]
        public void TestJsonTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "Data1"}, {"Field2", 2} },
                new Row { {"Field1", ""}, {"Field2", ""} },
                new Row { {"Field1", " f1 "}, {"Field2", " f2 "}},
                new Row { {"Field1", null }, {"Field2", null}}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true };
            entity.All["Field2"] = new Field(FieldType.Field) { Input = true };

            var process = new Process();
            process.Transforms = new AbstractTransform[] {
                new JsonTransform(
                    new Parameters {
                        { "Field1", new Parameter("Field1", null) },
                        { "Field2", new Parameter("Field2",null) }
                    }, 
                    new Dictionary<string, Field> {
                        { "result", new Field(FieldType.Field) }
                    })
            };

            var rows = TestOperation(
                input,
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("{\"Field1\":\"Data1\",\"Field2\":2}", rows[0]["result"]);
            Assert.AreEqual("{\"Field1\":\"\",\"Field2\":\"\"}", rows[1]["result"]);
            Assert.AreEqual("{\"Field1\":\" f1 \",\"Field2\":\" f2 \"}", rows[2]["result"]);
            Assert.AreEqual("{\"Field1\":null,\"Field2\":null}", rows[3]["result"]);
        }

        [Test]
        public void TestJsonTransform2() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "d1"}, {"Field2", "d2"} },
                new Row { {"Field1", 1}, {"Field2", 2} },
                new Row { {"Field1", null }, {"Field2", null}}
            });
            var input = mock.Object;

            var entity = new Entity();
            entity.All["Field1"] = new Field(FieldType.Field) { Input = true };
            entity.All["Field2"] = new Field(FieldType.Field) { Input = true };

            var process = new Process();
            process.Transforms = new AbstractTransform[] {
                new JsonTransform(new Parameters {
                    { "Field1", new Parameter("Field1",null) }, { "Field2", new Parameter("Field2",null) }, { "Field3", new Parameter("Field3", 3)}
                }, new Dictionary<string, Field> { { "result", new Field(FieldType.Field) } })
            };

            var rows = TestOperation(
                input,
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("{\"Field1\":\"d1\",\"Field2\":\"d2\",\"Field3\":3}", rows[0]["result"]);
            Assert.AreEqual("{\"Field1\":1,\"Field2\":2,\"Field3\":3}", rows[1]["result"]);
            Assert.AreEqual("{\"Field1\":null,\"Field2\":null,\"Field3\":3}", rows[2]["result"]);
        }


    }
}
