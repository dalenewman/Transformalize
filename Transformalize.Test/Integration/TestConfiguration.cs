using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Readers;

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
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Connections["input"].ConnectionString);
            Assert.AreEqual("Server=localhost;Database=TestOutput;Trusted_Connection=True;", process.Connections["output"].ConnectionString);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Entities["OrderDetail"].InputConnection.ConnectionString);
            Assert.AreEqual("OrderDetailKey", process.Entities["OrderDetail"].PrimaryKey["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", process.Entities["OrderDetail"].Fields["ProductKey"].Alias);
            Assert.AreEqual("RowVersion", process.Entities["OrderDetail"].Version.Alias);
            Assert.AreEqual("/Properties/Color", process.Entities["OrderDetail"].Fields["Properties"].InnerXml["Color"].XPath);
            Assert.AreEqual(1, process.Entities["OrderDetail"].Fields["Properties"].InnerXml["Color"].Index);
            Assert.AreEqual("OrderDetail", process.Relationships[0].LeftEntity.Name);
            Assert.AreEqual("Order", process.Relationships[0].RightEntity.Name);
            Assert.AreEqual("OrderKey", process.Relationships[0].Join[0].LeftField.Name);
            Assert.AreEqual("OrderKey", process.Relationships[0].Join[0].RightField.Name);

            Assert.AreEqual(3, process.RelatedKeys.Count());
            Assert.AreEqual(0, process.Entities["OrderDetail"].RelationshipToMaster.Count());
            Assert.AreEqual(1, process.Entities["Product"].RelationshipToMaster.Count());
            Assert.AreEqual(1, process.Entities["Order"].RelationshipToMaster.Count());
            Assert.AreEqual(2, process.Entities["Customer"].RelationshipToMaster.Count());

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
