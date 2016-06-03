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

**TODO**: Write Getting Started...

**NOTE**: This code-base is the second implementation of the idea and principles 
defined above.  To find out more about how Transformalize works, 
you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the first implementation).

 







