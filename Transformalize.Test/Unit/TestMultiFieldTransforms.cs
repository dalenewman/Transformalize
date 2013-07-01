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
