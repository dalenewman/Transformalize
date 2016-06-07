# Prerequisites


## Sample Data

I used [SQLite Browser](http://sqlitebrowser.org/) to create 
the database [business.sqlite3](../Files/business.sqlite3) with 
this [script](../Files/business.sql). There are five small entities: 

1. `Company`
1. `Product`
1. `Customer`
1. `OrderHeader`
1. `OrderLine`

To retrieve all relevent data from them, 
one writes [SQL](https://en.wikipedia.org/wiki/SQL) like this:

```sql
SELECT
	ol.OrderHeaderId,
	ol.ProductId,
	ol.UnitPrice,
	ol.Quantity,
	oh.CustomerId,
	oh.Created AS OrderDate,
	cu.Prefix AS CustomerPrefix,
	cu.FirstName AS CustomerFirstName,
	cu.MiddleName AS CustomerMiddleName,
	cu.LastName AS CustomerLastName,
	cu.Suffix AS CustomerSuffix,
	p.Name AS ProductName,
	p.UnitPrice AS ProductUnitPrice,
	p.CompanyId,
	co.Name as CompanyName
FROM OrderLine ol
INNER JOIN OrderHeader oh ON (oh.Id = ol.OrderHeaderId)
INNER JOIN Customer cu ON (cu.Id = oh.CustomerId)
INNER JOIN Product p ON (p.Id = ol.ProductId)
INNER JOIN Company co ON (co.Id = p.CompanyId);
```

The provided examples will show you how to transformalize these entities; 
making them easier and faster to query.

## Setup

To setup your development environment:

- download and decompress the [latest release](https://github.com/dalenewman/Transformalize/releases) of TFL to *c:\tfl*. 
- download [business.sqlite3](../Files/business.sqlite3) and place in *c:\tfl\data*.
- open a command prompt, and run:

```bash
set PATH=%PATH%;C:\tfl
```



