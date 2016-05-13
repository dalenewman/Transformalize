# Transformalize
Transformalize is released under the Apache 2 license.

## What is it?
Transformalize is a configurable ETL library specializing in 
incremental denormalization.

**Caution**: It is still under development.  Breaking changes are guarenteed.

### Key Principles

1. [ETL](#ETL) (Extract, Transform, and Load)
1. [Configurable](#CFG)
1. [Denormalization](#DEN)
1. [Incremental](#INC)
1. [Embrace Change](#CHG)

#### <a name="ETL"></a>ETL
Fundamentally, a Transformalize process:

- defines input(s) from which to **extract** data from 
- optionally defines **transformations** to the data
- **Loads** the data into a consistent *transformalized* output

#### <a name="CFG"></a>Configurable
Instead of:

1. Starting a project in an IDE
1. Coding
1. Compiling
1. Deploying

Once deployed, a Transformalize runs ETL as defined 
by an arrangement (aka configuration). Currently that is 
an XML or JSON document.

#### <a name="DEN"></a>Denormalization
Relational data is usually normalized to minimize data redundancy. 
This means the data is separated into specific entities 
and related with keys.

A Transformalize configuration models the relationships between 
input entities and outputs a star-schema and denormalized view of 
them.

**Note**: Denormalization only occurs when you define more than one entity.

#### <a name="INC"></a>Incremental
Initially, Transformalize processes all of your data.  Subsequent 
processing attempts to pull incremental updates from your input and 
apply them to your output.

Setup correctly with *version* fields (a field that increments everytime a 
row is updated), subsequent processing can be very fast and efficient.

#### <a name="CHG"></a>Embrace Change
Usually, when you gather data from many sources, it's for something like 
a [data warehouse](https://en.wikipedia.org/wiki/Data_warehouse) or 
[search engine](https://en.wikipedia.org/wiki/Search_engine_(computing)). These support 
analysis, browsing, and/or searching the data.

In business, when your present data to whomever is asking for it, 
they're first response is to ask for more or different data :-)

Because this is the *nature of the beast*, Transformalize has an 
easy way to handle change:

1. Stop incremental processing
1. Modify your configuration
1. Re-process (initialize)
1. Re-enable incremental processing

Transformalize has the power to create and destroy.

---

## Getting Started

**TODO**: Write Getting Started...

**NOTE**: This code-base is the second implementation of the idea and principles 
defined above.  To find out more about how Transformalize works, 
you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind) 
on Code Project (based on the first implementation).

 







