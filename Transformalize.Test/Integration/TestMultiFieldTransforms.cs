using System;
using NUnit.Framework;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Readers;
using System.Linq;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class TestMultiFieldTransforms : EtlProcessHelper {

        private Process _process;

        [SetUp]
        public void SetUp() {
            _process = new ProcessReader("Test").GetProcess();
        }

        [Test]
        public void TestExtract() {
            var input = new ParametersExtract(_process);

            var rows = TestOperation(input);

            Assert.AreEqual(5000, rows.Count);
            Assert.AreEqual(3, rows[0].Columns.Count());
        }

        [Test]
        public void TestTransform() {
            var input = new ParametersExtract(_process);

            var rows = TestOperation(
                input,
                new ProcessTransform(_process)
            );

            Assert.AreEqual(5000, rows.Count);
            Assert.AreEqual(4, rows[0].Columns.Count());
        }

        [Test]
        public void TestLoad() {
            var input = new ParametersExtract(_process);

            var rows = TestOperation(
                input,
                new ProcessTransform(_process),
                new ResultsLoad(_process)
            );

            Assert.AreEqual(0, rows.Count);
        }

        [Test]
        public void TestExperimentalLoad() {
            var input = new ParametersExtract(_process);

            var rows = TestOperation(
                input,
                new ProcessTransform(_process),
                new ResultsExperimentalLoad(_process)
            );

            Assert.AreEqual(0, rows.Count);
        }

    }
}
