using System.Configuration;
using NUnit.Framework;
using Transformalize.Configuration;

namespace Transformalize.Test {
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
            Assert.AreEqual("OrderDetailKey", process.Entities[0].Keys[0].Name);
            Assert.AreEqual("OrderDetailKey", process.Entities[0].Keys[0].Alias);
            Assert.AreEqual("OrderKey", process.Entities[0].Fields[0].Name);
            Assert.AreEqual("OrderKey", process.Entities[0].Fields[0].Alias);
            Assert.AreEqual("RowVersion", process.Entities[0].Version);
            Assert.AreEqual("/Properties/", process.Entities[0].Fields[4].Xml.XPath);
            Assert.AreEqual("Color", process.Entities[0].Fields[4].Xml[0].Alias);
            Assert.AreEqual("Color", process.Entities[0].Fields[4].Xml[0].XPath);
            Assert.AreEqual(1, process.Entities[0].Fields[4].Xml[0].Index);
            Assert.AreEqual("OrderDetail", process.Joins[0].LeftEntity);
            Assert.AreEqual("Order", process.Joins[0].RightEntity);
            Assert.AreEqual("OrderKey", process.Joins[0].LeftField);
            Assert.AreEqual("OrderKey", process.Joins[0].RightField);

        }

        [Test]
        public void TestGetAttributeAgain() {

            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var process = new ProcessConfiguration(config.Processes[0]);

            Assert.AreEqual("Test", process.Name);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Connections["input"].Value);
            Assert.AreEqual("Server=localhost;Database=TestOutput;Trusted_Connection=True;", process.Connections["output"].Value);
            Assert.AreEqual("Server=localhost;Database=TestInput;Trusted_Connection=True;", process.Entities["OrderDetail"].Connection.Value);
            Assert.AreEqual("OrderDetailKey", process.Entities["OrderDetail"].Keys["OrderDetailKey"].Alias);
            Assert.AreEqual("ProductKey", process.Entities["OrderDetail"].Fields["ProductKey"].Alias);
            Assert.AreEqual("RowVersion", process.Entities["OrderDetail"].Version.Alias);
            Assert.AreEqual("/Properties/Color", process.Entities["OrderDetail"].Fields["Properties"].Xml["Color"].XPath);
            Assert.AreEqual(1, process.Entities["OrderDetail"].Fields["Properties"].Xml["Color"].Index);
            Assert.AreEqual("OrderDetail", process.Joins[0].LeftEntity.Name);
            Assert.AreEqual("Order", process.Joins[0].RightEntity.Name);
            Assert.AreEqual("OrderKey", process.Joins[0].LeftField.Name);
            Assert.AreEqual("OrderKey", process.Joins[0].RightField.Name);

        }
    }
}
