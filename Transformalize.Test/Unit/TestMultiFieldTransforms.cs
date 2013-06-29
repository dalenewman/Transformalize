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
    public class MulitFieldTestTransforms : EtlProcessHelper {


        [Test]
        public void TestJavascriptTransform() {
            
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

        private static IOperation GetTestData(IEnumerable<Row> data) {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(data);
            return mock.Object;
        }
    }
}
