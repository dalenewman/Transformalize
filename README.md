# Transformalize
Transformalize is released under the Apache 2 license.

**Caution**: It is still under development.  Breaking changes are guarenteed.


## What is it?
Transformalize is a .NET based configurable ETL solution 
specializing in incremental denormalization. It is:

1. An [ETL](#ETL) (Extract, Transform, and Load) tool
1. [Configurable](#CFG)
1. A [Denormalization](#DEN) tool
1. An [Incremental](#INC) Updater
1. A tool that [Embraces Change](#CHG)

### <a name="ETL"></a>ETL
Fundamentally, a Transformalize process is an ETL process.  It:

- defines input(s) from which to **extract** data from 
- optionally defines **transformations** to said data
- **Loads** the data into a *transformalized* output

### <a name="CFG"></a>Configurable
Instead of:

1. Starting a project in an IDE
1. Coding
1. Compiling
1. Deploying

Transformalize runs according to your configurations. It's executable (tfl.exe) 
runs your XML or JSON configurations as ETL processes.

### <a name="DEN"></a>Denormalization
Relational data is usually normalized to minimize data redundancy. 
This means the data is separated into specific entities 
with related keys.

A Transformalize configuration models the relationships between 
input entities and outputs a star-schema and denormalized view of 
them.

**Note**: Denormalization only occurs when you define more than one entity.

### <a name="INC"></a>Incremental
Initially, Transformalize processes all of your data.  Subsequent 
processing pulls incremental updates from your input and 
applies them to your output.

Setup with *version* fields (a field that increments on every update), subsequent 
processing can be very fast and efficient.

Transformalize's executable (tfl.exe) can run your 
incrementals based on a cron expression (enabled by [Quartz.net](http://www.quartz-scheduler.net/)).

### <a name="CHG"></a>Embrace Change
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

Transformalize has the power to create and destroy (the beast?).

---

## Getting Started

**TODO**: Write Getting Started...

**NOTE**: This code-base is the second implementation of the idea and principles 
defined above.  To find out more about how Transformalize works, 
you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the first implementation).

 







