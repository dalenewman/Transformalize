# Prerequisites


## Sample Data

I used [SQLite Browser](http://sqlitebrowser.org/) to create the database [business.sqlite3](Files/business.sqlite3) with 
this [script](Files/business.sql). There are five small entities: `Company`, `Product`, `Customer`, `OrderHeader`, and `OrderLine`. 
To retrieve all relevent data from it, one writes [SQL](https://en.wikipedia.org/wiki/SQL) like this:

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

With only five entities, there is complexity in retrieving the data.  Moreover, 
as the model and data grow, the query's complexity increases, and retrieval 
performance decreases.  

The provided examples will show you how to transformalize 
these entities; making them easier and faster to query.

## Setup

To setup your development environment:

- download and decompress the latest release of Transformalize to *c:\tfl*. 
- download [business.sqlite3](Files/business.sqlite3) and place in *c:\tfl\data*.
- open a command prompt, and run:

```bash
set PATH=%PATH%;C:\tfl
```



