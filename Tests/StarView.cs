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
using Cfg.Net.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoorMansTSqlFormatterLib;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Desktop.Loggers;
using Transformalize.Provider.Ado.Ext;
using Transformalize.Provider.SqlServer;

namespace Tests {

    [TestClass]
    public class StarView {

        [TestMethod]
        public void StarSql() {
            var composer = new CompositionRoot();
            var controller = composer.Compose(@"Files\Northwind.xml");

            Assert.AreEqual(0, composer.Process.Errors().Length);

            var pipe = new PipelineContext(new TraceLogger(), composer.Process);
            var actual = new SqlFormattingManager().Format(pipe.SqlCreateStarView(new SqlServerConnectionFactory(new Connection())));

            Assert.IsNotNull(controller);
            const string expected = @"CREATE VIEW [NorthWindStar]
AS
SELECT A.[A1] AS [TflKey]
	,A.[A2] AS [TflBatchId]
	,A.[A3] AS [TflHashCode]
	,A.[A4] AS [TflDeleted]
	,A.[A5] AS [OrderDetailsDiscount]
	,A.[A6] AS [OrderDetailsOrderID]
	,A.[A7] AS [OrderDetailsProductID]
	,A.[A8] AS [OrderDetailsQuantity]
	,A.[A9] AS [OrderDetailsRowVersion]
	,A.[A10] AS [OrderDetailsUnitPrice]
	,A.[A11] AS [OrderDetailsExtendedPrice]
	,A.[A20] AS [CountryExchange]
	,A.[A21] AS [Test]
	,A.[B5] AS [OrdersCustomerID]
	,A.[B6] AS [OrdersEmployeeID]
	,A.[E12] AS [ProductsSupplierID]
	,A.[E5] AS [ProductsCategoryID]
	,A.[B19] AS [OrdersShipVia]
	,COALESCE(B.[B7], 0.0) AS [OrdersFreight]
	,COALESCE(B.[B8], '9999-12-31T00:00:00Z') AS [OrdersOrderDate]
	,COALESCE(B.[B10], '9999-12-31T00:00:00Z') AS [OrdersRequiredDate]
	,COALESCE(B.[B12], '') AS [OrdersShipAddress]
	,COALESCE(B.[B13], '') AS [OrdersShipCity]
	,COALESCE(B.[B14], '') AS [OrdersShipCountry]
	,COALESCE(B.[B15], '') AS [OrdersShipName]
	,COALESCE(B.[B16], '9999-12-31T00:00:00Z') AS [OrdersShippedDate]
	,COALESCE(B.[B17], '') AS [OrdersShipPostalCode]
	,COALESCE(B.[B18], '') AS [OrdersShipRegion]
	,COALESCE(B.[B20], '12-DEC') AS [TimeOrderMonth]
	,COALESCE(B.[B21], '9999-12-31') AS [TimeOrderDate]
	,COALESCE(B.[B22], '9999') AS [TimeOrderYear]
	,COALESCE(C.[C5], '') AS [CustomersAddress]
	,COALESCE(C.[C6], '') AS [CustomersCity]
	,COALESCE(C.[C7], '') AS [CustomersCompanyName]
	,COALESCE(C.[C8], '') AS [CustomersContactName]
	,COALESCE(C.[C9], '') AS [CustomersContactTitle]
	,COALESCE(C.[C10], '') AS [CustomersCountry]
	,COALESCE(C.[C12], '') AS [CustomersFax]
	,COALESCE(C.[C13], '') AS [CustomersPhone]
	,COALESCE(C.[C14], '') AS [CustomersPostalCode]
	,COALESCE(C.[C15], '') AS [CustomersRegion]
	,COALESCE(D.[D5], '') AS [EmployeesAddress]
	,COALESCE(D.[D6], '9999-12-31T00:00:00Z') AS [EmployeesBirthDate]
	,COALESCE(D.[D7], '') AS [EmployeesCity]
	,COALESCE(D.[D8], '') AS [EmployeesCountry]
	,COALESCE(D.[D10], '') AS [EmployeesExtension]
	,COALESCE(D.[D11], '') AS [EmployeesFirstName]
	,COALESCE(D.[D12], '9999-12-31T00:00:00Z') AS [EmployeesHireDate]
	,COALESCE(D.[D13], '') AS [EmployeesHomePhone]
	,COALESCE(D.[D14], '') AS [EmployeesLastName]
	,COALESCE(D.[D15], '') AS [EmployeesNotes]
	,COALESCE(D.[D18], '') AS [EmployeesPostalCode]
	,COALESCE(D.[D19], '') AS [EmployeesRegion]
	,COALESCE(D.[D21], '') AS [EmployeesTitle]
	,COALESCE(D.[D22], '') AS [EmployeesTitleOfCourtesy]
	,COALESCE(D.[D23], 0) AS [EmployeesReportsTo]
	,COALESCE(D.[D24], '') AS [EmployeesManager]
	,COALESCE(D.[D25], '') AS [Employee]
	,COALESCE(E.[E6], 0) AS [ProductsDiscontinued]
	,COALESCE(E.[E8], '') AS [ProductsProductName]
	,COALESCE(E.[E9], '') AS [ProductsQuantityPerUnit]
	,COALESCE(E.[E10], 0) AS [ProductsReorderLevel]
	,COALESCE(E.[E13], 0.0) AS [ProductsUnitPrice]
	,COALESCE(E.[E14], 0) AS [ProductsUnitsInStock]
	,COALESCE(E.[E15], 0) AS [ProductsUnitsOnOrder]
	,COALESCE(F.[F5], '') AS [SuppliersAddress]
	,COALESCE(F.[F6], '') AS [SuppliersCity]
	,COALESCE(F.[F7], '') AS [SuppliersCompanyName]
	,COALESCE(F.[F8], '') AS [SuppliersContactName]
	,COALESCE(F.[F9], '') AS [SuppliersContactTitle]
	,COALESCE(F.[F10], '') AS [SuppliersCountry]
	,COALESCE(F.[F11], '') AS [SuppliersFax]
	,COALESCE(F.[F12], '') AS [SuppliersHomePage]
	,COALESCE(F.[F13], '') AS [SuppliersPhone]
	,COALESCE(F.[F14], '') AS [SuppliersPostalCode]
	,COALESCE(F.[F15], '') AS [SuppliersRegion]
	,COALESCE(G.[G6], '') AS [CategoriesCategoryName]
	,COALESCE(G.[G7], '') AS [CategoriesDescription]
	,COALESCE(H.[H5], '') AS [ShippersCompanyName]
	,COALESCE(H.[H6], '') AS [ShippersPhone]
FROM [NorthWindOrder DetailsTable] A
LEFT OUTER JOIN [NorthWindOrdersTable] B ON (A.[A6] = B.[B9])
LEFT OUTER JOIN [NorthWindCustomersTable] C ON (A.[B5] = C.[C11])
LEFT OUTER JOIN [NorthWindEmployeesTable] D ON (A.[B6] = D.[D9])
LEFT OUTER JOIN [NorthWindProductsTable] E ON (A.[A7] = E.[E7])
LEFT OUTER JOIN [NorthWindSuppliersTable] F ON (A.[E12] = F.[F17])
LEFT OUTER JOIN [NorthWindCategoriesTable] G ON (A.[E5] = G.[G5])
LEFT OUTER JOIN [NorthWindShippersTable] H ON (A.[B19] = H.[H8]);
";

            Assert.AreEqual(expected, actual);
        }
    }


}
