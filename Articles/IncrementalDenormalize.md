### <a name="DEN"></a>De-normalization
The normal work-flow to deploy an arrangement is:

- Create your arrangement
- Execute TFL in `init` mode
- Install TFL as a service with [nssm](https://nssm.cc)
  - running incrementals on a schedule (using `-s` flag) enabled by [Quartz.net](http://www.quartz-scheduler.net/) cron expression.

To modify your arrangement:

1. Stop the incremental processing (stop the service)
1. Edit your XML or JSON.
1. Execute TFL in `init` mode (to rebuild it)
1. start the incremental processing (start the service)

A TFL output is disposable. You may routinely create and destroy it.

For a simple example, check out [Hello Countries](https://dotnetfiddle.net/G2Rbwn) on dotNetFiddle<a href="#dnf">*</a>.


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

![Relational to Star](../Files/er-to-star.png)

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


<a name="dnf">*</a>dotNetFiddle: attempts to run small programs from your web browser.  I created a special [nuget package](https://www.nuget.org/packages/Pipeline.DotNetFiddle) for running limited arrangements on dotNetFiddle. Sometimes
executing TFL on dotNetFiddle exceeds the execution time limit.  Often you can just re-run it to get it to work.







