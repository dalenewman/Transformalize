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
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Data;
using Transformalize.Model;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestConfiguration : EtlProcessHelper {
        [Test]
        public void TestReadConfiguration() {
            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var test = config.Processes.Get("Test");

            Assert.AreEqual("Test", test.Name);
            Assert.AreEqual("input", test.Connections[0].Name);
            Assert.AreEqual("output", test.Connections[1].Name);
            Assert.AreEqual("OrderDetail", test.Entities[0].Name);
            Assert.AreEqual("input", test.Entities[0].Connection);
            Assert.AreEqual("OrderDetailKey", test.Entities[0].PrimaryKey[0].Name);
            Assert.AreEqual("OrderDetailKey", test.Entities[0].PrimaryKey[0].Alias);
            Assert.AreEqual("OrderKey", test.Entities[0].Fields[0].Name);
            Assert.AreEqual("OrderKey", test.Entities[0].Fields[0].Alias);
            Assert.AreEqual("RowVersion", test.Entities[0].Version);
            Assert.AreEqual("/Properties/", test.Entities[0].Fields[4].Xml.XPath);
            Assert.AreEqual("Color", test.Entities[0].Fields[4].Xml[0].Alias);
            Assert.AreEqual("Color", test.Entities[0].Fields[4].Xml[0].XPath);
            Assert.AreEqual(1, test.Entities[0].Fields[4].Xml[0].Index);
            Assert.AreEqual("OrderDetail", test.Relationships[0].LeftEntity);
            Assert.AreEqual("Order", test.Relationships[0].RightEntity);
            Assert.AreEqual("OrderKey", test.Relationships[0].Join[0].LeftField);
            Assert.AreEqual("OrderKey", test.Relationships[0].Join[0].RightField);

        }

        [Test]
        public void TestProcessReader() {

            var process = new ProcessReader("Test").Read();

            Assert.AreEqual("Test", process.Name);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestInput;Integrated Security=True", process.Connections["input"].ConnectionString);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestOutput;Integrated Security=True", process.Connections["output"].ConnectionString);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestInput;Integrated Security=True", process.Entities.First().InputConnection.ConnectionString);
            Assert.AreEqual("OrderDetailKey", process.Entities.First().PrimaryKey["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", process.Entities.First().Fields["ProductKey"].Alias);
            Assert.AreEqual("RowVersion", process.Entities.First().Version.Alias);
            Assert.AreEqual("/Properties/Color", process.Entities.First().Fields["Properties"].InnerXml["Color"].XPath);
            Assert.AreEqual(1, process.Entities.First().Fields["Properties"].InnerXml["Color"].Index);
            Assert.AreEqual("OrderDetail", process.Relationships[0].LeftEntity.Name);
            Assert.AreEqual("Order", process.Relationships[0].RightEntity.Name);
            Assert.AreEqual("OrderKey", process.Relationships[0].Join[0].LeftField.Name);
            Assert.AreEqual("OrderKey", process.Relationships[0].Join[0].RightField.Name);

            Assert.AreEqual(3, process.RelatedKeys.Count());
            Assert.AreEqual(0, process.Entities.First().RelationshipToMaster.Count());
            Assert.AreEqual(1, process.Entities.First(e => e.Name.Equals("Product")).RelationshipToMaster.Count());
            Assert.AreEqual(1, process.Entities.First(e => e.Name.Equals("Order")).RelationshipToMaster.Count());
            Assert.AreEqual(2, process.Entities.First(e => e.Name.Equals("Customer")).RelationshipToMaster.Count());

        }

        [Test]
        public void TestGetAllFieldsNeededForMultiFieldTransformations()
        {
            var process = new ProcessReader("Test").Read();

            var expected = new Dictionary<string, Field> {
                {"LastName", new Field(FieldType.Field) { Entity = "Customer", Alias = "LastName"}},
                {"ProductName", new Field(FieldType.Field) { Entity = "Product", Alias="ProductName"}}
            };

            var actual = process.Parameters;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected["LastName"].Alias, actual["LastName"].Alias);
            Assert.AreEqual("Customer", actual["LastName"].Entity);
            Assert.AreEqual(expected["ProductName"].Alias, actual["ProductName"].Alias);
            Assert.AreEqual("Product", actual["ProductName"].Entity);
        }
    }
}
