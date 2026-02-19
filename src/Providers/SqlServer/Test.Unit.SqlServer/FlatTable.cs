#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Providers.Console;
using Transformalize.Providers.SqlServer;
using Transformalize.Providers.SqlServer.Autofac;

namespace Test.Unit.SqlServer {

   [TestClass]
   public class FlatTable {
      private const string Expected = @"CREATE TABLE [NorthWindFlat] (
	[TflKey] INT NOT NULL
	,[TflBatchId] INT NOT NULL
	,[TflHashCode] INT NOT NULL
	,[TflDeleted] BIT NOT NULL
	,[CountryExchange] NVARCHAR(128) NOT NULL
	,[OrderDetailsDiscount] REAL NOT NULL
	,[OrderDetailsExtendedPrice] DECIMAL(19, 4) NOT NULL
	,[OrderDetailsOrderID] INT NOT NULL
	,[OrderDetailsProductID] INT NOT NULL
	,[OrderDetailsQuantity] SMALLINT NOT NULL
	,[OrderDetailsRowVersion] VARBINARY(8) NOT NULL
	,[OrderDetailsUnitPrice] DECIMAL(19, 4) NOT NULL
	,[OrdersCustomerID] NVARCHAR(5) NOT NULL
	,[OrdersEmployeeID] INT NOT NULL
	,[OrdersShipVia] INT NOT NULL
	,[ProductsCategoryID] INT NOT NULL
	,[ProductsSupplierID] INT NOT NULL
	,[Test] NVARCHAR(64) NOT NULL
	,[CategoriesCategoryName] NVARCHAR(15) NOT NULL
	,[CategoriesDescription] NVARCHAR(max) NOT NULL
	,[CustomersAddress] NVARCHAR(60) NOT NULL
	,[CustomersCity] NVARCHAR(15) NOT NULL
	,[CustomersCompanyName] NVARCHAR(40) NOT NULL
	,[CustomersContactName] NVARCHAR(30) NOT NULL
	,[CustomersContactTitle] NVARCHAR(30) NOT NULL
	,[CustomersCountry] NVARCHAR(15) NOT NULL
	,[CustomersFax] NVARCHAR(24) NOT NULL
	,[CustomersPhone] NVARCHAR(24) NOT NULL
	,[CustomersPostalCode] NVARCHAR(10) NOT NULL
	,[CustomersRegion] NVARCHAR(15) NOT NULL
	,[Employee] NVARCHAR(64) NOT NULL
	,[EmployeesAddress] NVARCHAR(60) NOT NULL
	,[EmployeesBirthDate] DATETIME NOT NULL
	,[EmployeesCity] NVARCHAR(15) NOT NULL
	,[EmployeesCountry] NVARCHAR(15) NOT NULL
	,[EmployeesExtension] NVARCHAR(4) NOT NULL
	,[EmployeesFirstName] NVARCHAR(10) NOT NULL
	,[EmployeesHireDate] DATETIME NOT NULL
	,[EmployeesHomePhone] NVARCHAR(24) NOT NULL
	,[EmployeesLastName] NVARCHAR(20) NOT NULL
	,[EmployeesManager] NVARCHAR(64) NOT NULL
	,[EmployeesNotes] NVARCHAR(max) NOT NULL
	,[EmployeesPostalCode] NVARCHAR(10) NOT NULL
	,[EmployeesRegion] NVARCHAR(15) NOT NULL
	,[EmployeesReportsTo] INT NOT NULL
	,[EmployeesTitle] NVARCHAR(30) NOT NULL
	,[EmployeesTitleOfCourtesy] NVARCHAR(25) NOT NULL
	,[OrdersFreight] DECIMAL(19, 4) NOT NULL
	,[OrdersOrderDate] DATETIME NOT NULL
	,[OrdersRequiredDate] DATETIME NOT NULL
	,[OrdersShipAddress] NVARCHAR(60) NOT NULL
	,[OrdersShipCity] NVARCHAR(15) NOT NULL
	,[OrdersShipCountry] NVARCHAR(15) NOT NULL
	,[OrdersShipName] NVARCHAR(40) NOT NULL
	,[OrdersShippedDate] DATETIME NOT NULL
	,[OrdersShipPostalCode] NVARCHAR(10) NOT NULL
	,[OrdersShipRegion] NVARCHAR(15) NOT NULL
	,[ProductsDiscontinued] BIT NOT NULL
	,[ProductsProductName] NVARCHAR(40) NOT NULL
	,[ProductsQuantityPerUnit] NVARCHAR(20) NOT NULL
	,[ProductsReorderLevel] SMALLINT NOT NULL
	,[ProductsUnitPrice] DECIMAL(19, 4) NOT NULL
	,[ProductsUnitsInStock] SMALLINT NOT NULL
	,[ProductsUnitsOnOrder] SMALLINT NOT NULL
	,[ShippersCompanyName] NVARCHAR(40) NOT NULL
	,[ShippersPhone] NVARCHAR(24) NOT NULL
	,[SuppliersAddress] NVARCHAR(60) NOT NULL
	,[SuppliersCity] NVARCHAR(15) NOT NULL
	,[SuppliersCompanyName] NVARCHAR(40) NOT NULL
	,[SuppliersContactName] NVARCHAR(30) NOT NULL
	,[SuppliersContactTitle] NVARCHAR(30) NOT NULL
	,[SuppliersCountry] NVARCHAR(15) NOT NULL
	,[SuppliersFax] NVARCHAR(24) NOT NULL
	,[SuppliersHomePage] NVARCHAR(max) NOT NULL
	,[SuppliersPhone] NVARCHAR(24) NOT NULL
	,[SuppliersPostalCode] NVARCHAR(10) NOT NULL
	,[SuppliersRegion] NVARCHAR(15) NOT NULL
	,[TimeOrderDate] NVARCHAR(10) NOT NULL
	,[TimeOrderMonth] NVARCHAR(6) NOT NULL
	,[TimeOrderYear] NVARCHAR(4) NOT NULL
	,CONSTRAINT pk_NorthWindFlat_tflkey PRIMARY KEY ([TflKey])
	);
";

      [TestMethod]
      public void FlatSql() {
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope($@"files/Northwind.xml?User={Tester.User}&Pw={Tester.Pw}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {


					var cleaner = new Regex(@"[\r\n\t ]");
					var expected = cleaner.Replace(Expected, string.Empty);
					var pipe = new PipelineContext(new ConsoleLogger(), process);
					var actual = pipe.SqlCreateFlatTable(new SqlServerConnectionFactory(new Connection()));
					Assert.AreEqual(expected, cleaner.Replace(actual, string.Empty));

				}
         }
      }
   }
}
