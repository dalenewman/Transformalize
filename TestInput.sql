USE TestInput;
SET NOCOUNT ON;

IF OBJECT_ID('dbo.OrderDetail') IS NOT NULL
	DROP TABLE OrderDetail;
GO
IF OBJECT_ID('dbo.Order') IS NOT NULL
	DROP TABLE [Order];
GO
IF OBJECT_ID('dbo.Customer') IS NOT NULL
	DROP TABLE Customer;
GO
IF OBJECT_ID('dbo.Product') IS NOT NULL
	DROP TABLE Product;
GO

CREATE TABLE OrderDetail (
	OrderDetailKey INT PRIMARY KEY IDENTITY(1,1),
	OrderKey INT NOT NULL,
	ProductKey INT NOT NULL,
	Qty INT NOT NULL,
	Price DECIMAL(10,5) NOT NULL,
	Properties XML NOT NULL,
	[RowVersion] ROWVERSION NOT NULL
);
GO

INSERT INTO OrderDetail(OrderKey, ProductKey, Qty, Price, Properties) VALUES (1,1,1,1.00,'<Properties><Color>Red</Color><Size>Large</Size><Gender>Male</Gender></Properties>');
INSERT INTO OrderDetail(OrderKey, ProductKey, Qty, Price, Properties) VALUES (1,2,1,1.00,'<Properties><Color>Blue</Color><Size>Medium</Size><Gender>Femail</Gender></Properties>');
INSERT INTO OrderDetail(OrderKey, ProductKey, Qty, Price, Properties) VALUES (2,3,2,2.00,'<Properties><Color>Green</Color><Size>Small</Size><Gender>Female</Gender></Properties>');
INSERT INTO OrderDetail(OrderKey, ProductKey, Qty, Price, Properties) VALUES (3,4,3,3.00,'<Properties><Color>Black</Color><Size>X-Large</Size><Gender>Male</Gender></Properties>');
GO

SELECT * FROM OrderDetail;
GO

CREATE TABLE [Order] (
	OrderKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	CustomerKey INT NOT NULL,
	OrderDate DATETIME NOT NULL,
	[RowVersion] ROWVERSION NOT NULL
);
GO

INSERT INTO [Order](CustomerKey, OrderDate) VALUES (1,GETDATE());
INSERT INTO [Order](CustomerKey, OrderDate) VALUES (2,GETDATE());
INSERT INTO [Order](CustomerKey, OrderDate) VALUES (3,GETDATE());
GO

SELECT * FROM [Order];
GO

CREATE TABLE Customer (
	CustomerKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[First] NVARCHAR(50) NOT NULL,
	[Last] NVARCHAR(50) NOT NULL,
	[Address] NVARCHAR(100) NOT NULL,
	City NVARCHAR(50) NOT NULL,
	[State] NCHAR(2) NOT NULL,
	Country NCHAR(2) NOT NULL,
	[RowVersion] ROWVERSION NOT NULL
);
GO

INSERT INTO Customer([First],[Last],[Address],City,[State],Country) VALUES('Dale','Newman','306 Jones St.','Dowagiac','MI','US');
INSERT INTO Customer([First],[Last],[Address],City,[State],Country) VALUES('Jeff','Bezos','111 Amazon Ave.','Amazon','CA','US');
INSERT INTO Customer([First],[Last],[Address],City,[State],Country) VALUES('Bill','Gates','111 Microsoft Rd.','Microsoft','CA','US');
GO

SELECT * FROM Customer;
GO

CREATE TABLE Product (
	ProductKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(100) NOT NULL,
	[RowVersion] ROWVERSION NOT NULL
);
GO

INSERT INTO Product([Name]) VALUES('Resharper');
INSERT INTO Product([Name]) VALUES('PyCharm');
INSERT INTO Product([Name]) VALUES('Visual Studio 2012 Professional');
INSERT INTO Product([Name]) VALUES('Red Gate Sql Compare');
GO

SELECT * FROM Product;
GO