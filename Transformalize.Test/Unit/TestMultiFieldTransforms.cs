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
    public class MulitFieldTestTransforms : EtlProcessHelper {


        [Test]
        public void TestJavascriptTransformStrings() {

            var process = new Process {
                Transforms = new ITransform[] {
                    new JavascriptTransform("FirstName + ' ' + LastName",
                        new Dictionary<string, Field> { {"FirstName", new Field(FieldType.Field)},{"LastName",new Field(FieldType.Field)}},
                        new Dictionary<string, Field> { {"FullName", new Field(FieldType.Field)}}
                    )
                }
            };

            var rows = TestOperation(
                GetTestData(new List<Row> {
                    new Row { {"FirstName", "Dale"}, {"LastName", "Newman"} },
                }),
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("Dale Newman", rows[0]["FullName"]);
        }

        [Test]
        public void TestJavascriptTransformNumbers() {

            var process = new Process {
                Transforms = new ITransform[] {
                    new JavascriptTransform("x * y",
                        new Dictionary<string, Field> { {"x", new Field("System.Int32",8,FieldType.Field,true,0)},{"y",new Field("System.Int32",8,FieldType.Field,true,0)}},
                        new Dictionary<string, Field> { {"z", new Field(FieldType.Field)}}
                    )
                }
            };

            var rows = TestOperation(
                GetTestData(new List<Row> {
                    new Row { {"x", 3}, {"y", 11} },
                }),
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual(33, rows[0]["z"]);
        }

        [Test]
        public void TestFormatTransform() {

            var process = new Process {
                Transforms = new ITransform[] {
                    new FormatTransform("{0} {1}",
                        new Dictionary<string, Field> { {"x", new Field(FieldType.Field)},{"y",new Field(FieldType.Field)}},
                        new Dictionary<string, Field> { {"z", new Field(FieldType.Field)}}
                    )
                }
            };

            var rows = TestOperation(
                GetTestData(new List<Row> {
                    new Row { {"x", "Dale"}, {"y", "Newman"} },
                }),
                new ProcessTransform(process),
                new LogOperation()
            );

            Assert.AreEqual("Dale Newman", rows[0]["z"]);
        }


        private static IOperation GetTestData(IEnumerable<Row> data) {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(data);
            return mock.Object;
        }
    }
}
