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

using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Test.Unit.Builders;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestProcess {


        [Test]
        public void NorthWindProcessBuiltFromScratch() {

            var process = new ProcessBuilder("Test")
                .Connection("input").Database("NorthWind")
                .Connection("output").Database("NorthWindStar")
                .Connection("sass").Database("NorthWind").Provider("AnalysisServices")
                .Template("solr-data-handler").File("solr-data-handler.cshtml").Cache(true)
                    .Action("copy").File(@"C:\Solr\NorthWind\conf\data-config.xml")
                .Template("solr-schema").File("solr-schema.cshtml").Cache(true)
                    .Action("copy").File(@"C:\Solr\NorthWind\conf\schema.xml")
                    .Action("web").Mode("init").Url("http://localhost:8983/solr/admin/cores?action=RELOAD&core=NorthWind")
                    .Action("web").Mode("first").Url("http://localhost:8983/solr/NorthWind/dataimport?command=full-import&clean=true&commit=true&optimize=true")
                    .Action("web").Mode("default").Url("http://localhost:8983/solr/NorthWind/dataimport?command=delta-import&clean=false&commit=true&optimize=true")
                .Map("Managers").Connection("input").Sql("select EmployeeID, FirstName + ' ' + LastName FROM Employees;")
                .SearchType("facet").Type("lowercase").Store(true).Index(true)
                .SearchType("standard").Type("standard_lowercase").Store(false).Index(true)
                .Entity("Order Details").Version("RowVersion").Prefix("OrderDetails")
                    .Field("Discount").Single()
                    .Field("OrderID").Int32().PrimaryKey()
                    .Field("ProductID").Int32().PrimaryKey()
                    .Field("Quantity").Int16()
                    .Field("UnitPrice").Decimal().Precision(19).Scale(4)
                    .Field("RowVersion").RowVersion()
                    .CalculatedField("OrderDetailsExtendedPrice").Decimal().Precision(19).Scale(4)
                        .Transform("javascript").Script("OrderDetailsQuantity * (OrderDetailsUnitPrice * (1-OrderDetailsDiscount))")
                            .Parameter("*")
                .Entity("Orders").Version("RowVersion").Prefix("Orders")
                    .Field("CustomerID").Char().Length(5)
                    .Field("EmployeeID").Int32()
                    .Field("Freight").Decimal(19, 4)
                    .Field("OrderDate").DateTime()
                    .Field("OrderID").Int32().PrimaryKey()
                    .Field("RequiredDate").DateTime()
                    .Field("RowVersion").RowVersion()
                    .Field("ShipAddress")
                    .Field("ShipCity").Length(15)
                    .Field("ShipCountry").Length(15)
                    .Field("ShipName").Length(40)
                    .Field("ShippedDate").DateTime()
                    .Field("ShipPostalCode").Length(10)
                    .Field("ShipRegion").Length(15)
                    .Field("ShipVia").Int32()
                    .CalculatedField("TimeOrderMonth").Length(6).Default("12-DEC")
                        .Transform("toString").Format("MM-MMM")
                            .Parameter("OrderDate")
                        .Transform("toUpper")
                    .CalculatedField("TimeOrderDate").Length(10).Default("9999-12-31")
                        .Transform().ToString("yyyy-MM-dd")
                            .Parameter("OrderDate")
                    .CalculatedField("TimeOrderYear").Length(4).Default("9999")
                        .Transform().ToString("yyyy")
                            .Parameter("OrderDate")
                .Entity("Customers").Version("RowVersion").Prefix("Customers")
                    .Field("Address")
                    .Field("City").Length(15)
                    .Field("CompanyName").Length(40)
                    .Field("ContactName").Length(30)
                    .Field("ContactTitle").Length(30)
                    .Field("Country").Length(15)
                    .Field("CustomerID").Length(5).PrimaryKey()
                    .Field("Fax").Length(24)
                    .Field("Phone").Length(24)
                    .Field("PostalCode").Length(10)
                    .Field("Region").Length(15)
                    .Field("RowVersion").RowVersion()
                .Entity("Employees").Version("RowVersion").Prefix("Employees")
                    .Field("Address").Length(60)
                    .Field("BirthDate").DateTime()
                    .Field("City").Length(15)
                    .Field("Country").Length(15)
                    .Field("EmployeeID").Int32().PrimaryKey()
                    .Field("Extension").Length(4)
                    .Field("FirstName").Length(10)
                    .Field("HireDate").DateTime()
                    .Field("HomePhone").Length(24)
                    .Field("LastName").Length(20)
                    .Field("Notes").Length("MAX")
                    .Field("Photo").Alias("EmployeesPhoto").Input(false).Output(false)
                    .Field("PhotoPath").Alias("EmployeesPhotoPath").Input(false).Output(false)
                    .Field("PostalCode").Length(10)
                    .Field("Region").Length(15)
                    .Field("RowVersion").Type("System.Byte[]").Length(8)
                    .Field("Title").Length(30)
                    .Field("TitleOfCourtesy").Length(25)
                    .Field("ReportsTo").Alias("EmployeesManager")
                        .Transform("map").Map("Managers")
                    .CalculatedField("Employee")
                        .Transform("join").Separator(" ")
                            .Parameter("FirstName")
                            .Parameter("LastName")
                .Entity("Products").Version("RowVersion").Prefix("Products")
                    .Field("CategoryID").Int32()
                    .Field("Discontinued").Bool()
                    .Field("ProductID").Int32().PrimaryKey()
                    .Field("ProductName").Length(40)
                    .Field("QuantityPerUnit").Length(20)
                    .Field("ReorderLevel").Int16()
                    .Field("RowVersion").RowVersion()
                    .Field("SupplierID").Int32()
                    .Field("UnitPrice").Decimal(19, 4)
                    .Field("UnitsInStock").Int16()
                    .Field("UnitsOnOrder").Int16()
                .Entity("Suppliers").Version("RowVersion").Prefix("Suppliers")
                    .Field("Address").Length(60)
                    .Field("City").Length(15)
                    .Field("CompanyName").Length(40)
                    .Field("ContactName").Length(30)
                    .Field("ContactTitle").Length(30)
                    .Field("Country").Length(15)
                    .Field("Fax").Length(24)
                    .Field("HomePage").Length("MAX")
                    .Field("Phone").Length(24)
                    .Field("PostalCode").Length(10)
                    .Field("Region").Length(15)
                    .Field("RowVersion").RowVersion()
                    .Field("SupplierID").Int32().PrimaryKey()
                .Entity("Categories").Version("RowVersion").Prefix("Categories")
                    .Field("CategoryID").Int32().PrimaryKey()
                    .Field("CategoryName").Length(15)
                    .Field("Description").Length("MAX")
                    .Field("Picture").ByteArray().Length("MAX").Input(false).Output(false)
                    .Field("RowVersion").RowVersion()
                .Entity("Shippers").Version("RowVersion").Prefix("Shippers")
                    .Field("CompanyName").Length(40)
                    .Field("Phone").Length(24)
                    .Field("RowVersion").RowVersion()
                    .Field("ShipperID").Int32().PrimaryKey()
                .Relationship().LeftEntity("Order Details").LeftField("OrderID").RightEntity("Orders").RightField("OrderID")
                .Relationship().LeftEntity("Orders").LeftField("CustomerID").RightEntity("Customers").RightField("CustomerID")
                .Relationship().LeftEntity("Orders").LeftField("EmployeeID").RightEntity("Employees").RightField("EmployeeID")
                .Relationship().LeftEntity("Order Details").LeftField("ProductID").RightEntity("Products").RightField("ProductID")
                .Relationship().LeftEntity("Products").LeftField("SupplierID").RightEntity("Suppliers").RightField("SupplierID")
                .Relationship().LeftEntity("Products").LeftField("CategoryID").RightEntity("Categories").RightField("CategoryID")
                .Relationship().LeftEntity("Orders").LeftField("ShipVia").RightEntity("Shippers").RightField("ShipperID")
                .CalculatedField("CountryExchange").Length(128)
                    .Transform("format").Format("{0} to {1}")
                        .Parameter("SuppliersCountry")
                        .Parameter("OrdersShipCountry")
                .Process();

            Assert.IsNotNullOrEmpty(process.Serialize());

            //ProcessFactory.Create(process, new Options() { Mode = "init"}).Run();
            //ProcessFactory.Create(process).Run();
        }

        [Test]
        public void TestDefaultProviders() {
            var process = new ProcessBuilder("p1")
                .Connection("output").Provider("internal")
                .Process();

            Assert.IsNotNull(process);
            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual("p1Star", process.Star);

            var ready = ProcessFactory.Create(process);

            Assert.AreEqual(11, ready.Providers.Count);
        }

        [Test]
        public void TestConnection() {
            var process = new ProcessBuilder("p1")
                .Connection("input").Server("localhost").Database("Test")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(1, process.Connections.Count);
            Assert.AreEqual(500, process.Connections[0].BatchSize);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("localhost", process.Connections[0].Server);
            Assert.AreEqual("Test", process.Connections[0].Database);
        }

        [Test]
        public void TestTwoConnections() {
            var process = new ProcessBuilder("p1")
                .Connection("input").Database("I")
                .Connection("output").Database("O")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(2, process.Connections.Count);

            Assert.AreEqual(500, process.Connections[0].BatchSize);
            Assert.AreEqual("input", process.Connections[0].Name);
            Assert.AreEqual("localhost", process.Connections[0].Server);
            Assert.AreEqual("I", process.Connections[0].Database);

            Assert.AreEqual(500, process.Connections[1].BatchSize);
            Assert.AreEqual("output", process.Connections[1].Name);
            Assert.AreEqual("localhost", process.Connections[1].Server);
            Assert.AreEqual("O", process.Connections[1].Database);

        }

        [Test]
        public void TestMap() {
            var process = new ProcessBuilder("p1")
                .Connection("input").Database("I")
                .Map("m1")
                    .Item().From("one").To("1")
                    .Item().From("two").To("2").StartsWith()
                .Map("m2").Sql("SELECT [From], [To] FROM [Table];").Connection("input")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(2, process.Maps.Count);

            Assert.AreEqual(2, process.Maps[0].Items.Count);
            Assert.AreEqual("one", process.Maps[0].Items[0].From);
            Assert.AreEqual("1", process.Maps[0].Items[0].To);
            Assert.AreEqual("equals", process.Maps[0].Items[0].Operator);
            Assert.AreEqual("two", process.Maps[0].Items[1].From);
            Assert.AreEqual("2", process.Maps[0].Items[1].To);
            Assert.AreEqual("startswith", process.Maps[0].Items[1].Operator);

            Assert.AreEqual("input", process.Maps[1].Connection);
            Assert.AreEqual("SELECT [From], [To] FROM [Table];", process.Maps[1].Items.Sql);
        }

        [Test]
        public void TestEntity() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail").Version("OrderDetailVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("Order").Version("OrderVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("OrderDate").Type("System.DateTime").Default("9999-12-31")
                .Process();

            Assert.AreEqual(2, process.Entities.Count);

            var orderDetail = process.Entities[0];

            Assert.AreEqual(2, orderDetail.Fields.Count);

            Assert.AreEqual("OrderDetail", orderDetail.Name);
            Assert.AreEqual("OrderDetailVersion", orderDetail.Version);

            var orderId = orderDetail.Fields[0];
            Assert.AreEqual("OrderId", orderId.Name);
            Assert.AreEqual("int", orderId.Type);
            Assert.IsTrue(orderId.PrimaryKey);

            var productId = orderDetail.Fields[1];
            Assert.AreEqual("ProductId", productId.Name);
            Assert.AreEqual("int", productId.Type);
            Assert.IsTrue(productId.PrimaryKey);
            Assert.AreEqual("0", productId.Default);

            var order = process.Entities[1];

            Assert.AreEqual("Order", order.Name);
            Assert.AreEqual("OrderVersion", order.Version);

            orderId = order.Fields[0];
            Assert.AreEqual("OrderId", orderId.Name);
            Assert.AreEqual("int", orderId.Type);
            Assert.IsTrue(orderId.PrimaryKey);

            var orderDate = order.Fields[1];
            Assert.AreEqual("OrderDate", orderDate.Name);
            Assert.AreEqual("System.DateTime", orderDate.Type);
            Assert.IsFalse(orderDate.PrimaryKey);
            Assert.AreEqual("9999-12-31", orderDate.Default);

        }

        [Test]
        public void TestRelationship() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail").Version("OrderDetailVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("Order").Version("OrderVersion")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("OrderDate").Type("System.DateTime").Default("9999-12-31")
                .Relationship()
                    .LeftEntity("OrderDetail").LeftField("OrderId")
                    .RightEntity("Order").RightField("OrderId")
                .Process();

            Assert.AreEqual(1, process.Relationships.Count);

            var relationship = process.Relationships[0];
            Assert.AreEqual("OrderDetail", relationship.LeftEntity);
            Assert.AreEqual("OrderId", relationship.LeftField);
            Assert.AreEqual("Order", relationship.RightEntity);
            Assert.AreEqual("OrderId", relationship.RightField);
        }

        [Test]
        public void TestJoin() {
            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                .Entity("OrderDetailOptions")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                    .Field("Color").Default("Silver")
                .Relationship().LeftEntity("OrderDetail").RightEntity("Order")
                    .Join().LeftField("OrderId").RightField("OrderId")
                    .Join().LeftField("ProductId").RightField("ProductId")
                .Process();

            Assert.AreEqual(1, process.Relationships.Count);

            var relationship = process.Relationships[0];
            Assert.AreEqual("OrderDetail", relationship.LeftEntity);
            Assert.AreEqual("OrderId", relationship.Join[0].LeftField);
            Assert.AreEqual("Order", relationship.RightEntity);
            Assert.AreEqual("OrderId", relationship.Join[0].RightField);
        }

        [Test]
        public void TestFieldTransform() {

            var process = new ProcessBuilder("p1")
                .Entity("OrderDetail")
                    .Field("OrderId").Type("int").PrimaryKey()
                    .Field("Something1")
                        .Transform().Method("right").Length(4)
                    .Field("ProductId").Type("int").Default("0").PrimaryKey()
                    .Field("Something2")
                        .Transform().Method("left").Length(5)
                        .Transform().Method("trim").TrimChars("x")
                .Process();

            Assert.AreEqual(0, process.Entities[0].Fields[0].Transforms.Count);
            Assert.AreEqual(1, process.Entities[0].Fields[1].Transforms.Count);
            Assert.AreEqual(0, process.Entities[0].Fields[2].Transforms.Count);
            Assert.AreEqual(2, process.Entities[0].Fields[3].Transforms.Count);

            var left = process.Entities[0].Fields[1].Transforms[0];
            Assert.AreEqual("right", left.Method);
            Assert.AreEqual(4, left.Length);

            var right = process.Entities[0].Fields[3].Transforms[0];
            Assert.AreEqual("left", right.Method);
            Assert.AreEqual(5, right.Length);

            var trim = process.Entities[0].Fields[3].Transforms[1];
            Assert.AreEqual("trim", trim.Method);
            Assert.AreEqual("x", trim.TrimChars);
        }

        [Test]
        public void TestScript() {
            var process = new ProcessBuilder("Test")
                .Script("test").File("test.js")
                .Script("test2").File("test.js")
            .Process();

            Assert.AreEqual(2, process.Scripts.Count);
        }

    }
}