# Transformalize
Transformalize is released under the Apache 2 license.

**Caution**: It is still under development.  Breaking changes are guarenteed.


## What is it?
Transformalize is a configurable ETL solution specializing in incremental 
de-normalization.

### <a name="CFG"></a>Configurable

Transformalize processes are designed in an [XML](https://en.wikipedia.org/wiki/XML) or 
[JSON](https://en.wikipedia.org/wiki/JSON) editor. Designing a process is the same 
as writing a configuration.

### <a name="ETL"></a>ETL
At it's heart, Transformalize is an [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load) (**E**xtract, 
**T**ransform, and **L**oad) solution. 
It's not for general-purpose ETL.  Instead, it's for transforming relational tables into 
[star-schemas](https://en.wikipedia.org/wiki/Star_schema).

### <a name="DEN"></a>Denormalization
[Normalization](https://en.wikipedia.org/wiki/Database_normalization) of data is 
performed to minimize data redundancy. Data is separated into meaningful entities 
and related with keys. This is optimal for storage with integrity, but introduces 
complexity and performance issues for retrieval.

[Denormalization](https://en.wikipedia.org/wiki/Denormalization) reverses normalization 
in order to reduce complexity and improve performance of retrieval.

Ideally, we want the benefits of both normalized and de-normalized data. So, we store 
data in a normalized [RDBMS](https://en.wikipedia.org/wiki/Relational_database_management_system), 
and de-normalize it for our data-warehouses.

Using relational input and output, a Transformalize process re-arranges related entities 
into a star-schema and provides a de-normalized (flat) view of the data.

![Relational to Star](Files/er-to-star.png)

Currently implemented SQL-based providers are:

* SQL Server
* Postgres
* MySQL
* SQLite

Additional providers do not support de-normalization, but may be used 
to push denormalized data elsewhere. They are:

* ElasticSearch
* SOLR
* Lucene
* Files
* Memory (for other forms of presentation)

### <a name="INC"></a>Incremental
Initially, Transformalize processes all your data. Subsequent 
processing targets new, updated, and/or deleted data.  Setup 
with *version* fields (a field that increments on every update), 
subsequent processing can be very fast and efficient.

Transformalize may be setup as a service to run 
incrementals based on a cron expression (enabled by [Quartz.net](http://www.quartz-scheduler.net/)). 

### <a name="CHG"></a>Agile
Usually, when you gather data from many sources, it's for something like 
a [data warehouse](https://en.wikipedia.org/wiki/Data_warehouse) or 
[search engine](https://en.wikipedia.org/wiki/Search_engine_(computing)). These support 
analysis, browsing, and/or searching.

In business, when you present data to whomever is asking for it, 
they're first response is to ask for more or different data :-)  This is the 
*nature of the beast*, so you need to be able to add more and/or different 
data quickly.  Transformalize has an easy way to handle change requests:

1. Stop incremental processing
1. Modify your configuration
1. Re-process (initialize)
1. Re-enable incremental processing

*Transformalized* output is usually treated as disposable.  It is routine to 
create and destroy it.

---

## Getting Started

### Sample Data
To demonstrate, I need data.  So, using [SQLite Browser](http://sqlitebrowser.org/), 
I created the database: [business.sqlite3](Files\business.sqlite3). 
The script is also [available](Files\business.sql).  There are five small 
tables: `Company`, `Product`, `Customer`, `OrderHeader`, and `OrderLine`.  To retrieve all 
the data from it, we'd write a query like this:

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

As the model and data grow, the query's complexity will 
increase, and it's performance will decrease.

#### Order Line
Let's start with `OrderLine` since it's related to everything. 
Create *c:\temp\Business.Xml* and copy the XML:

```xml
<cfg name="Business">
    <connections>
        <add name="input" provider="sqlite" file="c:\temp\business.sqlite3" />
    </connections>
    <entities>
        <add name="OrderLine" />
    </entities>
</cfg>
```

Run `tfl.exe -ac:\temp\Business.xml`.  This should produce the warning:

> The entity OrderLine doesn't have any input fields defined.

Your *c:\temp\Business.xml* is checked everytime you try and 
run it.  Any errors found will stop it from running.  The above 
was a warning.  It was followed by the line:

> Detected 6 fields in OrderLine.

So, Transformalize saw that we hadn't defined any fields, or 
an output, and it detected the fields and output to the console:

```bash
OrderHeaderId,ProductId,UnitPrice,Quantity,Created,Modified
1,1,3000,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
1,6,1.19,6,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
2,2,1499,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
2,6,1.09,12,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
3,3,15,2,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
3,6,1.29,4,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
4,6,1.19,6,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
5,4,499,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
5,9,26.95,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
5,10,19.99,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
5,8,1.19,3,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
6,4,399,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
6,5,699,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
6,7,3.19,1,6/3/2016 1:49:11 PM,6/3/2016 1:49:11 PM
```

By default, the output is `csv`, but you can change it to `json` 
with the -o flag if you want.

Transformalize is **e**xtracting `OrderLine` from the 
SQLite database and **l**oading it to the console.  So, this is 
still ETL; just minus the **T** at this point.

It's nice that it detected fields, but I need them recorded 
in the arrangement file *Business.xml*. So, I run `tfl` in `check` mode, pipe the output to *c:\temp\check.xml*, 
and open *c:\temp\check.xml* in whatever Windows associates XML file
with:

`tfl -ac:\temp\Business.xml -lnone -mcheck > c:\temp\check.xml && start c:\temp\check.xml`

![NotePad++ Screen Shot](Files/notepadpp.png)

---

TBC (To be Continued)...

---

**NOTE**: This code-base is the second implementation of the idea and principles 
defined above.  To find out more about how Transformalize works, 
you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the first implementation).

 







