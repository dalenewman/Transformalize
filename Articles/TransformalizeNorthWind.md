# Transformalize Northwind

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

Now, rinse and repeat. That is, consult the NorthWind diagram and continue adding related entities until the relationships configuration look like this:

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

As you might expect, adding all these entities creates many 
duplicate field names. Instead of aliasing each one, 
we can add a prefix to the entity. A prefix aliases all the 
fields as prefix + name.

```xml
<!-- set prefix attribute to "Employee" --> 
<add name="Employees" version="RowVersion" prefix="Employee">
  <fields>
    <!-- cut for brevity -->
  </fields>
</add>
```

Initialize, and run. Console output should look like this:

<pre class="prettyprint linenums">
tfl NorthWind.xml {&#39;mode&#39;:&#39;init&#39;}
19:41:53 | Info | NorthWind | All | Initialized TrAnSfOrMaLiZeR.
19:41:53 | Info | NorthWind | All | Initialized NorthWindOrderDetails in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindOrders in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindProducts in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindCustomers in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindEmployees in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindShippers in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindSuppliers in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Initialized NorthWindCategories in NorthWindOutput on localhost.
19:41:53 | Info | NorthWind | All | Process completed in 00:00:01.1828232.
tfl NorthWind.xml
19:42:06 | Info | NorthWind | Order Details....... | Processed 2155 inserts, and 0 updates in Order Details.
19:42:07 | Info | NorthWind | Orders.............. | Processed 830 inserts, and 0 updates in Orders.
19:42:07 | Info | NorthWind | Products............ | Processed 77 inserts, and 0 updates in Products.
19:42:07 | Info | NorthWind | Customers........... | Processed 91 inserts, and 0 updates in Customers.
19:42:07 | Info | NorthWind | Employees........... | Processed 9 inserts, and 0 updates in Employees.
19:42:07 | Info | NorthWind | Shippers............ | Processed 3 inserts, and 0 updates in Shippers.
19:42:07 | Info | NorthWind | Suppliers........... | Processed 29 inserts, and 0 updates in Suppliers.
19:42:07 | Info | NorthWind | Categories.......... | Processed 8 inserts, and 0 updates in Categories.
19:42:07 | Info | NorthWind | Orders.............. | Processed 2155 rows. Updated Order Details with Orders.
19:42:07 | Info | NorthWind | Products............ | Processed 2155 rows. Updated Order Details with Products.
19:42:07 | Info | NorthWind | All................. | Process completed in 00:00:01.2583563.
tfl NorthWind.xml
19:42:13 | Info | NorthWind | Order Details....... | Processed 0 inserts, and 0 updates in Order Details.
19:42:13 | Info | NorthWind | Orders.............. | Processed 0 inserts, and 0 updates in Orders.
19:42:13 | Info | NorthWind | Products............ | Processed 0 inserts, and 0 updates in Products.
19:42:13 | Info | NorthWind | Customers........... | Processed 0 inserts, and 0 updates in Customers.
19:42:13 | Info | NorthWind | Employees........... | Processed 0 inserts, and 0 updates in Employees.
19:42:13 | Info | NorthWind | Shippers............ | Processed 0 inserts, and 0 updates in Shippers.
19:42:13 | Info | NorthWind | Suppliers........... | Processed 0 inserts, and 0 updates in Suppliers.
19:42:13 | Info | NorthWind | Categories.......... | Processed 0 inserts, and 0 updates in Categories.
19:42:13 | Info | NorthWind | All................. | Process completed in 00:00:00.7708553.
</pre>

Now there are 81 fields available in the output `NorthWindStar`:

<pre class="prettyprint linenums lang-sql">
SELECT COUNT(*) AS FieldCount
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = &#39;NorthWindStar&#39;
</pre>
<pre class="prettyprint linenums">
FieldCount
----------
81
</pre>

<img src="http://www.codeproject.com/KB/database/658971/NorthWindOrderDetailsStar.png" class="img-responsive img-thumbnail" alt="Northwind Star Schema" />

As you can see in the diagram above, I haven&#39;t completely de-normalized the data. &nbsp;Instead, I have created a [star schema](http://en.wikipedia.org/wiki/Star_schema), where every related table has a single join to the master table. &nbsp;In addition, I have created a view (e.g. NorthWind**Star**) so users may query it as if it we&#39;re a single table.&nbsp; Having this single view of the duplicated data allows us to quickly create simple cubes or integrate with search engines:&nbsp;

#### Leveraging SQL Server Analysis Services

Open up [BIDS](http://technet.microsoft.com/en-us/library/ms173767%28v=sql.105%29.aspx), and create an Analysis Services Project (or cube) to browse the data.

* Set the data source to your NorthWindOutput database&nbsp;
* Set the data view to the NorthWindStar view
* Create a dimension using all the fields (except the binary ones) in the view. Call it Properties.
* Create a cube with a couple measures (e.g. UnitPrice, Quantity) and connect it to the Properties dimension.
* Process the cube and browse it. Note: You may have to grant NT AUTHORITY\LOCAL SERVICE read writes to the NorthWindOutput database.

<img src="http://www.codeproject.com/KB/database/658971/NorthWindBrowse1.png" class="img-responsive img-thumbnail" alt="Business Intelligence Development Studio" />

As you can see, slicing the measures by order date isn&#39;t ideal. Moreover, the unit price and quantity measures don&#39;t help much by themselves. This cube needs a time hierarchy and revenue calculation. We can add them with Transformalize. First, add three calculated fields based on &quot;order date&quot; to create a time hierarchy:

<pre class="prettyprint linenums">
&lt;add name=&quot;Orders&quot; version=&quot;RowVersion&quot; prefix=&quot;Orders&quot;&gt;
    &lt;fields&gt;
      &lt;!-- ... --&gt;
    &lt;/fields&gt;
    &lt;calculated-fields&gt;
      &lt;add name=&quot;TimeDate&quot; length=&quot;10&quot; default=&quot;9999-12-31&quot;&gt;
        &lt;transforms&gt;
            &lt;add method=&quot;toString&quot; format=&quot;yyyy-MM-dd&quot; parameter=&quot;OrderDate&quot; /&gt;
        &lt;/transforms&gt;
      &lt;/add&gt;
      &lt;add name=&quot;TimeMonth&quot; length=&quot;6&quot; default=&quot;12-DEC&quot;&gt;
        &lt;transforms&gt;
            &lt;add method=&quot;toString&quot; format=&quot;MM-MMM&quot; parameter=&quot;OrderDate&quot; /&gt;
            &lt;add method=&quot;toUpper&quot; /&gt;
        &lt;/transforms&gt;
      &lt;/add&gt;
      &lt;add name=&quot;TimeYear&quot; type=&quot;System.Int16&quot; default=&quot;9999&quot;&gt;
        &lt;transforms&gt;
            &lt;add method=&quot;toString&quot; format=&quot;yyyy&quot; parameter=&quot;OrderDate&quot; /&gt;
		&lt;/transforms&gt;
      &lt;/add&gt;
    &lt;/calculated-fields&gt;
&lt;/add&gt;
</pre>

Calculated fields project new fields based on the values of other fields and previously defined other calculated fields. They are used at the entity level, or at the process level. In an entity, they have access to any field within their entity. In a process, they have access to all of the data. To control which fields they have access to, use parameters like this:

<pre class="prettyprint linenums">
&lt;transform method=&quot;format&quot; format=&quot;{0} is a big city!&quot;&gt;
    &lt;parameters&gt;
        &lt;add field=&quot;City&quot; /&gt;
    &lt;/parameters&gt;
&lt;/transform&gt;
</pre>

You may add multiple parameters in this way. &nbsp;However, if you only have a single parameter, you can specify it in the parameter attribute in the transform element itself, like this:

<pre class="prettyprint linenums">
&lt;transform method=&quot;format&quot; format=&quot;{0} is a big city!&quot; parameter=&quot;City&quot; /&gt;&nbsp;
</pre>

Another short-cut is to set the parameter attribute to &quot;*&quot; to include all fields.

There are many built-in [Transforms](https://github.com/dalenewman/Transformalize/wiki/Transforms).  If you can&#39;t find one that fits your needs, you can use the C#, JavaScript, or the Razor template transforms to define your own. Let&#39;s use a JavaScript transform to calculate revenue:

<pre class="prettyprint linenums">
&lt;calculated-fields&gt;
    &lt;add name=&quot;Revenue&quot; type=&quot;System.Decimal&quot; &gt;
        &lt;transforms&gt;
            &lt;add method=&quot;javascript&quot; script=&quot;(UnitPrice * (1 - Discount)) * Quantity&quot; parameter=&quot;*&quot; /&gt;
        &lt;/transforms&gt;
    &lt;/add&gt;
&lt;/calculated-fields&gt;
</pre>

Re-initialize and run Tfl. Then, using the new time fields and revenue, see if it improves the cube browsing experience.

<img src="http://www.codeproject.com/KB/database/658971/NorthWindBrowse2.png" class="img-responsive img-thumbnail" alt="Business Intelligence Development Studio (after)" />

The cube looks better now, but we&#39;ll need it to update whenever Transformalize runs. &nbsp;So, &nbsp;add a connection to Analysis Services and a corresponding template action:&nbsp;

<pre class="prettyprint linenums">
&lt;connections&gt;
    &lt;add name=&quot;input&quot; connection-string=&quot;server=localhost;Database=NorthWind;Trusted_Connection=True;&quot;/&gt;
    &lt;add name=&quot;output&quot; connection-string=&quot;Server=localhost;Database=NorthWindOutput;Trusted_Connection=True;&quot;/&gt;
    &lt;add name=&quot;cube&quot; connection-string=&quot;Data Source=localhost;Catalog=NorthWind;&quot; provider=&quot;AnalysisServices&quot;/&gt;
&lt;/connections&gt;
&lt;!-- ... --&gt;
&lt;templates path=&quot;C:\Tfl\&quot;&gt;
    &lt;add name=&quot;process-cube&quot; file=&quot;process-cube.xmla&quot;&gt;
        &lt;settings&gt;
            &lt;add name=&quot;DatabaseID&quot; value=&quot;NorthWind2&quot;&gt;&lt;/add&gt;
        &lt;/settings&gt;    
        &lt;actions&gt;
            &lt;add action=&quot;run&quot; connection=&quot;cube&quot;&gt;&lt;/add&gt;
        &lt;/actions&gt;
    &lt;/add&gt;
&lt;/templates&gt;**
</pre>

Transformalize &quot;templates&quot; use [C# Razor syntax](http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx). Settings are passed into the template and used like this:

<pre class="prettyprint linenums">
&lt;Batch xmlns=&quot;http://schemas.microsoft.com/analysisservices/2003/engine&quot;&gt;
  &lt;Process xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:ddl2=&quot;http://schemas.microsoft.com/analysisservices/2003/engine/2&quot; xmlns:ddl2_2=&quot;http://schemas.microsoft.com/analysisservices/2003/engine/2/2&quot; xmlns:ddl100_100=&quot;http://schemas.microsoft.com/analysisservices/2008/engine/100/100&quot; xmlns:ddl200=&quot;http://schemas.microsoft.com/analysisservices/2010/engine/200&quot; xmlns:ddl200_200=&quot;http://schemas.microsoft.com/analysisservices/2010/engine/200/200&quot;&gt;
    &lt;Object&gt;
      &lt;DatabaseID&gt;@(Model.Settings.DatabaseID)&lt;/DatabaseID&gt;
    &lt;/Object&gt;
    &lt;Type&gt;ProcessFull&lt;/Type&gt;
    &lt;WriteBackTableCreation&gt;UseExisting&lt;/WriteBackTableCreation&gt;
  &lt;/Process&gt;
&lt;/Batch&gt;
</pre>

The `@(Model.Settings.DatabaseID)` will be replaced with `NorthWind2`.  Transformalize&#39;s template manager will render the template, and subsequently run defined &quot;actions.&quot; The &quot;run&quot; action executes the rendered content against the designated connection. &nbsp;This allows you to dynamically build data manipulation queries, or XMLA commands in this case, and execute them.

<pre class="prettyprint linenums">
tfl NorthWind.xml
00:14:28 | Info | NorthWind | Order Details.. | Processed 2155 inserts, and 0 updates in Order Details.
00:14:28 | Info | NorthWind | Orders......... | Processed 830 inserts, and 0 updates in Orders.
00:14:28 | Info | NorthWind | Products....... | Processed 77 inserts, and 0 updates in Products.
00:14:28 | Info | NorthWind | Customers...... | Processed 91 inserts, and 0 updates in Customers.
00:14:28 | Info | NorthWind | Employees...... | Processed 9 inserts, and 0 updates in Employees.
00:14:28 | Info | NorthWind | Shippers....... | Processed 3 inserts, and 0 updates in Shippers.
00:14:28 | Info | NorthWind | Suppliers...... | Processed 29 inserts, and 0 updates in Suppliers.
00:14:28 | Info | NorthWind | Categories..... | Processed 8 inserts, and 0 updates in Categories.
00:14:28 | Info | NorthWind | Orders......... | Processed 2155 rows. Updated Order Details with Orders.
00:14:29 | Info | NorthWind | Products....... | Processed 2155 rows. Updated Order Details with Products.
00:14:31 | Info | NorthWind | Categories..... | process-cube ran successfully.
00:14:31 | Info | NorthWind | All............ | Process completed in 00:00:03.8312882.
</pre>

#### Leveraging Apache SOLR

With more complex templates, and an Apache [SOLR](http://lucene.apache.org/solr/) server, it is possible to integrate full text search into the process as well. &nbsp;Transformalize comes with a pair of templates that can build the necessary SOLR configuration files for schema, and data import handling.&nbsp;

<pre class="prettyprint linenums">
&lt;templates&gt;
    &lt;add name=&quot;solr-data-handler&quot; file=&quot;solr-data-handler.cshtml&quot; cache=&quot;true&quot;&gt;
      &lt;actions&gt;
        &lt;add action=&quot;copy&quot; file=&quot;C:\Solr\NorthWind\conf\data-config.xml&quot;/&gt;
      &lt;/actions&gt;
    &lt;/add&gt;
    &lt;add name=&quot;solr-schema&quot; file=&quot;solr-schema.cshtml&quot; cache=&quot;true&quot;&gt;
      &lt;actions&gt;
        &lt;add action=&quot;copy&quot; file=&quot;C:\Solr\NorthWind\conf\schema.xml&quot;/&gt;
        &lt;add action=&quot;web&quot; url=&quot;http://localhost:8983/solr/NorthWind/dataimport?command=full-import&amp;amp;clean=true&amp;amp;commit=true&amp;amp;optimize=true&quot;/&gt;
      &lt;/actions&gt;
    &lt;/add&gt;
&lt;/templates&gt;
&lt;search-types&gt;
    &lt;add name=&quot;default&quot; /&gt;
    &lt;add name=&quot;facet&quot; analyzer=&quot;lowercase&quot; store=&quot;true&quot; index=&quot;true&quot; /&gt;
    &lt;add name=&quot;standard&quot; analyzer=&quot;standard_lowercase&quot; store=&quot;false&quot; index=&quot;true&quot;/&gt;
&lt;/search-types&gt;
</pre>

The Razor templates &quot;_solr-data-handler.cshtml_&quot; and &quot;_solr-schema.cshtml_&quot; render the SOLR configuration files. &nbsp;This is possible because the template manager passes the entire NorthWind configuration (that we've built with XML above) into the templates.

To control how the fields are handled in SOLR, &quot;search types&quot; are applied to each `&lt;field/&gt;` or `&lt;calculated-field/&gt;`. By default, each field is indexed and stored in the search index according to it&#39;s data type. To assign more complex text analysis, you can set the search-type attribute to facet, or standard, or any others you define. To exclude a field from search, set search-type to &quot;none.&quot;

Running Tfl now produces:

<pre class="prettyprint">
    tfl NorthWind.xml
    ...
    00:48:25 | Info | NorthWind | Products.... | Processed 2155 rows. Updated Order Details with Products.
    00:48:28 | Info | NorthWind | Categories.. | process-cube ran successfully.
    00:48:28 | Info | NorthWind | Categories.. | Copied solr-data-handler template output to C:\Solr\NorthWind\conf\data-config.xml.
    00:48:29 | Info | NorthWind | Categories.. | Copied solr-schema template output to C:\Solr\NorthWind\conf\schema.xml.
    00:48:29 | Info | NorthWind | Categories.. | Made web request to http://localhost:8983/solr/NorthWind/dataimport?command=full-import&amp;clean=true&amp;commit=true&amp;optimize=true.
    00:48:29 | Info | NorthWind | Categories.. | Process completed in 00:00:04.8287386.
</pre>

In this example, the template action &quot;web&quot; triggers SOLR to clean and re-import the index. In a production environment, you&#39;d want to reload the schema when it changes, and make use of full and delta imports appropriately. If all goes well, you see something like this in the SOLR admin:

<img src="http://www.codeproject.com/KB/database/658971/NorthWindSolrDataImport.png" class="img-responsive img-thumbnail" alt="SOLR Admin" />

Now, if you schedule Transformalize to run every couple minutes, you have near real-time OLAP and search engine services on top of your OLTP data. &nbsp;An OLAP cube supports more performant and complex reporting requirements, and a search engine allows for lightning fast and _fuzzy_&nbsp;searches for specific records. &nbsp;If your users want to see the data in different ways, and they will, all you have to do is add transforms and/or new calculated fields and re-initialize your output. &nbsp;

When Transformalize reads your production databases, it attempts to do so introducing as little contention as possible.  Using version fields, it can keep a star-schema copy of very large databases up to date very quickly.

### Summary

The NorthWind data is fairly clean. In reality, you&#39;ll face more challenging data sources.

---

Transformalize uses several other open source projects including

1. [Rhino ETL](https://github.com/hibernating-rhinos/rhino-etl)
1. [Razor Engine](https://github.com/Antaris/RazorEngine)
1. [Jint](https://github.com/sebastienros/jint)
1. [Ninject](http://www.ninject.org/)
1. [fastJSON](http://www.codeproject.com/Articles/159450/fastJSON)
1. [Newtonsoft.JSON](https://github.com/JamesNK/Newtonsoft.Json)
1. [Dapper-dot-net](https://github.com/SamSaffron/dapper-dot-net)
1. [File Helpers](http://filehelpers.sourceforge.net/)
1. [Excel Data Reader](http://exceldatareader.codeplex.com/)
1. [Enterprise Library 6 Validation & Semantic Logging Blocks](http://msdn.microsoft.com/library/cc467894.aspx "Enterprise Library Home Page")
1. [Lucene.NET](http://lucenenet.apache.org/)
1. [Elasticsearch.NET & NEST](https://github.com/elasticsearch/elasticsearch-net)
1. [SolrNet](https://github.com/mausch/SolrNet)

Where possible, I've included source code from these projects rather than the Nuget packages. The upside &nbsp;of doing this is I get to step into and learn from other people&#39;s code. The downside is it&#39;s a bit harder to keep these libraries up to date.

<script src="http://cdnjs.cloudflare.com/ajax/libs/prettify/r298/run_prettify.js" type="text/javascript"></script>