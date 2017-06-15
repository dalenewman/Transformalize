# Transformalize

Transformalize is an [open source](https://github.com/dalenewman/Transformalize) 
extract, transform, and load ([ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load)) tool. 
It expedites mundane data processing tasks like cleaning, reporting, 
and [denormalization](https://en.wikipedia.org/wiki/Denormalization).

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
in the included [Orchard CMS](http://www.orchardproject.net/) module.

---

### Transformalizing Northwind

This document demonstrates the transformation (and denormalization) 
of Northwind's relational tables into a star-schema or single flat table. 
The de-normalized output may be used:

* As an OLAP cube data source
* To feed an Elasticsearch, SOLR, or Lucene search engine.
* To allow for easier and faster SQL queries.

If you want to follow along, you'll have to:

* have something to edit XML with (e.g. [Visual Studio Code](https://code.visualstudio.com/), or [Notepad++](https://notepad-plus-plus.org/)) 
* have a local instance of SQL Server
* have something to browse a SQLite database file with (e.g. [DB Browser for SQLite](http://sqlitebrowser.org))
* have the latest release of Tranformalize (on [GitHub](https://github.com/dalenewman/Transformalize/releases))
* download and install the [NorthWind](http://www.microsoft.com/en-us/download/details.aspx?id=23654) database

### Getting Started

First, we should take a glance at the Northwind schema (partial).

### The NorthWind Schema

<img src="http://www.codeproject.com/KB/database/658971/NorthWindOrderDetails.png" class="img-responsive img-thumbnail" alt="Northwind Schema" />

It shows eight tables.  The most important [fact table](https://en.wikipedia.org/wiki/Fact_table) 
is *Order Details*.  It is related to all of the 
other entities and contains the money.

### Order Details

So, let's start by writing an arrangment 
(aka configuration) that defines the *input* as Northwind's `Order Details` 
table:

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

Within the main `<cfg/>` section, I've added `<connections/>` and 
`<entities/>`.  By default, an entity assumes the **input** connection. 
This is enough for **`tfl`** to read the `Order Details` from 
the Northwind database:

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


**Note**: `tfl` detected the schema automatically.  This is handy, but 
if you ever want to apply transformations, or create a new field, 
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
      <!-- fields are added here -->
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

Now `tfl` doesn't have to detect the schema anymore, and you can create 
another field based of the fields you've defined.  Let's add a calculated 
field called "ExtendedPrice."  To do this, place a new `<calculated-fields/>` 
section just after the `<fields/>` section and define the field like so:

```xml
<calculated-fields>
  <add name="ExtendedPrice" type="decimal" scale="4" t="cs(UnitPrice*Quantity)" />
</calculated-fields>
```

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
built-in to `tfl`.

### Output

Up until now, `tfl` has returned everything to the console.  This isn't 
very useful unless you redirect it somewhere.  Let's send it to a SQLite 
database instead.  To do this, we need to add an **output** connection:

```xml
<connections>
    <add name="input" provider="sqlserver" database="NorthWind"/>
    <!-- add it here in the connections -->
    <add name="output" provider="sqlite" file="c:\temp\NorthWind.sqlite3" />
</connections>
```

### Initialization
Now that we intend to move *Order Details* into a persistent output, 
we need to initialize the output.  So, run `tfl` in `init` mode:

<pre>
> tfl -a NorthWind.xml <strong>-m init</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1044231.
warn  | NorthWind | Order Details | Initializing
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output Order Details
info  | NorthWind | Order Details | Ending 00:00:00.1715532
</pre>

Init mode creates the necessary structures for storage.  Then, 
it bulk inserts the data.  It's important to note that this is 
different from a general purpose ETL solution where you could 
*map* your input to a pre-existing output.  `tfl` maintains 
control of the output in order de-normalize it.

**Note**: Re-initializing with `tfl` will completely destroy 
and rebuild the output.  This is done whenever you change 
the arrangement.

### Incrementals

By default; when using persistent outputs, running `tfl` without 
a mode performs an incremental update:

<pre>
<strong>> tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1384721.
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | <strong>2155 from input</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.5755261
</pre>

To determine if an update is necessary, `tfl` reads *all* the input 
and compares it with the output.  If the row is new or different, it will 
be inserted or updated.  This is inefficient when your input 
provider has the ability to keep track of and return only the 
new or updated records.

A better approach is to only read new or updated records.  This 
is possible if each row stores a value that increments 
every time an insert or update occurs.  Conveniently 
enough, SQL Server offers a `ROWVERSION` type that 
automatically does this.

So, let's add a `RowVersion` column to `Order Details` like this:

```sql
ALTER TABLE [Order Details] ADD [RowVersion] ROWVERSION;
```

Now you have to let `tfl` know about it. Add the new `RowVersion` 
field to *Order Details* and mark it as the `version` field:

```xml
<entities>
                            <!----- here ------>
  <add name="Order Details" version="RowVersion" >
    <fields>
      <add name="OrderID" type="int" primary-key="true" />
      <add name="ProductID" type="int" primary-key="true" />
      <add name="Discount" type="single" />
      <add name="Quantity" type="short" />
      <add name="UnitPrice" type="decimal" precision="19" scale="4"/>

      <!---------- and define the field here ---------->
      <add name="RowVersion" type="byte[]" length="8" />
    </fields>
  </add>
</entities> 
```

When adding a field to an entity, the output 
must be re-initialized.  So, please re-initialize:

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

Now, to test how many rows are read, run `tfl` in default mode:

<pre>
<strong>>tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1064016.
info  | NorthWind | Order Details | Starting
info  | NorthWind | Order Details | <strong>Change Detected: No.</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.3498366
</pre>

With a `version` field in place, the normal run 
doesn't say *"2155 from input"* anymore, instead it says 
*"Change Detected: No"*.  Transformalize didn't have 
to read and compare *any* records.  Now our incremental 
is faster, and more efficient.

### Denormalization

So far, we've worked with a single entity.  Singles are easy.  However, 
it is rare that you will find all the data you need in one place.  More 
often than not, the data you seek is spread out everywhere. 
It needs to be joined with other data before it makes sense. 
Even in Northwind, the data is stored in many different tables. 
It's normalized; which means it is optimized for storage, not retreival. 

They say that building a data warehouse is 80-90% ETL.  This means that 
most of the work goes into collecting, cleaning, combining, transforming, 
and finally loading the data into the actual data warehouse.

Transformalize tackles this 80-90%.  It helps us transform 
and de-normalize the data incrementally, so that it's optimized 
for retreival.

### Orders

Review the NorthWind diagram. The next closest tables to 
*Order Details* are *Orders* and *Products*. Let's do *Orders* next.  

* Alter the table to include the `RowVersion` column. 
This enables efficient incrementals. 
* Add an *Orders* entity just after *Order Details*'s closing `<add/>` 
tag but still inside the `<entities/>` tag.
* Run `tfl` in `check` mode to get the field definitions for *Orders*.  
* Add the fields
* Set the version attribute to *RowVersion*
  
Now your arrangement should have this entity:

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

Since we added another entity, we have to re-initialize:

<pre>
<strong>tfl -a NorthWind.xml -m init</strong>
error | Process |  You have 2 entities so you need 1 relationships. You have 0 relationships.
error | Process |  The configuration errors must be fixed before this job will run.
</pre>

Bad news.  The configuration is invalid.  **`tfl`** reports 
errors instead of running.

#### Relationships

The error says we need a relationship between the entities to continue. In 
fact, every time you add an entity after the first one, it  
must be related to the first entity via relationships. 
It may be related directly, or through another entity, but it must be 
related in order for `tfl` to de-normalize.

In this case, we need to say how the records from *Order Details* 
and *Orders* relate.  What field in *Order Details* is used 
to lookup more information about *Orders*?  Well, it's the 
`OrderID` field.  Here's how it looks in your arrangement:

```xml
  <relationships>
    <add left-entity="Order Details" left-field="OrderID" right-entity="Orders" right-field="OrderID"/>
  </relationships>
```

The `<relationships/>` section should be added just after your `<entities/>`.  Since 
we changed our arrangement, we have to re-initialize and run.

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

Notice the logging indicates 830 records were processed from the `Orders` table.  
In addition to copying the data from `Order Details` and `Orders` to 
the output, `tfl` created a view called `NorthWindStar`.  Take a 
look to make sure it's working:

```sql
SELECT
	Discount AS Disc,
	OrderID,
	ProductID AS PId,
	Quantity AS Qty,
	UnitPrice,
	CustomerID AS CustId,
	EmployeeID AS EId,
	Freight,
	OrderDate,
	RequiredDate,
	ShipAddress,
	ShipCity,
	ShippedDate,
	ShipPostalCode,
	ShipRegion,
	ShipVia AS SId
FROM NorthWindStar
LIMIT 10;
```

<pre>
<strong>Disc OrderID PId Qty UnitPrice  CustId EId Freight  OrderDate  RequiredDate ShipAddress            ShipCity        ShippedDate ShipPostalCode ShipRegion Sid
---- ------- --- --- ---------  ------ --- -------- ---------- ------------ ---------------------- --------------- ----------- -------------- ---------- ---</strong>
0.2  10248   11  12  14.0000    VINET  5   32.3800  1996-07-04 1996-08-01   59 rue de l&#39;Abbaye     Reims           1996-07-16  51100                     3
0    10248   42  10  9.8000     VINET  5   32.3800  1996-07-04 1996-08-01   59 rue de l&#39;Abbaye     Reims           1996-07-16  51100                     3
0    10248   72  5   34.8000    VINET  5   32.3800  1996-07-04 1996-08-01   59 rue de l&#39;Abbaye     Reims           1996-07-16  51100                     3
0    10249   14  9   18.6000    TOMSP  6   11.6100  1996-07-05 1996-08-16   Luisenstr. 48          M&uuml;nster         1996-07-10  44087                     1
0    10249   51  40  42.4000    TOMSP  6   11.6100  1996-07-05 1996-08-16   Luisenstr. 48          M&uuml;nster         1996-07-10  44087                     1
0    10250   41  10  7.7000     HANAR  4   65.8300  1996-07-08 1996-08-05   Rua do Pa&ccedil;o, 67        Rio de Janeiro  1996-07-12  05454-876      RJ         2
0.15 10250   51  35  42.4000    HANAR  4   65.8300  1996-07-08 1996-08-05   Rua do Pa&ccedil;o, 67        Rio de Janeiro  1996-07-12  05454-876      RJ         2
0.15 10250   65  15  16.8000    HANAR  4   65.8300  1996-07-08 1996-08-05   Rua do Pa&ccedil;o, 67        Rio de Janeiro  1996-07-12  05454-876      RJ         2
0.05 10251   22  6   16.8000    VICTE  3   41.3400  1996-07-08 1996-08-05   2, rue du Commerce     Lyon            1996-07-15  69004                     1
0.05 10251   57  15  15.6000    VICTE  3   41.3400  1996-07-08 1996-08-05   2, rue du Commerce     Lyon            1996-07-15  69004                     1
</pre>