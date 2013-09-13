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

namespace Transformalize.Test.Unit
{
    [TestFixture]
    public class MultiFieldTestTransforms : EtlProcessHelper
    {
        private static IOperation GetTestData(IEnumerable<Row> data)
        {
            var mock = new Mock<IOperation>();
            mock.Setup(foo => foo.Execute(It.IsAny<IEnumerable<Row>>())).Returns(data);
            return mock.Object;
        }

        [Test]
        public void TestFormatTransform()
        {
            var parameters = new Parameters
                                 {
                                     {"x", new Parameter("x", null)},
                                     {"y", new Parameter("y", null)}
                                 };

            var z = new Field(FieldType.Field)
                        {
                            Alias = "z"
                        };
            z.Transforms.Add(new FormatTransform("{0} {1}", parameters));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"x", "Dale"},
                                            {"y", "Newman"}
                                        },
                                }),
                new TransformFields(z),
                new LogOperation()
                );

            Assert.AreEqual("Dale Newman", rows[0]["z"]);
        }

        [Test]
        public void TestJavascriptTransformNumbers()
        {
            var parameters = new Parameters
                                 {
                                     {"x", new Parameter("x", null)},
                                     {"y", new Parameter("y", null)}
                                 };

            var z = new Field(FieldType.Field)
                        {
                            Alias = "z"
                        };
            z.Transforms.Add(new JavascriptTransform("x * y", parameters, new Dictionary<string, Script>()));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"x", 3},
                                            {"y", 11}
                                        },
                                }),
                new TransformFields(z),
                new LogOperation()
                );

            Assert.AreEqual(33, rows[0]["z"]);
        }

        [Test]
        public void TestJavascriptTransformStrings()
        {
            var parameters = new Parameters
                                 {
                                     {"FirstName", new Parameter("FirstName", null)},
                                     {"LastName", new Parameter("LastName", null)}
                                 };

            var fullName = new Field(FieldType.Field)
                               {
                                   Alias = "FullName"
                               };
            fullName.Transforms.Add(new JavascriptTransform("FirstName + ' ' + LastName", parameters, new Dictionary<string, Script>()));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"FirstName", "Dale"},
                                            {"LastName", "Newman"}
                                        },
                                }),
                new TransformFields(fullName),
                new LogOperation()
                );

            Assert.AreEqual("Dale Newman", rows[0]["FullName"]);
        }

        [Test]
        public void TestJoinTransformStrings()
        {
            var parameters = new Parameters
                                 {
                                     {"FirstName", new Parameter("FirstName", null)},
                                     {"LastName", new Parameter("LastName", null)}
                                 };

            var fullName = new Field(FieldType.Field)
                               {
                                   Alias = "FullName"
                               };
            fullName.Transforms.Add(new JoinTransform(" ", parameters));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"FirstName", "Dale"},
                                            {"LastName", "Newman"}
                                        },
                                }),
                new TransformFields(fullName),
                new LogOperation()
                );

            Assert.AreEqual("Dale Newman", rows[0]["FullName"]);
        }

        [Test]
        public void TestTemplateTransformStringsWithDictionary()
        {
            var parameters = new Parameters
                                 {
                                     {"FirstName", new Parameter("FirstName", null)},
                                     {"LastName", new Parameter("LastName", null)}
                                 };
            var templates = new Dictionary<string, Template>();

            var fullName = new Field(FieldType.Field)
                               {
                                   Alias = "FullName"
                               };
            fullName.Transforms.Add(
                new TemplateTransform("@{ var fullName = Model[\"FirstName\"] + \" \" + Model[\"LastName\"];}@fullName", "dictionary",
                                      "FullName",
                                      parameters,
                                      templates
                    ));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"FirstName", "Dale"},
                                            {"LastName", "Newman"}
                                        },
                                }),
                new TransformFields(fullName),
                new LogOperation()
                );

            Assert.AreEqual("Dale Newman", rows[0]["FullName"]);
        }

        [Test]
        public void TestTemplateTransformStringsWithDynamic()
        {
            var parameters = new Parameters
                                 {
                                     {"FirstName", new Parameter("FirstName", null)},
                                     {"LastName", new Parameter("LastName", null)}
                                 };
            var templates = new Dictionary<string, Template>();

            var fullName = new Field(FieldType.Field)
                               {
                                   Alias = "FullName"
                               };
            fullName.Transforms.Add(
                new TemplateTransform("@{ var fullName = Model.FirstName + \" \" + Model.LastName;}@fullName",
                                      "FullName",
                                      "dynamic",
                                      parameters,
                                      templates
                    ));

            var rows = TestOperation(
                GetTestData(new List<Row>
                                {
                                    new Row
                                        {
                                            {"FirstName", "Dale"},
                                            {"LastName", "Newman"}
                                        },
                                }),
                new TransformFields(fullName),
                new LogOperation()
                );

            Assert.AreEqual("Dale Newman", rows[0]["FullName"]);
        }
    }
}