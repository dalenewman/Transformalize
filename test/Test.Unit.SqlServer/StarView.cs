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
   public class StarView {
      private const string Expected = @"CREATE VIEW [NorthWindStar]
AS
SELECT A.[A1] AS [TflKey]
	,A.[A2] AS [TflBatchId]
	,A.[A3] AS [TflHashCode]
	,A.[A4] AS [TflDeleted]
	,COALESCE(G.[G6], '') AS [CategoriesCategoryName]
	,COALESCE(G.[G7], '') AS [CategoriesDescription]
	,A.[A20] AS [CountryExchange]
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
	,COALESCE(D.[D25], '') AS [Employee]
	,COALESCE(D.[D5], '') AS [EmployeesAddress]
	,COALESCE(D.[D6], '9999-12-31T00:00:00Z') AS [EmployeesBirthDate]
	,COALESCE(D.[D7], '') AS [EmployeesCity]
	,COALESCE(D.[D8], '') AS [EmployeesCountry]
	,COALESCE(D.[D10], '') AS [EmployeesExtension]
	,COALESCE(D.[D11], '') AS [EmployeesFirstName]
	,COALESCE(D.[D12], '9999-12-31T00:00:00Z') AS [EmployeesHireDate]
	,COALESCE(D.[D13], '') AS [EmployeesHomePhone]
	,COALESCE(D.[D14], '') AS [EmployeesLastName]
	,COALESCE(D.[D24], '') AS [EmployeesManager]
	,COALESCE(D.[D15], '') AS [EmployeesNotes]
	,COALESCE(D.[D18], '') AS [EmployeesPostalCode]
	,COALESCE(D.[D19], '') AS [EmployeesRegion]
	,COALESCE(D.[D23], 0) AS [EmployeesReportsTo]
	,COALESCE(D.[D21], '') AS [EmployeesTitle]
	,COALESCE(D.[D22], '') AS [EmployeesTitleOfCourtesy]
	,A.[A5] AS [OrderDetailsDiscount]
	,A.[A11] AS [OrderDetailsExtendedPrice]
	,A.[A6] AS [OrderDetailsOrderID]
	,A.[A7] AS [OrderDetailsProductID]
	,A.[A8] AS [OrderDetailsQuantity]
	,A.[A9] AS [OrderDetailsRowVersion]
	,A.[A10] AS [OrderDetailsUnitPrice]
	,A.[B5] AS [OrdersCustomerID]
	,A.[B6] AS [OrdersEmployeeID]
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
	,A.[B19] AS [OrdersShipVia]
	,A.[E5] AS [ProductsCategoryID]
	,COALESCE(E.[E6], 0) AS [ProductsDiscontinued]
	,COALESCE(E.[E8], '') AS [ProductsProductName]
	,COALESCE(E.[E9], '') AS [ProductsQuantityPerUnit]
	,COALESCE(E.[E10], 0) AS [ProductsReorderLevel]
	,A.[E12] AS [ProductsSupplierID]
	,COALESCE(E.[E13], 0.0) AS [ProductsUnitPrice]
	,COALESCE(E.[E14], 0) AS [ProductsUnitsInStock]
	,COALESCE(E.[E15], 0) AS [ProductsUnitsOnOrder]
	,COALESCE(H.[H5], '') AS [ShippersCompanyName]
	,COALESCE(H.[H6], '') AS [ShippersPhone]
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
	,A.[A21] AS [Test]
	,COALESCE(B.[B21], '9999-12-31') AS [TimeOrderDate]
	,COALESCE(B.[B20], '12-DEC') AS [TimeOrderMonth]
	,COALESCE(B.[B22], '9999') AS [TimeOrderYear]
FROM [NorthWindOrder DetailsTable] A
LEFT OUTER JOIN [NorthWindOrdersTable] B ON (A.[A6] = B.[B9])
LEFT OUTER JOIN [NorthWindCustomersTable] C ON (A.[B5] = C.[C11])
LEFT OUTER JOIN [NorthWindEmployeesTable] D ON (A.[B6] = D.[D9])
LEFT OUTER JOIN [NorthWindProductsTable] E ON (A.[A7] = E.[E7])
LEFT OUTER JOIN [NorthWindSuppliersTable] F ON (A.[E12] = F.[F17])
LEFT OUTER JOIN [NorthWindCategoriesTable] G ON (A.[E5] = G.[G5])
LEFT OUTER JOIN [NorthWindShippersTable] H ON (A.[B19] = H.[H8]);
";

      [TestMethod]
      public void StarSql() {
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope($@"files/NorthWind.xml?Server={Tester.Server},{Tester.Port}&User={Tester.User}&Pw={Tester.Pw}", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule()).CreateScope(process, logger)) {

					var cleaner = new Regex(@"[\r\n\t ]");
					var expected = cleaner.Replace(Expected, string.Empty);
					var pipe = new PipelineContext(new ConsoleLogger(), process);
					var actual = cleaner.Replace(pipe.SqlCreateStarView(new SqlServerConnectionFactory(new Connection())), string.Empty);
               Assert.AreEqual(expected, actual);
            }
         }
      }
   }
}
