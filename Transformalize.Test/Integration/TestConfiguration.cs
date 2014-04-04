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

using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Main.Parameters;
using Transformalize.Runner;

namespace Transformalize.Test.Integration
{
    [TestFixture]
    public class TestConfiguration : EtlProcessHelper
    {
        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Debug);
            LogManager.ReconfigExistingLoggers();
        }

        private static readonly ProcessConfigurationElement Element = new ProcessConfigurationReader("Test").Read()[0];
        private readonly Process _process = new ProcessReader(Element, new Options()).Read();

        [Test]
        public void TestFromXmlTransformFieldsToParametersAdapter()
        {
            //Assert.AreEqual(3, Element.Entities[0].Fields[4].Transforms[0].Fields.Cast<FieldConfigurationElement>().Count());
            //Assert.AreEqual(0, Element.Entities[0].Fields[4].Transforms[0].Parameters.Count);

            //new FromXmlTransformFieldsToParametersAdapter(Element).Adapt();

            //Assert.AreEqual(3, Element.Entities[0].Fields[4].Transforms[0].Parameters.Count);

            //// tests combined because they affect one another

            //var initialCount = Element.Entities[0].Fields.Cast<FieldConfigurationElement>().Count();
            //Assert.AreEqual(3, Element.Entities[0].Fields[4].Transforms[0].Fields.Cast<FieldConfigurationElement>().Count());

            //new FromXmlTransformFieldsMoveAdapter(Element).Adapt();

            Assert.AreEqual(0, Element.Entities[0].Fields[5].Transforms[0].Fields.Cast<FieldConfigurationElement>().Count());
            Assert.AreEqual(10, Element.Entities[0].Fields.Cast<FieldConfigurationElement>().Count());
        }

        [Test]
        public void TestGetAllFieldsNeededForMultiFieldTransformations()
        {
            var expected = new Parameters {
                {"LastName", "LastName", null, "System.Object"},
                {"ProductName", "ProductName", null, "System.Object"}
            };

            var actual = _process.Parameters;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected["LastName"].Name, actual["LastName"].Name);
            Assert.AreEqual(expected["ProductName"].Value, actual["ProductName"].Value);
        }

        [Test]
        public void TestProcessReader()
        {
            Assert.AreEqual("Test", _process.Name);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True", _process.Connections["input"].GetConnectionString());
            Assert.AreEqual("Server=localhost;Database=TestOutput;Trusted_Connection=True", _process.Connections["output"].GetConnectionString());
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True", _process.Entities.First().Input.First().Connection.GetConnectionString());
            Assert.AreEqual("OrderDetailKey", _process.Entities.First().PrimaryKey["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", _process.Entities.First().Fields["ProductKey"].Alias);
            Assert.AreEqual("OrderDetailRowVersion", _process.Entities.First().Version.Alias);

            Assert.AreEqual("OrderDetail", _process.Relationships[0].LeftEntity.Alias);
            Assert.AreEqual("Order", _process.Relationships[0].RightEntity.Alias);
            Assert.AreEqual("OrderKey", _process.Relationships[0].Join[0].LeftField.Name);
            Assert.AreEqual("OrderKey", _process.Relationships[0].Join[0].RightField.Name);

            Assert.AreEqual(0, _process.Entities.First().RelationshipToMaster.Count());
            Assert.AreEqual(1, _process.Entities.First(e => e.Alias.Equals("Product")).RelationshipToMaster.Count());
            Assert.AreEqual(1, _process.Entities.First(e => e.Alias.Equals("Order")).RelationshipToMaster.Count());
            Assert.AreEqual(2, _process.Entities.First(e => e.Alias.Equals("Customer")).RelationshipToMaster.Count());
        }

        [Test]
        public void TestReadConfiguration()
        {
            var test = Element;

            Assert.AreEqual("Test", test.Name);
            Assert.AreEqual("input", test.Connections[0].Name);
            Assert.AreEqual("output", test.Connections[1].Name);
            Assert.AreEqual("OrderDetail", test.Entities[0].Name);
            Assert.AreEqual("input", test.Entities[0].Connection);
            Assert.AreEqual("OrderDetailKey", test.Entities[0].Fields[0].Name);
            Assert.AreEqual("OrderDetailKey", test.Entities[0].Fields[0].Alias);
            Assert.AreEqual("OrderKey", test.Entities[0].Fields[1].Name);
            Assert.AreEqual("OrderKey", test.Entities[0].Fields[1].Alias);
            Assert.AreEqual("RowVersion", test.Entities[0].Version);
            Assert.AreEqual("OrderDetail", test.Relationships[0].LeftEntity);
            Assert.AreEqual("Order", test.Relationships[0].RightEntity);
            Assert.AreEqual("OrderKey", test.Relationships[0].Join[0].LeftField);
            Assert.AreEqual("OrderKey", test.Relationships[0].Join[0].RightField);
        }
    }
}