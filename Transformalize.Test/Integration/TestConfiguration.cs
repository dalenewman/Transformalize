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

using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestConfiguration : EtlProcessHelper
    {

        private readonly Process _process = new ProcessReader("Test").Read();

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

            Assert.AreEqual("Test", Process.Name);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestInput;Integrated Security=True", Process.Connections["input"].ConnectionString);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestOutput;Integrated Security=True", Process.Connections["output"].ConnectionString);
            Assert.AreEqual("Data Source=localhost;Initial Catalog=TestInput;Integrated Security=True", Process.Entities.First().InputConnection.ConnectionString);
            Assert.AreEqual("OrderDetailKey", Process.Entities.First().PrimaryKey["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", Process.Entities.First().Fields["ProductKey"].Alias);
            Assert.AreEqual("RowVersion", Process.Entities.First().Version.Alias);
            Assert.AreEqual("/Properties/Color", Process.Entities.First().Fields["Properties"].InnerXml["Color"].XPath);
            Assert.AreEqual(1, Process.Entities.First().Fields["Properties"].InnerXml["Color"].Index);
            Assert.AreEqual("OrderDetail", _process.Relationships[0].LeftEntity.Alias);
            Assert.AreEqual("Order", _process.Relationships[0].RightEntity.Alias);
            Assert.AreEqual("OrderKey", _process.Relationships[0].Join[0].LeftField.Name);
            Assert.AreEqual("OrderKey", _process.Relationships[0].Join[0].RightField.Name);

            Assert.AreEqual(3, _process.RelatedKeys.Count());
            Assert.AreEqual(0, Process.Entities.First().RelationshipToMaster.Count());
            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Product")).RelationshipToMaster.Count());
            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Order")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Customer")).RelationshipToMaster.Count());

        }

        [Test]
        public void TestRelatedKeys() {

            var process = new ProcessReader("NorthWind").Read();

            Assert.AreEqual(7, process.RelatedKeys.Count());

            foreach (var relatedKey in process.RelatedKeys) {
                Console.WriteLine("{0} : {1}", relatedKey.Entity, relatedKey.Name);
            }

            Assert.AreEqual(0, Process.Entities.First().RelationshipToMaster.Count());

            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Products")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Customers")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Employees")).RelationshipToMaster.Count());

            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Orders")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Categories")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Suppliers")).RelationshipToMaster.Count());

        }

        [Test]
        public void TestGetAllFieldsNeededForMultiFieldTransformations() {
            var process = new ProcessReader("Test").Read();

            var expected = new Parameters() {
                {"LastName", "LastName", null, "System.Object"},
                {"ProductName", "ProductName", null, "System.Object"}
            };

            var actual = process.Parameters;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected["LastName"].Name, actual["LastName"].Name);
            Assert.AreEqual(expected["ProductName"].Value, actual["ProductName"].Value);
        }

    }
}
