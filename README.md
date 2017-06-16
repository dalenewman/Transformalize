# Transformalize

Transformalize is an [open source](https://github.com/dalenewman/Transformalize) 
extract, transform, and load ([ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load)) tool. 
It expedites mundane data processing tasks like cleaning, reporting, [denormalization](https://en.wikipedia.org/wiki/Denormalization), 
and incrementally updating your data.

It works with many data sources:

<table class="table table-condensed">
    <thead>
        <tr>
            <th>Provider</th>
            <th>Read<br/>Input</th>
            <th>Write<br/>Output</th>
            <th>De-<br/>normalize</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>SQL Server</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
        </tr>
        <tr>
            <td>MySql</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
        </tr>
        <tr>
            <td>PostgreSql</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
        </tr>
        <tr>
            <td>SQLite</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
        </tr>
        <tr>
            <td>SqlCe</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
        </tr>
        <tr>
            <td>Elasticsearch</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td> </td>
        </tr>
        <tr>
            <td>Files</td>
            <td style="color:green;">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td> </td>
        </tr>
        <tr>
            <td>Web</td>
            <td style="color:green">&#10003;</td>
            <td> </td>
            <td> </td>
        </tr>
        <tr>
            <td>SOLR</td>
            <td style="color:green">&#10003;</td>
            <td> </td>
            <td> </td>
        </tr>
        <tr>
            <td>Lucene</td>
            <td style="color:green">&#10003;</td>
            <td style="color:green">&#10003;</td>
            <td> </td>
        </tr>
        <tr>
            <td>Console</td>
            <td> </td>
            <td style="color:green">&#10003;</td>
            <td> </td>
        </tr>
    </tbody>
</table>

Jobs are arranged in [XML](https://en.wikipedia.org/wiki/XML)
or [JSON](https://en.wikipedia.org/wiki/JSON) and executed 
with a [CLI](https://en.wikipedia.org/wiki/Command-line_interface) or 
in an [Orchard CMS](http://www.orchardproject.net/) module.

---

### Transformalizing Northwind

This document demonstrates the transformation (and denormalization) 
of Northwind's relational tables into a [star-schema](https://en.wikipedia.org/wiki/Star_schema) or single flat table. 
The de-normalized output may be used:

* As an OLAP cube data source
* To feed an Elasticsearch, SOLR, or Lucene search engine.
* To allow for easier and faster SQL queries.

If you want to follow along, you'll have to:

* be familier with XML and have something to edit it with (e.g. [Visual Studio Code](https://code.visualstudio.com/), or [Notepad++](https://notepad-plus-plus.org/)) 
* have a local instance of SQL Server
* have something to browse a SQLite database file with (e.g. [DB Browser for SQLite](http://sqlitebrowser.org))
* have the latest release of Tranformalize (on [GitHub](https://github.com/dalenewman/Transformalize/releases))
  * add where you downloaded it to your [PATH](https://en.wikipedia.org/wiki/PATH_(variable))
</pre>
* download and install the [NorthWind](http://www.microsoft.com/en-us/download/details.aspx?id=23654) database

### Getting Started

First, we should take a glance at the Northwind schema (partial).

### The NorthWind Schema

<img src="http://www.codeproject.com/KB/database/658971/NorthWindOrderDetails.png" class="img-responsive img-thumbnail" alt="Northwind Schema" />

The diagram shows eight [normalized](https://en.wikipedia.org/wiki/Database_normalization) tables. The 
most important [fact table](https://en.wikipedia.org/wiki/Fact_table) is 
*Order Details*.  It is related to everything and stores the sales.

### Order Details

Let's start by writing an arrangment (aka configuration) that defines 
the *input* as Northwind's `Order Details` table.  Open your editor and 
paste this in:

```xml
<cfg name="NorthWind">
  <connections>
    <add name="input" provider="sqlserver" database="NorthWind" />
  </connections>
  <entities>
    <add name="Order Details" />
  </entities>
</cfg>
```

The root element is `<cfg/>` and it requires a `name`.  Within `<cfg/>`, 
I've added `<connections/>` and `<entities/>` sections.

#### Connections

In my "input" connection, I set the `provider` to "sqlserver" and 
the `database` to "NorthWind."  A `server` setting is unnecessary 
because it defaults to "localhost."  Credentials (`user` and `password`) are also 
unnecessary as I am relying on Windows' trusted security.

#### Entities

I set the entity's `name` to "Order Details" which matches the name of the fact table 
I'm interested in.  By default, it's `connection` is "input."

---

Save your arrangement as *NorthWind.xml*.  It has enough information in it for 
`tfl` to read the `Order Details` from the Northwind database.  Use the CLI 
`tfl.exe` to run it like this:

<pre>
<strong>> tfl -a NorthWind.xml</strong>
OrderID,ProductID,UnitPrice,Quantity,Discount
10248,11,14.0000,12,0
10248,42,9.8000,10,0
10248,72,34.8000,5,0
10249,14,18.6000,9,0
10249,51,42.4000,40,0
...
</pre>


Transformalize detected the schema automatically.  This is handy, but 
if you want to transform a field, or create a new field, 
you must define the fields.  You could hand-write them, 
or run `tfl` in `check` mode like this:

<pre><code>
> tfl -a NorthWind.xml <strong>-m check</strong>
...
&lt;fields&gt;
  &lt;add name="OrderID" type="int" primarykey="true" /&gt;
  &lt;add name="ProductID" type="int" primarykey="true" /&gt;
  &lt;add name="UnitPrice" type="decimal" precision="19" scale="4" /&gt;
  &lt;add name="Quantity" type="short" /&gt;
  &lt;add name="Discount" type="single" /&gt;
&lt;/fields>
...
</code></pre>

Instead of getting order details (the records), `check` mode 
returned the detected schema.  You'll need to add the fields 
to your arrangement like this:

```xml
<cfg name="NorthWind">
  <connections>
    <add name="input" provider="sqlserver" database="NorthWind"/>
  </connections>
  <entities>
    <add name="Order Details">
      <!-- add fields here -->
      <fields>
        <add name="OrderID" type="int" primary-key="true" />
        <add name="ProductID" type="int" primary-key="true" />
        <add name="UnitPrice" type="decimal" precision="19" scale="4" />
        <add name="Quantity" type="short" />
        <add name="Discount" type="single" />
      </fields>
    </add>
  </entities>
</cfg>
```

Now you may create another field based of the fields you've defined.  Let's add 
a calculated field called "ExtendedPrice."  To do this, place a new `<calculated-fields/>` 
section just after the `<fields/>` section and define the field like so:

```xml
<calculated-fields>
  <add name="ExtendedPrice" type="decimal" scale="4" 
       t="cs(UnitPrice*Quantity)" />
</calculated-fields>
```

**Note**: A `calculated-field` is the exact same as a field with it's `input` set to `false`.

Now run `tfl`.  You should see the same data as before 
plus your new field:

<pre>
<strong>> tfl -a NorthWind.xml</strong>
OrderID,ProductID,UnitPrice,Quantity,Discount,<strong>ExtendedPrice</strong>
10248,11,14.0000,12,0,<strong>168.0000</strong>
10248,42,9.8000,10,0,<strong>98.0000</strong>
10248,72,34.8000,5,0,<strong>174.0000</strong>
10249,14,18.6000,9,0,<strong>167.4000</strong>
10249,51,42.4000,40,0,<strong>1696.0000</strong>
...
</pre>

"ExtendedPrice" was created by a C# transformation defined in 
the **`t`** property which is short for *transformation*.  The C# transformation 
is one of [many transformations](https://github.com/dalenewman/Transformalize/blob/master/Pipeline.Ioc.Autofac/Modules/TransformModule.cs) 
injected in to `tfl`.

### Output

Because we have not defined an output, `tfl` has returned everything to the console.  
This isn't useful unless you redirect it somewhere.  Let's send it to a SQLite 
database instead.  To do this, we need to add an **output** connection:

```xml
<connections>
    <add name="input" provider="sqlserver" database="NorthWind"/>
    <!-- add it here -->
    <add name="output" provider="sqlite" file="c:\temp\NorthWind.sqlite3" />
</connections>
```

### Initialization
Now that *Order Details* goes into a persistent output, 
we need to initialize it.  So, run `tfl` in `init` mode like this:

<pre>
> tfl -a NorthWind.xml <strong>-m init</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1044231.
warn  | NorthWind | Order Details | Initializing
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output Order Details
info  | NorthWind | Order Details | Ending 00:00:00.1715532
</pre>

Because the output is not console anymore, you'll see logging.  Initializing does three things:

1. destroys any pre-existing output structures
2. creates output structures
3. bulk inserts data.

Re-initializing **wipes out everything and rebuilds it from 
scratch**.  This is done whenever you change an arrangement.  It is best to think 
of your transformalized output as disposable.

You may have noticed that Transformalize doesn't let you *map* 
your input to a pre-existing output structure.  Instead, it maintains 
control over the output structure.  It does this because it's main 
purpose is to de-normalize multiple inputs into a single output, and perform 
incremental transformed updates on that output.

Here's the control you do get over your output:

* order of the fields
* re-naming fields with an `alias`
* choice to **not** output a field by setting `output` to `false`


### Incrementals

When using persistent outputs, running `tfl` without a mode performs an 
incremental update on the output:

<pre>
<strong>> tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1384721.
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | <strong>2155 from input</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.5755261
</pre>

To determine if an update is necessary, `tfl` reads *all* the input 
and compares it with the output.  If the row is new or different, it will 
be inserted or updated.  While Transformalize does use keys and hashes 
to perform the comparison, it is an unnecessary overhead when your input 
provider has the capability to keep track of and return only new or updated records.

A provider can return new or updated records when it is queryable, and 
each record stores a value that increments every time an insert or update occurs.  
Conveniently enough, SQL Server offers a `ROWVERSION` type that that does 
this automatically.  All you have to do is add one to your table.

So, let's add a `RowVersion` column to `Order Details` like this:

```sql
ALTER TABLE [Order Details] ADD [RowVersion] ROWVERSION;
```

Now let `tfl` know about it by adding the new `RowVersion` 
field to *Order Details* and marking it as the `version` in the entity:

```xml
<entities>
                            <!-- mark it here -->
  <add name="Order Details" version="RowVersion" >
    <fields>
      <add name="OrderID" type="int" primary-key="true" />
      <add name="ProductID" type="int" primary-key="true" />
      <add name="Discount" type="single" />
      <add name="Quantity" type="short" />
      <add name="UnitPrice" type="decimal" precision="19" scale="4"/>

      <!-- define it here -->
      <add name="RowVersion" type="byte[]" length="8" />
    </fields>
  </add>
</entities> 
```

When adding a field to an entity, the output must be re-initialized. So, let's run 
`tfl` in `init` mode once again:

<pre>
<strong>tfl -a NorthWind.xml -m init</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1161231.
warn  | NorthWind | Order Details | Initializing
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | <strong>Change Detected: Input: 0x73bb3 > Output: null</strong>
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output
info  | NorthWind |               | Time elapsed: 00:00:00.8981349
</pre>

Now, to test how many rows are read for an incremental, run `tfl` again:

<pre>
<strong>>tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1064016.
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | <strong>Change Detected: No.</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.3498366
</pre>

With a `version` field in place, the normal run 
doesn't say *"2155 from input"* anymore.  Instead, it says 
*"Change Detected: No"*.  Transformalize used the `version` field 
to avoid reading records that didn't change.  Therefore, our incremental 
is faster, and more efficient!

### Denormalization

So far, we've worked with a single entity.  This is easy, but you 
should know that it is rare to find all the data you need 
in one place.  More likely, the data you seek is spread around.

Even in Northwind (a single database), the data is stored in many different tables. 
It's normalized; which means optimized for storage and integrity, 
rather than retreival.

Look at the output from "Order Details" (above).  You can't gain insight from 
it because it's all keys and numbers.  Knowing keys allows us to look up more 
descriptive information about the entity it refers to.  We need the *Products*
and *Orders* descriptive information at the same level 
as *Order Details* (price, quantity, etc).

### Orders

Refer back to the NorthWind diagram. The next closest tables to 
*Order Details* are *Orders* and *Products*.  This makes sense 
since we have *OrderID8 and *ProductID* in *Order Details." Let's add 
*Orders* to our arrangement.  Here are the steps:

* Alter the table to include the `RowVersion` column. 
This enables efficient incrementals. 
* Add an *Orders* entity just after *Order Details*'s closing `<add/>` 
tag but still inside the `<entities/>` tag.
* Run `tfl` in `check` mode to get the field definitions for *Orders*.  
* Add the fields to your arrangement
* Set the version attribute to *RowVersion* in the *Orders* entity
  
Now your arrangement should have the *Orders* entity like this:

```xml
<add name="Orders" version="RowVersion">
  <fields>
    <add name="OrderID" type="int" primary-key="true" />
    <add name="CustomerID" length="5" />
    <add name="EmployeeID" type="int" />
    <add name="OrderDate" type="datetime" />
    <add name="RequiredDate" type="datetime" />
    <add name="ShippedDate" type="datetime" />
    <add name="ShipVia" type="int" />
    <add name="Freight" type="decimal" precision="19" scale="4" />
    <add name="ShipName" length="40" />
    <add name="ShipAddress" length="60" />
    <add name="ShipCity" length="15" />
    <add name="ShipRegion" length="15" />
    <add name="ShipPostalCode" length="10" />
    <add name="ShipCountry" length="15" />
    <add name="RowVersion" alias="OrdersRowVersion" type="byte[]" length="8" />
  </fields>
</add> 
```

Before we re-initialize, we need to tell Transformalize how to relate 
*Order Details* to *Orders*.

#### Relationships

After the first entity, any other entity you add must be related to 
the first entity via relationships.  It may be related directly, or 
through another entity, but it must be related in order for `tfl` to de-normalize.

Using the diagram as a reference, we need to add our first 
entity relationship like this:

```xml
  <relationships>
    <add left-entity="Order Details" left-field="OrderID" 
         right-entity="Orders" right-field="OrderID"/>
  </relationships>
```

The `<relationships/>` section is added just after your `<entities/>`.  Now 
we can go ahead and re-initialize and run Transformalize:

<pre>
<strong>tfl -a NorthWind.xml -m init</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1272141.
warn  | NorthWind | Order Details | Initializing
warn  | NorthWind | Orders        | Initializing
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | Change Detected: Input: 0x73bb3 > Output: null
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output
info  | NorthWind | Orders        | Starting
info  | NorthWind | Orders        | Change Detected: Input: 0x73bb4 > Output: null
info  | NorthWind | Orders        | 830 from input
info  | NorthWind | Orders        | 830 inserts into output
info  | NorthWind |               | Time elapsed: 00:00:01.0855408

<strong>tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1124897.
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | Change Detected: No.
info  | NorthWind | Orders        | Starting
info  | NorthWind | Orders        | Change Detected: No.
info  | NorthWind |               | Time elapsed: 00:00:00.3670649
</pre>

The logging indicates records were processed from *Order Details* and *Orders*.  
In addition, a view called `NorthWindStar` was created.  *NorthWindStar* is a view that 
pulls together Transformalize's star-schema output so that it appears to be a single entity.

Using a SQLite program, query *NorthWindStar* to make sure Transformalize is working:

```sql
SELECT
    ProductID,
    Discount,
    Quantity,
    UnitPrice,
    CustomerID,
    EmployeeID,
    Freight,
    OrderDate,
    RequiredDate,
    ShipAddress,
    ShipCity,
    ShippedDate,
    ShipPostalCode,
    ShipRegion,
    ShipVia
FROM NorthWindStar
LIMIT 10;
```

<pre>
<strong>ProductId   Discount    Quantity    UnitPrice   CustomerID  EmployeeID  Freight OrderDate   RequiredDate    ShipAddress ...</strong>
<strong>---------   --------    --------    ---------   ----------  ----------  ------- ---------   ------------    -----------</strong>
11	    0.0	        12	    14	        VINET       5           32.38   1996-07-04  1996-08-01      59 rue de l'Abbaye
42	    0.0	        10	    9.8	        VINET       5           32.38   1996-07-04  1996-08-01      59 rue de l'Abbaye
72	    0.0	        5	    34.8        VINET       5           32.38   1996-07-04  1996-08-01      59 rue de l'Abbaye
14	    0.0	        9	    18.6        TOMSP       6           11.61	1996-07-05  1996-08-16      Luisenstr. 48
51	    0.0	        40	    42.4        TOMSP       6           11.61	1996-07-05  1996-08-16      Luisenstr. 48
41	    0.0	        10	    7.7         HANAR       4           65.83	1996-07-08  1996-08-05      Rua do Paço, 67
51	    0.15        35	    42.4        HANAR       4           65.83	1996-07-08  1996-08-05      Rua do Paço, 67
65	    0.15        15	    16.8        HANAR       4           65.83	1996-07-08  1996-08-05      Rua do Paço, 67
22	    0.05        6	    16.8        VICTE       3           41.34	1996-07-08  1996-08-05      2, rue du Commerce
57	    0.05        15	    15.6        VICTE       3           41.34	1996-07-08  1996-08-05      2, rue du Commerce
</pre>

### Star Schema & Single "Flat" Entity

In many cases, data retrieval across multiple entities is made more expensive 
by the number and complexity of connections (aka "joins") between entities.

Data retrieval from a star-schema performs better than a relational model because the entities 
are no more than once removed from center of the star.  That is, the connections 
are less complex. Check out the diagram below:

![Relational to Star](Files/er-to-star.png)

The left side is relational.  Note the red entities are two connections away from the center, and the 
purple entity is three away.  These connections, sometimes called "joins," are more expensive 
the farther they are removed from the center.

The right side is a star-schema.  Every entity is connected to the center.  There is no more 
than one "join" between the center and an entity.  The cost of retrieval is less expensive.

If you want to de-normalize even further, you have the option to `flatten` the output. 
This is done in the main `<cfg/>` element like this:

```xml
<cfg name="NorthWind" flatten="true">
    <!-- commented out for brevity -->
</cfg>
```

After setting `flatten` to true, re-initialize.  An additional log entry is displayed stating: 
"*2155 records inserted into flat*."  Flattening creates a single output structure named 
*NorthWindFlat*.  You may query it just as you query *NorthWindStar*.

Flattening demands more overhead to create than a star-schema, but the output is more optimized 
for retrieval.  This is true because a flat table query has no joins, whereas a star-schema has 
one per relation.

### More Relationships

When we queried *NorthWindStar* above, you may have noticed that more key fields came in with 
the *Orders* fields.  The keys included *CustomerID*, *EmployeeID*, and *ShipVia*.  These relate to 
the entities *Customers*, *Employees*, and *Shippers* in the Northwind database.

To incorporate all the entities from NorthWind database I diagramed above, we need to follow the 
same process outlined for adding *Orders* to *Products*, *Customers*, *Employees*, *Shippers*, *Suppliers*, and *Categories*.

In the end, our relationships should look like this:

```xml
<relationships>
    <add left-entity="Order Details" left-field="OrderID" right-entity="Orders" right-field="OrderID" />
    <add left-entity="Order Details" left-field="ProductID" right-entity="Products" right-field="ProductID" />
    <add left-entity="Orders" left-field="CustomerID" right-entity="Customers" right-field="CustomerID" />
    <add left-entity="Orders" left-field="EmployeeID" right-entity="Employees" right-field="EmployeeID" />
    <add left-entity="Orders" left-field="ShipVia" right-entity="Shippers" right-field="ShipperID" />
    <add left-entity="Products" left-field="SupplierID" right-entity="Suppliers" right-field="SupplierID" />
    <add left-entity="Products" left-field="CategoryID" right-entity="Categories" right-field="CategoryID" />
</relationships>
```

