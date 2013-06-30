using System.Collections.Generic;
using System.Text;
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

    public class FormatTransform : ITransform {
        private readonly string _format;
        private readonly Dictionary<string, Field> _parameters;
        private readonly Dictionary<string, Field> _results;
        private readonly bool _hasParameters;
        private readonly bool _hasResults;
        private readonly object[] _parameterValues;
        private int _index;

        public bool HasParameters {
            get { return _hasParameters; }
        }
        public bool HasResults {
            get { return _hasResults; }
        }

        public FormatTransform(string format, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _format = format;
            _parameters = parameters;
            _results = results;
            _hasParameters = parameters != null && parameters.Count > 0;
            _hasResults = results != null && results.Count > 0;

            if (!_hasParameters) return;

            _parameterValues = new object[_parameters.Count];
        }

        public FormatTransform(string format) : this(format, null, null) { }

        public void Transform(ref StringBuilder sb) {
            // not efficient, could do AppendFormat() first if value passed in along with stringbuilder
            var value = sb.ToString();
            sb.Clear();
            sb.AppendFormat(_format, value);
        }

        public void Transform(ref object value) {
            value = string.Format(_format, value);
        }

        public void Transform(ref Row row) {
            _index = 0;
            foreach (var key in _parameters.Keys) {
                _parameterValues[_index] = row[key];
                _index++;
            }

            var result = string.Format(_format, _parameterValues);
            foreach (var key in _results.Keys) {
                row[key] = result;
            }
        }

        public void Dispose() {
        }

    }
}
