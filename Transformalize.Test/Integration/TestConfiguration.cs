using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Readers;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestConfiguration : EtlProcessHelper {
        [Test]
        public void TestGetAttribute() {
            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var process = config.Processes[0];

            Assert.AreEqual("Test", process.Name);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("output", process.Connections[1].Name);
            Assert.AreEqual("OrderDetail", process.Entities[0].Name);
            Assert.AreEqual("input", process.Entities[0].Connection);
            Assert.AreEqual("OrderDetailKey", process.Entities[0].PrimaryKey[0].Name);
            Assert.AreEqual("OrderDetailKey", process.Entities[0].PrimaryKey[0].Alias);
            Assert.AreEqual("OrderKey", process.Entities[0].Fields[0].Name);
            Assert.AreEqual("OrderKey", process.Entities[0].Fields[0].Alias);
            Assert.AreEqual("RowVersion", process.Entities[0].Version);
            Assert.AreEqual("/Properties/", process.Entities[0].Fields[4].Xml.XPath);
            Assert.AreEqual("Color", process.Entities[0].Fields[4].Xml[0].Alias);
            Assert.AreEqual("Color", process.Entities[0].Fields[4].Xml[0].XPath);
            Assert.AreEqual(1, process.Entities[0].Fields[4].Xml[0].Index);
            Assert.AreEqual("OrderDetail", process.Relationships[0].LeftEntity);
            Assert.AreEqual("Order", process.Relationships[0].RightEntity);
            Assert.AreEqual("OrderKey", process.Relationships[0].LeftField);
            Assert.AreEqual("OrderKey", process.Relationships[0].RightField);

        }

        [Test]
        public void TestGetAttributeAgain() {

            var process = new ProcessReader("Test").GetProcess();

            Assert.AreEqual("Test", process.Name);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Connections["input"].ConnectionString);
            Assert.AreEqual("Server=localhost;Database=TestOutput;Trusted_Connection=True;", process.Connections["output"].ConnectionString);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Entities["OrderDetail"].InputConnection.ConnectionString);
            Assert.AreEqual("OrderDetailKey", process.Entities["OrderDetail"].PrimaryKey["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", process.Entities["OrderDetail"].Fields["ProductKey"].Alias);
            Assert.AreEqual("RowVersion", process.Entities["OrderDetail"].Version.Alias);
            Assert.AreEqual("/Properties/Color", process.Entities["OrderDetail"].Fields["Properties"].InnerXml["Color"].XPath);
            Assert.AreEqual(1, process.Entities["OrderDetail"].Fields["Properties"].InnerXml["Color"].Index);
            Assert.AreEqual("OrderDetail", process.Joins[0].LeftEntity.Name);
            Assert.AreEqual("Order", process.Joins[0].RightEntity.Name);
            Assert.AreEqual("OrderKey", process.Joins[0].LeftField.Name);
            Assert.AreEqual("OrderKey", process.Joins[0].RightField.Name);

        }

        [Test]
        public void TestGetAllFieldsNeededForMultiFieldTransformations()
        {
            var process = new ProcessReader("Test").GetProcess();

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
