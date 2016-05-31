# Transformalize
Transformalize is released under the Apache 2 license.

**Caution**: It is still under development.  Breaking changes are guarenteed.


## What is it?
Transformalize is a .NET based configurable ETL solution 
specializing in incremental denormalization.

### <a name="ETL"></a>ETL
It's just an **E**xtract, **T**ransform, and **L**oad process. 
Every Transformalize arrangement defines three things:

- input from which to **extract** data
- **transformations** to said data
- output where data is **Loaded**

It's not a general ETL tool where you could *load* data into any output structure. 
Instead, depending on the output provider, your data is loaded into a 
[star-schema](https://en.wikipedia.org/wiki/Star_schema).

### <a name="CFG"></a>Configurable
Instead of:

1. Starting a project in an IDE
1. Coding
1. Compiling
1. Deploying

Transformalize runs your XML or JSON [Cfg-NET](https://github.com/dalenewman/Cfg-NET) configurations.

### <a name="DEN"></a>Denormalization
Relational data is normalized to minimize data redundancy. 
This means data is separated into specific entities with related keys. This is 
optimal for storage, but can introduce complexity and performance 
issues for retrieval.

Using an [RDBMS](https://en.wikipedia.org/wiki/Relational_database_management_system) 
based input and output, Transformalize re-arranges the 
entities into a simpler star-schema model and provides a 
de-normalized (flat) view of the data.

Currently implemented SQL-based providers are:

* SQL Server
* Postgres
* MySQL
* SQLite

There are additional providers that do not support 
de-normalization, but may be used to push denormalized data 
elsewhere, they are:

* Elastic(Search)
* SOLR
* Lucene
* Files
* Memory (to be used in other types of presentation)

### <a name="INC"></a>Incremental
Initially, Transformalize processes all your data. Subsequent 
processing targets new, updated, and/or deleted data.  Setup 
with *version* fields (a field that increments on every update), 
subsequent processing can be very fast and efficient.

Transformalize may be setup as a service to run 
incrementals based on a cron expression (enabled by [Quartz.net](http://www.quartz-scheduler.net/)). 

### <a name="CHG"></a>Embracing Change
Usually, when you gather data from many sources, it's for something like 
a [data warehouse](https://en.wikipedia.org/wiki/Data_warehouse) or 
[search engine](https://en.wikipedia.org/wiki/Search_engine_(computing)). These support 
analysis, browsing, and/or searching the data.

In business, when you present data to whomever is asking for it, 
they're first response is to ask for more or different data :-)

Because this is the *nature of the beast*, Transformalize has an 
easy way to handle change:

1. Stop incremental processing
1. Modify your configuration
1. Re-process (initialize)
1. Re-enable incremental processing

The *transformalized* output is usually treated as 
disposable.  It is routine to create and destroy it.

---

## Getting Started

**TODO**: Write Getting Started...

**NOTE**: This code-base is the second implementation of the idea and principles 
defined above.  To find out more about how Transformalize works, 
you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the first implementation).

 







