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
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Test.Builders;
using Process = Transformalize.Main.Process;

namespace Transformalize.Test {
    [TestFixture]
    public class TestProcess {

        private static Process GetTestProcess(string outputProvider) {

            var process = new ProcessBuilder("Test")
                .StarEnabled(false)
                .Connection("input").Provider("internal")
                .Connection("output").Provider(outputProvider)
                .Entity("OrderDetail")
                    .Schema("dbo")
                    .Field("OrderDetailId").PrimaryKey()
                    .Field("OrderId")
                    .Field("ProductId")
                    .Field("Price")
                    .Field("Quantity")
                .Entity("Order")
                    .Schema("dbo")
                    .Field("OrderId").PrimaryKey()
                    .Field("CustomerId")
                .Entity("Customer")
                    .Schema("dbo")
                    .Field("CustomerId").PrimaryKey()
                    .Field("FirstName")
                    .Field("LastName")
                .Entity("Product")
                    .Schema("dbo")
                    .Field("ProductId").PrimaryKey()
                    .Field("Name").Alias("Product")
                    .Field("Description")
                .Entity("ProductCategory")
                    .Schema("dbo")
                    .Field("ProductId").PrimaryKey()
                    .Field("CategoryId").PrimaryKey()
                .Entity("Category")
                    .Schema("dbo")
                    .Field("CategoryId").PrimaryKey()
                    .Field("Name").Alias("Category")
                .Relationship()
                    .LeftEntity("OrderDetail").LeftField("OrderId")
                    .RightEntity("Order").RightField("OrderId")
                .Relationship()
                    .LeftEntity("Order").LeftField("CustomerId")
                    .RightEntity("Customer").RightField("CustomerId")
                .Relationship()
                    .LeftEntity("OrderDetail").LeftField("ProductId")
                    .RightEntity("Product").RightField("ProductId")
                .Relationship()
                    .LeftEntity("Product").LeftField("ProductId")
                    .RightEntity("ProductCategory").RightField("ProductId")
                .Relationship()
                    .LeftEntity("ProductCategory").LeftField("CategoryId")
                    .RightEntity("Category").RightField("CategoryId")
            .Process();

            process = new TflRoot(process).Processes[0];

            process.Entities[0].InputOperation = new RowsBuilder()
                .Row("OrderDetailId", 1).Field("OrderId", 1).Field("ProductId", 1).Field("Quantity", 2).Field("Price", 2.0)
                .Row("OrderDetailId", 2).Field("OrderId", 1).Field("ProductId", 2).Field("Quantity", 2).Field("Price", 3.0)
                .Row("OrderDetailId", 3).Field("OrderId", 2).Field("ProductId", 2).Field("Quantity", 4).Field("Price", 3.0)
                .ToOperation();

            process.Entities[1].InputOperation = new RowsBuilder()
                .Row("OrderId", 1).Field("CustomerId", 1)
                .Row("OrderId", 2).Field("CustomerId", 2)
                .ToOperation();

            process.Entities[2].InputOperation = new RowsBuilder()
                .Row("CustomerId", 1).Field("FirstName", "Dale").Field("LastName", "Newman")
                .Row("CustomerId", 2).Field("FirstName", "Tara").Field("LastName", "Newman")
                .ToOperation();

            process.Entities[3].InputOperation = new RowsBuilder()
                .Row("ProductId", 1).Field("Name", "Beans").Field("Description", "Really nice beans.")
                .Row("ProductId", 2).Field("Name", "Cheese").Field("Description", "Very sharp cheese.")
                .ToOperation();

            process.Entities[4].InputOperation = new RowsBuilder()
                .Row("ProductId", 1).Field("CategoryId", 1)
                .Row("ProductId", 1).Field("CategoryId", 2)
                .Row("ProductId", 2).Field("CategoryId", 3)
                .ToOperation();

            process.Entities[5].InputOperation = new RowsBuilder()
                .Row("CategoryId", 1).Field("Name", "Proteins")
                .Row("CategoryId", 2).Field("Name", "Miracle Fruits")
                .Row("CategoryId", 3).Field("Name", "Dairy")
                .ToOperation();

            return ProcessFactory.CreateSingle(process, new TestLogger());

        }

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
                .SearchType("facet").Analyzer("lowercase").Store(true).Index(true)
                .SearchType("standard").Analyzer("standard_lowercase").Store(false).Index(true)
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

            //ProcessFactory.Create(process, new Options() { Mode = "init"}).Run();
            //ProcessFactory.Create(process).Run();
        }

        [Test]
        public void TestConnection() {
            var process = new ProcessBuilder("p1")
                .Connection("input").Server("localhost").Database("Test")
                .Process();

            Assert.AreEqual("p1", process.Name);
            Assert.AreEqual(2, process.Connections.Count, "There should be 2 connections because an internal output is automatically added.");
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
            Assert.AreEqual("SELECT [From], [To] FROM [Table];", process.Maps[1].Query);
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
            Assert.AreEqual("datetime", orderDate.Type);
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

        [Test]
        public void TestViewSql() {

            const string expected = @"SELECT
    m.[OrderDetailId],
    m.[OrderId],
    m.[ProductId],
    m.[Price],
    m.[Quantity],
    r1.[CustomerId],
    r2.[FirstName],
    r2.[LastName],
    r3.[Product],
    r3.[Description],
    r4.[CategoryId],
    r5.[Category]
FROM [TestOrderDetail] m
LEFT OUTER JOIN [TestOrder] r1 ON (m.[OrderId] = r1.[OrderId])
LEFT OUTER JOIN [TestCustomer] r2 ON (r1.[CustomerId] = r2.[CustomerId])
LEFT OUTER JOIN [TestProduct] r3 ON (m.[ProductId] = r3.[ProductId])
LEFT OUTER JOIN [TestProductCategory] r4 ON (r3.[ProductId] = r4.[ProductId])
LEFT OUTER JOIN [TestCategory] r5 ON (r4.[CategoryId] = r5.[CategoryId]);";

            var process = GetTestProcess("sqlserver");
            var actual = new SqlServerViewWriter().ViewSql(process);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestJoinInternalOutput() {
            var process = GetTestProcess("internal");
            var results = process.Execute().ToArray();

            Assert.IsInstanceOf<IEnumerable<Row>>(results);
            Assert.AreEqual(4, results.Length);

            var r0 = results[0];
            Assert.AreEqual(1, r0["OrderDetailId"]);
            Assert.AreEqual(1, r0["OrderId"]);
            Assert.AreEqual(1, r0["ProductId"]);
            Assert.AreEqual(2, r0["Quantity"]);
            Assert.AreEqual(2.0m, r0["Price"]);
            Assert.AreEqual(1, r0["CustomerId"]);
            Assert.AreEqual("Dale", r0["FirstName"]);
            Assert.AreEqual("Beans", r0["Product"]);
            Assert.AreEqual("Proteins", r0["Category"]);

            var r1 = results[1];
            Assert.AreEqual(1, r1["OrderDetailId"]);
            Assert.AreEqual(1, r1["OrderId"]);
            Assert.AreEqual(1, r1["ProductId"]);
            Assert.AreEqual(2, r1["Quantity"]);
            Assert.AreEqual(2.0m, r1["Price"]);
            Assert.AreEqual(1, r1["CustomerId"]);
            Assert.AreEqual("Dale", r1["FirstName"]);
            Assert.AreEqual("Beans", r1["Product"]);
            Assert.AreEqual("Miracle Fruits", r1["Category"]);

            var r2 = results[2];
            Assert.AreEqual(2, r2["OrderDetailId"]);
            Assert.AreEqual(1, r2["OrderId"]);
            Assert.AreEqual(2, r2["ProductId"]);
            Assert.AreEqual(2, r2["Quantity"]);
            Assert.AreEqual(3.0m, r2["Price"]);
            Assert.AreEqual(1, r2["CustomerId"]);
            Assert.AreEqual("Dale", r2["FirstName"]);
            Assert.AreEqual("Cheese", r2["Product"]);
            Assert.AreEqual("Dairy", r2["Category"]);

            var r3 = results[3];
            Assert.AreEqual(3, r3["OrderDetailId"]);
            Assert.AreEqual(2, r3["OrderId"]);
            Assert.AreEqual(2, r3["ProductId"]);
            Assert.AreEqual(4, r3["Quantity"]);
            Assert.AreEqual(3.0m, r3["Price"]);
            Assert.AreEqual(2, r3["CustomerId"]);
            Assert.AreEqual("Tara", r3["FirstName"]);
            Assert.AreEqual("Cheese", r3["Product"]);
            Assert.AreEqual("Dairy", r3["Category"]);

        }

        [Test]
        public void TestOutputToJson() {
            var process = GetTestProcess("internal");
            var results = process.Execute().ToArray();

            const string json = @"
[
    {
        'OrderDetailId':1,
        'Order':{
            'OrderId':1,
            'Customer':{
                'CustomerId':1,
                'FirstName':'Dale',
                'LastName':'Newman'
            },
            'Product':{
                'ProductId':1,
                'Name':'Beans',
                'Description':'Really Nice Beans',
                'Categories':[
                    {
                        'CategoryId':1,
                        'Name':'Proteins'
                    },
                    {
                        'CategoryId':2,
                        'Name':'Miracle Fruits'
                    }
                ]
            }
        },
        'Quantity':2,
        'Price':2.0
    },
    {
        'OrderDetailId':2,
        'Order':{
            'OrderId':1,
            'Customer':{
                'CustomerId':1,
                'FirstName':'Dale',
                'LastName':'Newman'
            },
            'Product':{
                'ProductId':2,
                'Name':'Cheese',
                'Description':'Very Sharp Cheese',
                'Category':[
                    {
                        'CategoryId':3,
                        'Name':'Dairy'
                    }
                ]

            }
        },
        'Quantity':2,
        'Price':3.0
    },
    {
        'OrderDetailId':3,
        'Order':{
            'OrderId':2,
            'Customer':{
                'CustomerId':2,
                'FirstName':'Tara',
                'LastName':'Newman'
            },
            'Product':{
                'ProductId':2,
                'Name':'Cheese',
                'Description':'Very Sharp Cheese',
                'Category':[
                    {
                        'CategoryId':3,
                        'Name':'Dairy'
                    }
                ]
            }
        },
        'Quantity':4,
        'Price':3.0
    }
]";
            //var expected = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json.Replace("'", "\""));

            //Assert.AreEqual(3, expected);
            //Assert.AreEqual(1, (expected[0])["OrderDetailId"]);
            //ssert.AreEqual(2, (expected[1])["OrderDetailId"]);
            //Assert.AreEqual(3, (expected[2])["OrderDetailId"]);

        }


    }



}