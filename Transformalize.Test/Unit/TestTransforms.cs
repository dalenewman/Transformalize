using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;
using System.Linq;
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
                new Row { {"Field1", null }}
            });
        }

        [Test]
        public void TestReplaceTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new ReplaceTransform("b", "B"), new ReplaceTransform("2", "Two") }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A B C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 Two 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestInsertTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new InsertTransform(1, ".") }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A. b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1. 2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestRemoveTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new RemoveTransform(2, 2) }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("1 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestTrimStartTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new TrimStartTransform("1 ") }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A b C d E f G", rows[0]["Field1"]);
            Assert.AreEqual("2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestTrimEndTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new TrimEndTransform("G ") }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A b C d E f", rows[0]["Field1"]);
            Assert.AreEqual("1 2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestTrimTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new TrimTransform("1G") }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A b C d E f ", rows[0]["Field1"]);
            Assert.AreEqual(" 2 3 4 5 6 7", rows[1]["Field1"]);

        }

        [Test]
        public void TestSubStringTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new SubstringTransform(4, 3) }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("C d", rows[0]["Field1"]);
            Assert.AreEqual("3 4", rows[1]["Field1"]);

        }

        [Test]
        public void TestLeftTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new LeftTransform(4) }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("A b ", rows[0]["Field1"]);
            Assert.AreEqual("1 2 ", rows[1]["Field1"]);

        }

        [Test]
        public void TestRightTransform() {

            var entity = new Entity();
            entity.All["Field1"] = new Field { Length = 20, Transforms = new[] { new RightTransform(3) }, Input = true };

            var rows = TestOperation(
                _testInput.Object,
                new TransformOperation(entity),
                new LogOperation()
            );

            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("f G", rows[0]["Field1"]);
            Assert.AreEqual("6 7", rows[1]["Field1"]);

        }



    }
}
