# Transformalize
Transformalize is released under the Apache 2 license.

**Caution**: It is still under development.  Breaking changes are guarenteed.


## What is it?
Transformalize (aka TFL) is a configurable [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load) 
solution specializing in incremental [de-normalization](https://en.wikipedia.org/wiki/Denormalization). 
It is used to prepare data for [data warehouses](https://en.wikipedia.org/wiki/Data_warehouse), 
[search engines](https://en.wikipedia.org/wiki/Search_engine_%28computing%29), 
and other forms of analysis and/or presentation.

### <a name="CFG"></a>Configurable
TFL processes (aka arrangements) are created and modified in an [XML](https://en.wikipedia.org/wiki/XML) or 
[JSON](https://en.wikipedia.org/wiki/JSON) editor. Creating an arrangement is writing a 
configuration.

To modify your arrangement:

1. Disable incremental processing
1. Edit your XML or JSON.
1. Execute TFL in `init` mode (to rebuild it)
1. Enable incremental processing

A TFL output is disposable. You may routinely create and destroy it.

### <a name="ETL"></a>ETL
At it's heart, TFL is [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load) 
(**E**xtract, **T**ransform, and **L**oad). However, it's not general purpose. 
Instead, it specializes in transforming relational models 
into [star-schemas](https://en.wikipedia.org/wiki/Star_schema). 
While input varies, output structure is pre-determined.  This is not to 
say you can't add transformations.  You may add 
single and multi-entity transformations.

### <a name="DEN"></a>De-normalization
[Normalization](https://en.wikipedia.org/wiki/Database_normalization) of data is 
performed to maintain data integrity and minimize data redundancy. Data is separated into meaningful 
entities and related with keys.  Integrity is enforced by constraints and relationships. While 
this is optimal for storage, it introduces some complexity and performance issues for retrieval.

[De-normalization](https://en.wikipedia.org/wiki/Denormalization) reverses normalization 
in order to reduce complexity and improve performance of retrieval.

Ideally, we want the benefits of normalized and de-normalized data. So, we store 
data in a normalized [RDBMS](https://en.wikipedia.org/wiki/Relational_database_management_system), 
and we de-normalize it for our [data warehouses](https://en.wikipedia.org/wiki/Data_warehouse), 
[search engines](https://en.wikipedia.org/wiki/Search_engine_(computing)), and other needs.

Using relational input and output, a TFL process re-arranges related entities 
into a star-schema and provides a de-normalized (flat) view of the data.

![Relational to Star](Files/er-to-star.png)

In the graphic above, TFL transforms the relational model (on the left), to the star-schema (on the right). 
It is easier for other value-adding data services to take advantage of the star-schema.  The data in 
the star-schema is kept updated by incrementals.

### Incrementals
Initially, TFL processes all of your data per the arrangement. Subsequent 
processing targets new, updated, and deleted data. If you do not physically 
delete rows, but instead mark rows as *deleted*, then you may omit checking 
for deletes. Setup with *version* fields (a field that increments it's value on update), 
incrementals are fast and efficient.

TFL may be setup as a service with [nssm](https://nssm.cc) and run 
incrementals based on cron expressions (enabled by [Quartz.net](http://www.quartz-scheduler.net/)).

---

<!--
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

-->

### Getting Started

The best way you can understand how to use TFL is by reviewing examples.

1. [Prerequisites](Articles/Example-00-Prerequisites.md)
1. [Example 1: Working with a Single Entity](Articles/Example-01-Single-Entity.md)

---

**NOTE**: This code-base is the 2nd implementation.  To find out more about 
how Transformalize works, you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the 1st implementation).

 







