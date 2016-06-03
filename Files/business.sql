CREATE TABLE `Product` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`Name`	TEXT  NOT NULL,
	`UnitPrice`	NUMERIC NOT NULL,
	`CompanyId`	INTEGER NOT NULL,
	`Created` TIMESTAMP NOT NULL,
	`Modified` TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE TABLE `Company` (
	`Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`Name`	TEXT NOT NULL,
	`Created` TIMESTAMP NOT NULL,
	`Modified` TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);


CREATE TABLE `OrderHeader` (
	`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`CustomerId` INTEGER NOT NULL,
	`Created` TIMESTAMP NOT NULL,
	`Modified` TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
)

CREATE TABLE `Customer` (
	`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`Prefix` TEXT NOT NULL,
	`FirstName` TEXT NOT NULL,
	`MiddleName` TEXT NOT NULL,
	`LastName` TEXT NOT NULL,
	`Suffix` TEXT NOT NULL,
	`Created` TIMESTAMP NOT NULL,
	`Modified` TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
)

CREATE TABLE `OrderLine` (
	`OrderHeaderId` INTEGER NOT NULL,
	`ProductId` INTEGER NOT NULL,
	`UnitPrice` NUMERIC NOT NULL,
    `Quantity` INTEGER NOT NULL,	
	`Created` TIMESTAMP NOT NULL,
	`Modified` TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
	PRIMARY KEY (OrderHeaderId, ProductId)
)

INSERT INTO Customer(Prefix,FirstName,MiddleName,LastName,Suffix,Created) VALUES ('Mr.','Derp','','Noobman','Jr',CURRENT_TIMESTAMP);
INSERT INTO Customer(Prefix,FirstName,MiddleName,LastName,Suffix,Created) VALUES ('Mr.','Derl','P','Nooberman','',CURRENT_TIMESTAMP);
INSERT INTO Customer(Prefix,FirstName,MiddleName,LastName,Suffix,Created) VALUES ('Mr.','Daryl','E','Noobstein','',CURRENT_TIMESTAMP);

INSERT INTO Company(Name,Created) VALUES ('Microsoft', CURRENT_TIMESTAMP);
INSERT INTO Company(Name,Created) VALUES ('Google', CURRENT_TIMESTAMP);
INSERT INTO Company(Name,Created) VALUES ('Apple', CURRENT_TIMESTAMP);
INSERT INTO Company(Name,Created) VALUES ('Bell''s Beer', CURRENT_TIMESTAMP);
INSERT INTO Company(Name,Created) VALUES ('Frito-Lay', CURRENT_TIMESTAMP);
INSERT INTO Company(Name,Created) VALUES ('Mojang', CURRENT_TIMESTAMP);

INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Hololens',1,3000,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Surface Book',1,1599,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Card Board',2,15,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Apple Watch',3,499,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('iPhone 6S Plus',3,699,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Oberon',4,1.66,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Doritos',5,3.29,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Cracker Jack',5,1.19,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Minecraft',6,26.95,CURRENT_TIMESTAMP);
INSERT INTO Product(Name,CompanyId,UnitPrice,Created) VALUES('Cobalt',6,19.99,CURRENT_TIMESTAMP);

INSERT INTO OrderHeader(CustomerId,Created) VALUES(1,'2016-05-28 15:36:56');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(1,'2016-06-03 13:20:19');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(2,'2016-04-24 09:10:17');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(2,'2016-05-15 11:15:23');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(3,'2016-03-04 08:05:03');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(3,'2016-04-05 14:22:10');
INSERT INTO OrderHeader(CustomerId,Created) VALUES(3,'2016-06-07 19:13:10');

INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(1,1,3000,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(1,6,1.19,6,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(2,2,1499,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(2,6,1.09,12,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(3,3,15,2,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(3,6,1.29,4,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(4,6,1.19,6,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(5,4,499,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(5,9,26.95,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(5,10,19.99,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(5,8,1.19,3,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(6,4,399,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(6,5,699,1,CURRENT_TIMESTAMP);
INSERT INTO OrderLine(OrderHeaderId,ProductId,UnitPrice,Quantity,Created) VALUES(6,7,3.19,1,CURRENT_TIMESTAMP);








