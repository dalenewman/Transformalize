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
using Moq;
using NUnit.Framework;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestTransforms : EtlProcessHelper {
        [SetUp]
        public void SetUp() {
            _testInput.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> {
                new Row { {"Field1", "A b C d E f G"} },
                new Row { {"Field1", "1 2 3 4 5 6 7"} },
                new Row { {"Field1", "    "} },
                new Row { {"Field1", null} }
            });
        }

        private readonly Mock<IOperation> _testInput = new Mock<IOperation>();

        [Test]
        public void TestCalcIfTransform() {

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row> { 
                new Row { {"var1", 2},  {"var2", 3.5} },
                new Row { {"var1", 1}, {"var2", 5.3} },
                new Row { {"var1", 0}, {"var2", 1.0} }
            });
            var input = mock.Object;

            var parameters = new Parameters {
                {"var1", new Parameter("var1", null)},
                {"var2", new Parameter("var2", null)}
            };

            var result = new Field("boolean", "0", FieldType.Field, true, "false") { Alias = "result" };
            result.Transforms.Add(new ExpressionTransform("if([var1] * [var2] == 7, true, false)", parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
                new LogOperation()
            );

            Assert.AreEqual(true, rows[0]["result"]);
            Assert.AreEqual(false, rows[1]["result"]);
            Assert.AreEqual(false, rows[2]["result"]);
        }




        [Test]
        public void TestToStringTransform() {

            const string fieldName = "field";

            var inputOperation = new RowsBuilder()
                .WithRow().WithField(fieldName, 46.2387m)
                .ToOperation();

            var field1 = new Field("System.Decimal", "", FieldType.Field, true, "00.0") { Alias = fieldName };
            field1.Transforms.Add(new ToStringTransform("000.00", new Parameters()));

            var rows = TestOperation(
                inputOperation,
                new TransformFields(field1),
                new LogOperation()
            );

            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("046.24", rows[0][fieldName]);
        }

        private static IOperation GetInputOperation(IEnumerable<Row> input) {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(input);
            var data = mock.Object;
            return data;
        }

        [Test]
        public void TestCalcTransform() {
            var process = new Process("Test");

            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(
                new List<Row> { 
                    new Row { {"var1", 2}, {"var2", 3.5} }, 
                    new Row { {"var1", 1}, {"var2", 5.3} },
                    new Row { {"var1", 0}, {"var2", 1.0} }
                });
            var input = mock.Object;

            var parameters = new Parameters {
                {"var1", new Parameter("var1", null)},
                {"var2", new Parameter("var2", null)}
            };
            var result = new Field(FieldType.Field) {
                Alias = "result"
            };
            result.Transforms.Add(new ExpressionTransform("[var1] * [var2]", parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
                new LogOperation()
                );

            Assert.AreEqual(7.0, rows[0]["result"]);
            Assert.AreEqual(5.3, rows[1]["result"]);
            Assert.AreEqual(0.0, rows[2]["result"]);
        }

        [Test]
        public void TestConcatTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(
                new List<Row> {
                    new Row { {"Field1", "F1"}, {"Field2", "F2"} },
                    new Row { {"Field1", ""}, {"Field2", ""} },
                    new Row { {"Field1", " f1 "}, {"Field2", " f2 "} },
                    new Row { {"Field1", null}, {"Field2", null} }
                });
            var input = mock.Object;

            var parameters = new Parameters
                                 {
                                     {"Field1", new Parameter()},
                                     {"Field2", new Parameter("Field2", null)}
                                 };
            var result = new Field(FieldType.Field) {
                Alias = "result"
            };
            result.Transforms.Add(new ConcatTransform(parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
                new LogOperation()
                );

            Assert.AreEqual("F1F2", rows[0]["result"]);
            Assert.AreEqual("", rows[1]["result"]);
            Assert.AreEqual(" f1  f2 ", rows[2]["result"]);
            Assert.AreEqual("", rows[3]["result"]);
        }

        [Test]
        public void TestFormatTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "F1"},
                                                                                                 {"Field2", "F2"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""},
                                                                                                 {"Field2", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", " f1 "},
                                                                                                 {"Field2", " f2 "}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null},
                                                                                                 {"Field2", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var parameters = new Parameters
                                 {
                                     {"Field1", new Parameter("Field1", null)},
                                     {"Field2", new Parameter("Field2", null)}
                                 };
            var result = new Field(FieldType.Field) {
                Alias = "result"
            };
            result.Transforms.Add(new FormatTransform("{0}+{1}", parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
                new LogOperation()
                );

            Assert.AreEqual("F1+F2", rows[0]["result"]);
            Assert.AreEqual("+", rows[1]["result"]);
            Assert.AreEqual(" f1 + f2 ", rows[2]["result"]);
            Assert.AreEqual("+", rows[3]["result"]);
        }


        [Test]
        public void TestInsertTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms {
                    new InsertTransform(1, ".", new Parameters())
                }
            };

            var rows = TestOperation(
                _testInput.Object,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual(4, rows.Count);
            Assert.AreEqual("A. b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1. 2 3 4 5 6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestJavscriptFromScript() {
            const string scriptContent = "function Double(input) { if(!input || input == '') { return '';} else { return input + input;} }";
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "T"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "2"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var entity = new Entity(1);
            var scripts = new Dictionary<string, Script>
                              {
                                  {"test", new Script("test", scriptContent, "test.js")}
                              };
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new JavascriptTransform("Double(Field1);", "Field1", scripts)
                                                            }
            };

            var rows = TestOperation(
                input,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("TT", rows[0]["Field1"]);
            Assert.AreEqual("22", rows[1]["Field1"]);
            Assert.AreEqual("", rows[2]["Field1"]);
        }


        [Test]
        public void TestJavscriptInt32Transform() {
            var numbersMock = new Mock<IOperation>();
            numbersMock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                            {
                                                                                                new Row
                                                                                                    {
                                                                                                        {"Field1", 10}
                                                                                                    },
                                                                                                new Row
                                                                                                    {
                                                                                                        {"Field1", 20}
                                                                                                    },
                                                                                                new Row
                                                                                                    {
                                                                                                        {"Field1", 0}
                                                                                                    },
                                                                                                new Row
                                                                                                    {
                                                                                                        {"Field1", null}
                                                                                                    }
                                                                                            });
            var numbers = numbersMock.Object;

            var entity = new Entity(1);
            var scripts = new Dictionary<string, Script>();
            entity.Fields["Field1"] = new Field("System.Int32", "8", FieldType.Field, true, "0") {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new JavascriptTransform("Field1 * 2;", "Field1", scripts)
                                                            },
                Default = 0
            };

            var rows = TestOperation(
                numbers,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual(20, rows[0]["Field1"]);
            Assert.AreEqual(40, rows[1]["Field1"]);
            Assert.AreEqual(0, rows[2]["Field1"]);
            Assert.AreEqual(0, rows[3]["Field1"]);
        }

        [Test]
        public void TestJavscriptStringTransform() {
            var entity = new Entity(1);
            var scripts = new Dictionary<string, Script>();
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new JavascriptTransform("Field1.length;", "Field1", scripts)
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("13", rows[0]["Field1"]);
            Assert.AreEqual("13", rows[1]["Field1"]);
            Assert.AreEqual("4", rows[2]["Field1"]);
            Assert.AreEqual("0", rows[3]["Field1"]);
        }

        [Test]
        public void TestJsonTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "Data1"},
                                                                                                 {"Field2", 2}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""},
                                                                                                 {"Field2", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", " f1 "},
                                                                                                 {"Field2", " f2 "}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null},
                                                                                                 {"Field2", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var parameters = new Parameters
                                 {
                                     {"Field1", new Parameter("Field1", null)},
                                     {"Field2", new Parameter("Field2", null)}
                                 };
            var result = new Field(FieldType.Field) {
                Alias = "result"
            };
            result.Transforms.Add(new ToJsonTransform(parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
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
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "d1"},
                                                                                                 {"Field2", "d2"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", 1},
                                                                                                 {"Field2", 2}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null},
                                                                                                 {"Field2", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var parameters = new Parameters
                                 {
                                     {"Field1", new Parameter("Field1", null)},
                                     {"Field2", new Parameter("Field2", null)},
                                     {"Field3", new Parameter("Field3", 3)}
                                 };
            var result = new Field(FieldType.Field) {
                Alias = "result"
            };
            result.Transforms.Add(new ToJsonTransform(parameters));

            var rows = TestOperation(
                input,
                new TransformFields(result),
                new LogOperation()
                );

            Assert.AreEqual("{\"Field1\":\"d1\",\"Field2\":\"d2\",\"Field3\":3}", rows[0]["result"]);
            Assert.AreEqual("{\"Field1\":1,\"Field2\":2,\"Field3\":3}", rows[1]["result"]);
            Assert.AreEqual("{\"Field1\":null,\"Field2\":null,\"Field3\":3}", rows[2]["result"]);
        }

        [Test]
        public void TestLeftTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new LeftTransform(4, new Parameters())
                                                            },
                Input = true
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A b ", rows[0]["Field1"]);
            Assert.AreEqual("1 2 ", rows[1]["Field1"]);
        }

        [Test]
        public void TestMapTransformEndsWith() {
            var mapEquals = new Map();
            var mapStartsWith = new Map();
            var mapEndsWith = new Map();

            mapEquals["A b C d E f G"] = new Item("They're Just Letters!");
            mapEndsWith["7"] = new Item("I used to end with 7.");

            var entity = new Entity(1);
            var parameters = new Parameters();
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new MapTransform(new[] {mapEquals, mapStartsWith, mapEndsWith}, parameters)
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("They're Just Letters!", rows[0]["Field1"]);
            Assert.AreEqual("I used to end with 7.", rows[1]["Field1"]);
        }

        [Test]
        public void TestMapTransformMore() {
            var mapEquals = new Map();
            var mapStartsWith = new Map();
            var mapEndsWith = new Map();

            mapStartsWith["A b C"] = new Item("abc");
            mapEndsWith["6 7"] = new Item("67");

            var entity = new Entity(1);
            var parameters = new Parameters();
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new MapTransform(new[] {mapEquals, mapStartsWith, mapEndsWith}, parameters)
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("abc", rows[0]["Field1"]);
            Assert.AreEqual("67", rows[1]["Field1"]);
        }

        [Test]
        public void TestMapTransformStartsWith() {
            var mapEquals = new Map();
            var mapStartsWith = new Map();
            var mapEndsWith = new Map();

            mapEquals["A b C d E f G"] = new Item("They're Just Letters!");
            mapStartsWith["1"] = new Item("I used to start with 1.");

            var entity = new Entity(1);
            var parameters = new Parameters();
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new MapTransform(new[] {mapEquals, mapStartsWith, mapEndsWith}, parameters)
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("They're Just Letters!", rows[0]["Field1"]);
            Assert.AreEqual("I used to start with 1.", rows[1]["Field1"]);
        }

        [Test]
        public void TestPadLeftTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "345"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "x"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new PadLeftTransform(5, '0', new Parameters())
                                                            },
                Default = "00000"
            };

            var rows = TestOperation(
                input,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
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
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "345"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "x"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new PadRightTransform(5, '0', new Parameters())
                                                            },
                Default = "00000"
            };

            var rows = TestOperation(
                input,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("34500", rows[0]["Field1"]);
            Assert.AreEqual("00000", rows[1]["Field1"]);
            Assert.AreEqual("x0000", rows[2]["Field1"]);
            Assert.AreEqual("00000", rows[3]["Field1"]);
        }

        [Test]
        public void TestRegexReplaceTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new RegexReplaceTransform("[bd]", "X", 0, new Parameters()),
                                                                new RegexReplaceTransform(@"[\d]{1}$", "DIGIT", 0, new Parameters())
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A X C X E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 2 3 4 5 6 DIGIT", rows[1]["Field1"]);
        }

        [Test]
        public void TestRemoveTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new RemoveTransform(2, 2, new Parameters())
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 3 4 5 6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestReplaceTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new ReplaceTransform("b", "B", new Parameters()),
                                                                new ReplaceTransform("2", "Two", new Parameters())
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A B C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 Two 3 4 5 6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestRightTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new RightTransform(3, new Parameters())
                                                            },
                Input = true
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("f G", rows[0]["Field1"]);
            Assert.AreEqual("6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestSubStringTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new SubstringTransform(4, 3, new Parameters())
                                                            },
                Input = true
            };

            var rows = TestOperation(
                _testInput.Object,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("C d", rows[0]["Field1"]);
            Assert.AreEqual("3 4", rows[1]["Field1"]);
        }

        [Test]
        public void TestTemplateTransform() {
            var input = new Mock<IOperation>();

            input.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                      {
                                                                                          new Row
                                                                                              {
                                                                                                  {"Name", "Dale"}
                                                                                              }
                                                                                      });

            var entity = new Entity(1);
            var templates = new Dictionary<string, Template>();
            entity.Fields["Name"] = new Field(FieldType.Field) {
                Alias = "Name",
                Transforms = new Transforms
                                                          {
                                                              new TemplateTransform("Hello @Name", "Name", templates)
                                                          }
            };

            var rows = TestOperation(
                input.Object,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("Hello Dale", rows[0]["Name"]);
        }

        [Test]
        public void TestToLowerTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "345!"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "abcDe"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new ToLowerTransform(null)
                                                            },
                Default = ""
            };

            var rows = TestOperation(
                input,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("345!", rows[0]["Field1"]);
            Assert.AreEqual("", rows[1]["Field1"]);
            Assert.AreEqual("abcde", rows[2]["Field1"]);
            Assert.AreEqual("", rows[3]["Field1"]);
        }

        [Test]
        public void TestToUpperTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "345!"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", ""}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", "abcDe"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"Field1", null}
                                                                                             }
                                                                                     });
            var input = mock.Object;

            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Input = true,
                Transforms = new Transforms
                                                            {
                                                                new ToUpperTransform(null)
                                                            },
                Default = ""
            };

            var rows = TestOperation(
                input,
                new ApplyDefaults(entity.Fields),
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("345!", rows[0]["Field1"]);
            Assert.AreEqual("", rows[1]["Field1"]);
            Assert.AreEqual("ABCDE", rows[2]["Field1"]);
            Assert.AreEqual("", rows[3]["Field1"]);
        }

        [Test]
        public void TestTrimEndTransform1() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new TrimEndTransform(" ", new Parameters())
                                                            },
                Input = true
            };
            var fields = new Dictionary<string, Field>
                             {
                                 {"", null}
                             };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("", rows[2]["Field1"]);
        }


        [Test]
        public void TestTrimEndTransform2() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new TrimEndTransform("G ", new Parameters())
                                                            },
                Input = true
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A b C d E f", rows[0]["Field1"]);
            Assert.AreEqual("1 2 3 4 5 6 7", rows[1]["Field1"]);
            Assert.AreEqual("", rows[2]["Field1"]);
        }

        [Test]
        public void TestTrimStartTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new TrimStartTransform("1 ", new Parameters())
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("2 3 4 5 6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestTrimTransform() {
            var entity = new Entity(1);
            entity.Fields["Field1"] = new Field(FieldType.Field) {
                Alias = "Field1",
                Transforms = new Transforms
                                                            {
                                                                new TrimTransform("1G", new Parameters())
                                                            }
            };

            var rows = TestOperation(
                _testInput.Object,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("A b C d E f ", rows[0]["Field1"]);
            Assert.AreEqual(" 2 3 4 5 6 7", rows[1]["Field1"]);
        }

        [Test]
        public void TestXmlTransform() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(new List<Row>
                                                                                     {
                                                                                         new Row
                                                                                             {
                                                                                                 {"XmlField", "<xml><item1>something1</item1><item2>2</item2></xml>"}
                                                                                             },
                                                                                         new Row
                                                                                             {
                                                                                                 {"XmlField", "<xml><item1>something3</item1><item2>4</item2></xml>"}
                                                                                             }
                                                                                     });
            var xmlInput = mock.Object;

            var parameters = new Parameters
                                 {
                                     {"item1", "item1", null, "string"},
                                     {"XmlItem2", "item2", null, "int32"}
                                 };

            var entity = new Entity(1);
            entity.Fields["XmlField"] = new Field(FieldType.Field) {
                Alias = "XmlField",
                Input = true,
                Transforms = new Transforms
                                                              {
                                                                  new FromXmlTransform("XmlField", parameters)
                                                              },
                Default = ""
            };

            var rows = TestOperation(
                xmlInput,
                new TransformFields(entity.Fields),
                new LogOperation()
                );

            Assert.AreEqual("something1", rows[0]["item1"]);
            Assert.AreEqual(2, rows[0]["XmlItem2"]);
            Assert.AreEqual("something3", rows[1]["item1"]);
            Assert.AreEqual(4, rows[1]["XmlItem2"]);
        }
    }

    public class RowBuilder {
        private readonly RowsBuilder _rowsBuilder;
        private readonly Row _row;

        public RowBuilder(ref RowsBuilder rowsBuilder, ref Row row) {
            _rowsBuilder = rowsBuilder;
            _row = row;
        }

        public RowBuilder WithField(string field, object value) {
            _row[field] = value;
            return this;
        }

        public RowBuilder WithRow() {
            return _rowsBuilder.WithRow();
        }

        public IEnumerable<Row> ToRows() {
            return _rowsBuilder.ToRows();
        }

        public IOperation ToOperation() {
            return _rowsBuilder.ToOperation();
        }

    }

    public class RowsBuilder {
        private readonly IList<Row> _rows = new List<Row>();

        public RowBuilder WithRow() {
            var row = new Row();
            _rows.Add(row);
            var rowsBuilder = this;
            return new RowBuilder(ref rowsBuilder, ref row);
        }

        public IEnumerable<Row> ToRows() {
            return _rows;
        }

        public IOperation ToOperation() {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(ToRows());
            return mock.Object;
        }
    }
}