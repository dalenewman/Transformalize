# Transformalize

Transformalize automates moving data into data warehouses, search engines, and other *value-adding* systems.

It works with many data sources:

<div class="table-responsive">
<table class="table table-condensed">
    <thead>
        <tr>
            <th>Relational</th>
            <th>Non-Relational</th>
            <th>Other</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="vertical-align:top">
                <table class="table table-condensed">
                    <thead>
                        <tr>
                            <th>Provider</th>
                            <th>Input</th>
                            <th>Output</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.SqlServer">SQL Server</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.MySql">MySql</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.PostgreSql">PostgreSql</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Sqlite">SQLite</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.SqlCe">SqlCe</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Access">Access (32-bit)</a></td>
                            <td style="color:green">BETA</td>
                            <td style="color:green">BETA</td>
                        </tr>
                    </tbody>
                </table>
            </td>
            <td style="vertical-align:top">
                <table class="table table-condensed">
                    <thead>
                        <tr>
                            <th>Provider</th>
                            <th>Input</th>
                            <th>Output</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Elasticsearch">Elasticsearch</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Lucene">Lucene</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Solr">SOLR</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td title="SQL Server Analysis Services"><a href="https://github.com/dalenewman/Transformalize.Provider.Ssas">SSAS</a></td>
                            <td style="color:green"></td>
                            <td style="color:green">BETA</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.RethinkDb">RethinkDB</a></td>
                            <td style="color:green"></td>
                            <td style="color:green">BETA</td>
                        </tr>
                    </tbody>
                </table>
            </td>
            <td style="vertical-align:top">
                <table class="table table-condensed">
                    <thead>
                        <tr>
                            <th>Provider</th>
                            <th>Input</th>
                            <th>Output</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Excel">Excel</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Razor">Razor</a></td>
                            <td style="color:green"></td>
                            <td style="color:green">BETA</td>
                        </tr>
                        <tr>
                            <td>Files</td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td>Web</td>
                            <td style="color:green">&#10003;</td>
                            <td> </td>
                        </tr>
                        <tr>
                            <td>Console</td>
                            <td style="color:green">BETA</td>
                            <td style="color:green">&#10003;</td>
                        </tr>
                        <tr>
                            <td title="Forms in Orchard CMS Module">Humans</td>
                            <td style="color:green">BETA</td>
                            <td style="color:green"></td>
                        </tr>
                        <tr>
                            <td><a href="https://github.com/dalenewman/Transformalize.Provider.Bogus">Bogus</a></td>
                            <td style="color:green">&#10003;</td>
                            <td style="color:green"></td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
</div>

---

### Getting Started

> This section introduces `<connections/>`, `<entities/>`, and the **`tfl.exe`** command line interface.

This introduction to Transformalize uses the Northwind 
relational database. To follow along, you need:

* the [latest release](https://github.com/dalenewman/Transformalize/releases) of Transformalize.
* a SQLite tool (e.g. [DB Browser for SQLite](http://sqlitebrowser.org))
* this [northwind.sqlite](https://github.com/dalenewman/Transformalize/blob/master/Files/northwind.sqlite) database to use as input; found [here](http://www.microsoft.com/en-us/download/details.aspx?id=23654) and converted [here](https://www.rebasedata.com/convert-tsql-to-sqlite-online).
* an editor like [Visual Studio Code](https://code.visualstudio.com/) with the [Transformalize extension](https://marketplace.visualstudio.com/items?itemName=DaleNewman.transformalize).

First, take a look at the diagram below to get see a partial view of the NorthWind schema:

<img src="https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/northwind-diagram.png" class="img-responsive img-thumbnail" alt="Northwind Schema" />

This shows eight [normalized](https://en.wikipedia.org/wiki/Database_normalization) 
tables.  Our main point of interest is *Order Details* because it contains sales and is related to 
everything else.

---

Transformalize jobs are arranged (aka *configured*) in [XML](https://en.wikipedia.org/wiki/XML), [JSON](https://en.wikipedia.org/wiki/JSON), or [C#](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)).  Open your editor and paste this in:

```xml
<cfg name="NorthWind">
  <connections>
    <add name="input" provider="sqlite" file="E:\Code\Transformalize\Files\NorthWindInput.sqlite3" />
  </connections>
  <entities>
    <add name="Order Details" page="1" size="5" />
  </entities>
</cfg>
```

The arrangment above defines the *input* as the *NorthWind.sqlite3* database's `Order Details` table. 
Save it as *NorthWind.xml* and use **`tfl.exe`** to run it:

<pre style="font-size:smaller;">
<strong>> tfl -a NorthWind.xml</strong>
OrderID,ProductID,UnitPrice,Quantity,Discount
10248,11,14.0000,12,0
10248,42,9.8000,10,0
10248,72,34.8000,5,0
10249,14,18.6000,9,0
10249,51,42.4000,40,0
</pre>

Transformalize detected the field names and read 5 rows. 
This is handy, but if you want to modify or create new fields, 
you have to define what fields to extract. You could hand-write 
fields, or run `tfl` in `check` mode like this:

<pre style="font-size:smaller;">
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
</pre>


> * Introducing **`<fields/>`** within **`<entities/>`**
> * Specifying modes with the **`-m`** flag
> * **`check`** mode

Instead of getting order details (the records), `check` mode 
returns the detected fields. Copy them into the arrangement 
like this:

```xml
<cfg name="NorthWind">
  <connections>
    <add name="input" provider="sqlite" file="e:\Code\Transformalize\Files\NorthWindInput.sqlite3" />
  </connections>
  <entities>
    <add name="Order Details">
      <!-- copy/paste the fields here -->
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

> * Introducing The **`<calculated-fields/>`** section within **`<entities/>`**
> * The **`t`** attribute (short for **t**ransformation)
> * and th **`js`** and **`round`** transformations

Now you may calculate a new field. Place **`<calculated-fields/>`** right after **`<fields/>`** and add *Revenue* like this:

```xml
<calculated-fields>
  <add name="Revenue" 
       type="decimal" 
       t="js(Quantity * ((1-Discount) * UnitPrice)).round(2)" />
</calculated-fields>
```
Now run `tfl`:
<pre style="font-size:smaller;">
<strong>> tfl -a NorthWind.xml</strong>
OrderID,ProductID,UnitPrice,Quantity,Discount,<strong>Revenue</strong>
10248,11,14.0000,12,0,<strong>168</strong>
10248,42,9.8000,10,0,<strong>98</strong>
10248,72,34.8000,5,0,<strong>174</strong>
10249,14,18.6000,9,0,<strong>167.4</strong>
10249,51,42.4000,40,0,<strong>1696</strong>
...
</pre>

*Revenue* is created by the **js** (JavaScript) and **round** [transformations](https://github.com/dalenewman/Transformalize/blob/master/Pipeline.Ioc.Autofac/Modules/TransformModule.cs).  You 
may chain transformations as long as the output of one is compatible with the input of another.

**Note**: Transforms are being moved into plug-ins (e.g. [Jint](https://github.com/dalenewman/Transformalize.Transform.Jint), [C#](https://github.com/dalenewman/Transformalize.Transform.CSharp), [Humanizer](https://github.com/dalenewman/Transformalize.Transform.Humanizer), [LambdaParser](https://github.com/dalenewman/Transformalize.Transform.LambdaParser), etc.).

### Output

> Introducing **`init`** mode

Without defining an output, `tfl` writes to console. To save output, define the output as a [SQLite](https://en.wikipedia.org/wiki/SQLite) database. Add an output in `<connections/>`:

```xml
<connections>
    <add name="input" provider="sqlserver" database="NorthWind"/>
    <!-- add it here -->
    <add name="output" provider="sqlite" file="c:\temp\NorthWind.sqlite3" />
</connections>
```

### Initialization

Initializing does three things:

1. destroys pre-existing output structures
2. creates output structures
3. bulk inserts data.

Initializing is required anytime you create or change an arrangement's output structure.

To initialize, run **`tfl`** in `init` mode using the **`-m`** flag like this:

<pre style="font-size:smaller;">
> tfl -a NorthWind.xml <strong>-m init</strong>
<strong style="color:#FF7F50;">warn  | NorthWind | Order Details | Initializing</strong>
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output Order Details
info  | NorthWind | Order Details | Ending 00:00:00.67
</pre>

Writing *Order Details* into SQLite frees up the console for logging.

#### Mapping

Transformalize doesn't *map* input to pre-existing output.  Instead, it creates a consistent output structure that is optimized for incremental updates.

You decide:

* what new fields to calculate
* the order of fields
* the name of fields (using `alias`)
* the transformation and/or validation of fields
* and the output of field (using `output="true|false"`)

### Incrementals (by Default)

> Introducing the **`version`** attribute for an **`entity`**

An *initialization* is a full rebuild and may be time-consuming. So, by default, Transformalize performs incrementals.  To determine if an update or 
insert is necessary, `tfl` compares input with output.

While keys and hashes are used to compare, comparison is unnecessary when an input's provider is queryable and stores a record version.  A version is a value in a row that increments anytime the row's data is updated.  Many tables have these by design.  If not, SQL Server includes a `ROWVERSION` type that may be added to provide automatic versioning. Add one to `Order Details` like this:

```sql
ALTER TABLE [Order Details] ADD [RowVersion] ROWVERSION;
```

Now let `tfl` know about `RowVersion` like this:

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

      <!-- add (define) it here -->
      <add name="RowVersion" type="byte[]" length="8" />
    </fields>
  </add>
</entities>
```
Adding a field changes output structure, so re-initialize like so:

<pre style="font-size:smaller;">
<strong>tfl -a NorthWind.xml -m init</strong>
warn  | NorthWind | Order Details | Initializing
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output
info  | NorthWind |               | Time elapsed: 00:00:00.70

<strong>>tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1064016.
<strong>info  | NorthWind | Order Details | Change Detected: No.</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.20
</pre>

With a `version` in place, the second run doesn't read and compare 
un-changed data.

### Denormalization

Relational data is [normalized](https://en.wikipedia.org/wiki/Database_normalization) and 
stored in many tables. It's optimized for efficient storage and integrity. It may be 
queried, but not without an overhead of joining busy tables. This makes retrieval slower.

[De-normalization](https://en.wikipedia.org/wiki/Denormalization) is the process of 
joining related data back together. The data is pre-joined (and duplicated) to 
avoid joining tables at run-time. Retrieval of de-normalized data is faster.

The output of *Order Details* (above) is numeric. Some numbers 
are [foreign keys](https://en.wikipedia.org/wiki/Foreign_key) (e.g. `ProductID`, `OrderID`). 
These refer to more descriptive information in related entities. Others are 
[measures](https://en.wikipedia.org/wiki/Measure_(data_warehouse)) (i.e. `Quantity`, `UnitPrice`).

To denormalize *Order Details*, we need to use the foreign keys `OrderID` and `ProductID` to 
retrieve the related information from *Orders* and *Products* (see diagram).  This means we have 
to add the *Orders* and *Products* entities to our arrangement.

### Adding an Entity

Here is the process for adding entities:

1. Identify or add a version field to the source if possible (optional)
1. Add the entity to the `<entities/>` section.
1. Run `tfl` in `check` mode to get field definitions (optional).  
1. Add the fields to your new entity (in the arrangement)
1. Set the version attribute on the entity (optional)
1. Relate the new entity to the first entity
  
Follow the first 5 steps to add *Orders* to the arrangement. When finished, 
the arrangement should have a new entity like this:

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
Next, tell Transformalize how to relate *Order Details* to *Orders*.

#### Relationships
<span style="background-color:#777777">

> Introducing the **`<relationships/>`** section

</span>

All entities must be related to the first entity in the `<relationships/>` section which 
follows `<entities/>`.  To relate *Orders* to *Order Details*, add this to your arrangement:

```xml
  <relationships>
    <add left-entity="Order Details" 
         left-field="OrderID" 
         right-entity="Orders" 
         right-field="OrderID"/>
  </relationships>
```

This tells Transformalize to use `OrderID` to relate the two entities. Now re-initialize 
and run Transformalize:

<pre style="font-size:smaller;">
<strong>tfl -a NorthWind.xml -m init</strong>
warn  | NorthWind | Order Details | Initializing
warn  | NorthWind | Orders        | Initializing
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output
<strong>info  | NorthWind | Orders        | 830 from input
info  | NorthWind | Orders        | 830 inserts into output</strong>
info  | NorthWind |               | Time elapsed: 00:00:01.02

<strong>tfl -a NorthWind.xml</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1124897.
info  | NorthWind | Order Details | Change Detected: No.
<strong>info  | NorthWind | Orders        | Change Detected: No.</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.25
</pre>

Logging indicates records were processed from *Order Details* and *Orders*. In addition, 
a view called `NorthWindStar` is created.  *NorthWindStar* joins Transformalize's 
[star-schema](https://en.wikipedia.org/wiki/Star_schema) output so that it appears to be a 
single entity.

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

<pre style="font-size:smaller;">
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

> Introducing the **`flatten`** attribute


Transformalize de-normalizes in two phases.  First, it moves data 
from a relational model into a [star-schema](https://en.wikipedia.org/wiki/Star_schema).
Secondly, it moves data into a completely de-normalized (flat) output. 

![Relational to Star](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/transformalize-diagram.jpg)

To create a star-schema, it moves the foreign keys to the center.  Data retrieval is 
faster because everything is directly related.

To create a flat output, it moves *everything* to the center.  Data retrieval is 
even faster because there aren't any relations.

To completely de-normalize, set `flatten` to `true` 
in the main `<cfg/>` like this:

```xml
<cfg name="NorthWind" flatten="true">
    <!-- commented out for brevity -->
</cfg>
```

When you re-initialize, a single output structure named *NorthWindFlat* is created and populated. 
You may query it just as you queried *NorthWindStar*.

### More Relationships

To add all the entities from NorthWind database (diagramed above), follow the *Add an Entity* 
process (above) for *Products*, *Customers*, *Employees*, *Shippers*, *Suppliers*, and *Categories*.

In the end, the relationships should look like this:

```xml
<relationships>
  <!-- following Orders to Customers, Employees, and Shippers -->
  <add left-entity="Order Details" left-field="OrderID" right-entity="Orders" right-field="OrderID" />
  <add left-entity="Orders" left-field="CustomerID" right-entity="Customers" right-field="CustomerID" />
  <add left-entity="Orders" left-field="EmployeeID" right-entity="Employees" right-field="EmployeeID" />
  <add left-entity="Orders" left-field="ShipVia" right-entity="Shippers" right-field="ShipperID" />

  <!-- following Products to Suppliers and Categories -->
  <add left-entity="Order Details" left-field="ProductID" right-entity="Products" right-field="ProductID" />
  <add left-entity="Products" left-field="SupplierID" right-entity="Suppliers" right-field="SupplierID" />
  <add left-entity="Products" left-field="CategoryID" right-entity="Categories" right-field="CategoryID" />
</relationships>
```

If you'd rather not do all that work, you can use this pre-created [arrangement](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/NorthWindEntitiesRelated.xml).

Now when you initialize and run Transformalize, there's a lot going on:

<pre style="font-size:smaller;">
<strong>>tfl -a "c:\Temp\NorthWind.xml" -m init</strong>
<span style="color:#FF7F50;">warn  | NorthWind | Order Details | Initializing
warn  | NorthWind | Orders        | Initializing
warn  | NorthWind | Products      | Initializing
warn  | NorthWind | Customers     | Initializing
warn  | NorthWind | Employees     | Initializing
warn  | NorthWind | Shippers      | Initializing
warn  | NorthWind | Suppliers     | Initializing
warn  | NorthWind | Categories    | Initializing</span>
info  | NorthWind | Order Details | 2155 from input
info  | NorthWind | Order Details | 2155 inserts into output
info  | NorthWind | Orders        | 830 from input
info  | NorthWind | Orders        | 830 inserts into output
info  | NorthWind | Products      | 77 from input
info  | NorthWind | Products      | 77 inserts into output
info  | NorthWind | Customers     | 91 from input
info  | NorthWind | Customers     | 91 inserts into output
info  | NorthWind | Employees     | 9 from input
info  | NorthWind | Employees     | 9 inserts into output
info  | NorthWind | Shippers      | 3 from input
info  | NorthWind | Shippers      | 3 inserts into output
info  | NorthWind | Suppliers     | 29 from input
info  | NorthWind | Suppliers     | 29 inserts into output
info  | NorthWind | Categories    | 8 from input
info  | NorthWind | Categories    | 8 inserts into output
info  | NorthWind |               | 2155 records inserted into flat
info  | NorthWind |               | Time elapsed: 00:00:02.66

<strong>>tfl -a "c:\Temp\NorthWind.xml"</strong>
info  | NorthWind | Order Details | Change Detected: No.
info  | NorthWind | Orders        | Change Detected: No.
info  | NorthWind | Products      | Change Detected: No.
info  | NorthWind | Customers     | Change Detected: No.
info  | NorthWind | Employees     | Change Detected: No.
info  | NorthWind | Shippers      | Change Detected: No.
info  | NorthWind | Suppliers     | Change Detected: No.
info  | NorthWind | Categories    | Change Detected: No.
info  | NorthWind |               | Time elapsed: 00:00:00.59
</pre>

### Incrementals (Part 2)

Let's simulate a data change:

```sql
USE [NorthWind];

UPDATE Customers
SET CompanyName = 'Bottom Dollar Markets'
WHERE CustomerID = 'BOTTM';
```
Now run Transformalize again:

<pre style="font-size:smaller;">
<strong>>tfl -a "c:\Temp\NorthWind.xml"</strong>
info  | NorthWind | Order Details | Change Detected: No.
info  | NorthWind | Orders        | Change Detected: No.
info  | NorthWind | Products      | Change Detected: No.
info  | NorthWind | Customers     | Change Detected: Input: 0x75ad2 > Output: 0x73bb5
<strong>info  | NorthWind | Customers     | 1 from input
info  | NorthWind | Customers     | 1 to output
info  | NorthWind | Customers     | 1 updates to output</strong>
info  | NorthWind | Employees     | Change Detected: No.
info  | NorthWind | Shippers      | Change Detected: No.
info  | NorthWind | Suppliers     | Change Detected: No.
info  | NorthWind | Categories    | Change Detected: No.
<strong>info  | NorthWind |               | 35 records updated in flat</strong>
info  | NorthWind |               | Time elapsed: 00:00:00.74
</pre>

Using the version, Transformalize picked up the one change in *Customers*.  Since this 
customer has purchased 35 items (in *Order Details*), the flat table is updated as well.

#### Scheduling Incrementals

> Intrucing the **`-s`** (schedule) flag

Most likely, you'll want to schedule incremantals so that the de-normalized data is current. 
Transformalize uses [Quartz.NET](https://www.quartz-scheduler.net) for this. Using 
the **`-s`** schedule flag, pass in a [cron expression](http://www.quartz-scheduler.org/documentation/quartz-2.x/tutorials/tutorial-lesson-06.html) 
like this:

<pre style="font-size:smaller;">
<strong>>tfl -a "c:\Temp\NorthWind.xml" -s "0/5 * * * * ?"
info  | Process   |                 Starting Scheduler: 0/5 * * * * ?</strong>
info  | NorthWind |               | Compiled NorthWind user code in 00:00:00.1032057.
info  | NorthWind | Order Details | Change Detected: No.
info  | NorthWind | Orders        | Change Detected: No.
... and just keeps running ...
</pre>

This runs an incremental every five seconds until you press **`CTRL-C`**.  If you 
want to run Transformalize as a service, I recommend using [NSSM](https://nssm.cc).

### Transformations to Make Life Easier

> * Introducing the **`copy`** transform
> * the **`datePart`** transform
> * the **`format`** transform
> * the **`toUpper`** transform

Most often, in addition to de-normalization, you'll need to transform records too. 
Transformalize de-normalizes and transforms at the same time (thus, the name).

Let's add some time [dimension](https://en.wikipedia.org/wiki/Dimension_(data_warehouse)) fields. 
Modify the *Orders* entity to include a `<calculated-fields/>` section like this:

```xml
<calculated-fields>
  <add name="OrderYear" type="int" t="copy(OrderDate).datePart(year)" />
  <add name="OrderMonthSortable" t="format({OrderDate:MM-MMM}).toUpper()" />
  <add name="OrderDaySortable" t="format({OrderDate:yyyy-MM-dd})" />
  <add name="OrderDayOfWeek" t="copy(OrderDate).datePart(dayOfWeek)" />
</calculated-fields>		
```

**Note**: The **`copy`** method is mainly used to copy 
other fields into your transformation.  Generally speaking, when a 
transform uses field names in it's expression (e.g. **`js`**, **`cs`**, and **`format`**), 
you don't need to preceed it with a **`copy`** method.

After re-initializing, *NorthWindFlat* has some helpful time related fields that allow you 
to run queries like:

```sql
SELECT OrderDayOfWeek AS [Day], SUM(Revenue) AS [Sales]
FROM NorthWindFlat
GROUP BY OrderDayOfWeek
```
<pre style="font-size:smaller;">
<strong>Day         Sales</strong>
Friday      284393.64
Monday      275256.90
Thursday    256143.26
Tuesday     272113.27
Wednesday   266546.72
</pre>

Note that the query isn't dealing with joins or parsing dates. This is 
because we de-normalized it and pre-calculated useful fields.

## Post De-Normalization

> * Introducing system fields in output
> * the **`read-only`** attribute

Transformalize must use a relation output to de-normalize (i.e. SQLite).  However, now that it's flat, 
we can leverage the non-relational providers as well.

Transformalize records four *system* fields that may 
be used by additional `tfl` arrangements and/or other systems:

* TflKey - a surrogate key (an auto-incrementing value)
* TflBatchId - a version number corresponding to `tfl` runs
* TflHashCode - a numerical value calculated from every field (used for comparisons)
* TflDeleted - a boolean field tracking deletes (an optional setting)

**Note:** You can disable system fields by setting `read-only` 
to `true` in the top-most `<cfg/>` element.

### Leveraging Elasticsearch & Kibana

> Introducing the **elasticsearch** provider

This section demonstrates how to load the flattened Northwind 
data into [Elasticsearch](https://www.elastic.co/products/elasticsearch) 
and view it with [Kibana](https://www.elastic.co/products/kibana).

#### Elasticsearch

Start a new arrangement with this in your XML editor:

```xml
<cfg name="NorthWind">
  <connections>
    <add name="input" provider="sqlite" file="c:\temp\NorthWind.sqlite3" />
    <add name="output" 
         provider="elasticsearch" 
         server="localhost" 
         port="9200" 
         index="NorthWind" 
         version="5" />
  </connections>
  <entities>
    <add name="NorthWindFlat" version="TflBatchId" >
      <fields>
        <add name="TflKey" alias="Key" type="long" primary-key="true" />
        <add name="TflBatchId" alias="Version" type="long" />
        <add name="Revenue" type="decimal" precision="19" scale="2" />
        <add name="Freight" type="decimal" precision="19" scale="4" />
        <add name="OrderDate" type="datetime" />
        <add name="OrderYear" type="long" />
        <add name="OrderMonthSortable" />
        <add name="Country" length="15" />
        <add name="CategoryName" length="15" />
      </fields>
    </add>
  </entities>
</cfg>
```

This arrangement uses an elasticsearch output.  Save as *NorthWindToES.xml* and run in it:

<pre style="font-size:smaller;">
<strong>>tfl -a c:\temp\NorthWindToES.xml -m init</strong>
warn  | NorthWind | NorthWindFlat | Initializing
info  | NorthWind | NorthWindFlat | 2155 from input
info  | NorthWind | NorthWindFlat | 2155 to output
info  | NorthWind |               | Time elapsed: 00:00:02.40

<strong>>tfl -a c:\temp\NorthWindToES.xml</strong>
info  | NorthWind | NorthWindFlat | Starting
info  | NorthWind | NorthWindFlat | Change Detected: No.
info  | NorthWind |               | Time elapsed: 00:00:00.30
</pre>

A quick query in your browser can confirm records loaded:

[http://localhost:9200/northwind/northwindflat/_search?q=*:*&size=0](http://localhost:9200/northwind/northwindflat/_search?q=*:*&size=0)

```json
{
    "took": 2,
    "timed_out": false,
    "_shards": {
        "total": 5,
        "successful": 5,
        "failed": 0
    },
    "hits": {
        "total": 2155,
        "max_score": 0.0,
        "hits": []
    }
}
```

#### Kibana

Kibana offers interactive dashboards based on Elasticsearch 
indexes. Here's a quick 30 second video:

[![NorthWind in Kibana](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/northwind-in-kibana-youtube.png)](https://youtu.be/NzrFiG54foc "Northwind in Kibana")

### Leveraging SOLR & Banana

> Introducing the **solr** provider

This section demonstrates how to load the flattened Northwind 
data into [SOLR](http://lucene.apache.org/solr) 
and view it with [Banana](https://github.com/lucidworks/banana).

#### SOLR

Start a new arrangement with this in your XML editor:

```xml
<cfg name="NorthWind">
    <connections>
        <add name="input" provider="sqlite" file="c:\temp\NorthWind.sqlite3" />
        <add name="output" 
             provider="solr" 
             server="localhost" 
             port="8983" 
             path="solr" 
             core="northwind" 
             folder="C:\java\solr-6.6.0\server\solr" />
    </connections>
    <entities>
        <add name="NorthWindFlat" version="TflBatchId">
            <fields>
                <add name="TflKey" alias="Key" type="long" primary-key="true" />
                <add name="TflBatchId" alias="Version" type="long" />
                <add name="Revenue" type="decimal" precision="19" scale="2" />
                <add name="Freight" type="decimal" precision="19" scale="4" />
                <add name="OrderDate" type="datetime" />
                <add name="OrderYear" type="long" />
                <add name="OrderMonthSortable" />
                <add name="Country" length="15" />
                <add name="CategoryName" length="15" />
            </fields>
        </add>
    </entities>
</cfg>
```

Save as *NorthWindToSOLR.xml* and run:

<pre style="font-size:smaller;">
<strong>>tfl -ac:\Temp\NorthWindToSOLR.xml -m init</strong>
info  | NorthWind | NorthWindFlat | Starting
info  | NorthWind | NorthWindFlat | 2155 from input
info  | NorthWind | NorthWindFlat | 2155 to output
info  | NorthWind | NorthWindFlat | Ending
info  | NorthWind |               | Time elapsed: 00:00:06

<strong>>tfl -ac:\Temp\NorthWindToSOLR.xml</strong>
info  | NorthWind | NorthWindFlat | Starting
info  | NorthWind | NorthWindFlat | Change Detected: No.
info  | NorthWind |               | Time elapsed: 00:00:00.285
</pre>

A quick query in your browser can confirm the records loaded:

[http://localhost:8983/solr/northwind/select?indent=on&q=*:*&rows=0&wt=json](http://localhost:8983/solr/northwind/select?indent=on&q=*:*&rows=0&wt=json)

```json
{
    "responseHeader": {
        "status": 0,
        "QTime": 0,
        "params": {
            "q": "*:*",
            "indent": "on",
            "rows": "0",
            "wt": "json"
        }
    },
    "response": {
        "numFound": 2155,
        "start": 0,
        "docs": []
    }
}
```

#### Banana

Similar to Kibana, Banana offers interactive dashboards.  However, it's 
works against SOLR indexes instead of Elasticsearch. Here's a quick 20 second video:

[![NorthWind in Banana](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/northwind-in-banana-youtube.png)](https://youtu.be/59t5HJRsv_4 "Northwind in Banana")

### Leveraging SQL Server [Analysis Services](https://en.wikipedia.org/wiki/Microsoft_Analysis_Services) (SSAS) & Excel

> * Introducing the **sqlserver** provider
> * the **ssas** provider
> * the `measure` and `dimension` attributes on `fields`

This section demonstrates loading the data into a *SSAS* 
cube and browsing it with Excel.  To follow along, 
you'll need a local instance of Analysis Services, and Excel.

The SSAS provider only works with a SQL Server input, so first 
make a database called `TflNorthWind`, and then modify 
the *NorthWind.xml* arrangement to output to SQL Server 
instead of SQLite:

```xml
<cfg name="NorthWind" flatten="true">
  <connections>
    <add name="input" provider="sqlserver" server="localhost" database="NorthWind"/>
    <!-- change output to ... -->
    <add name="output" 
         provider="sqlserver" 
         server="localhost"
         database="TflNorthWind" />
  </connections>
  <!-- clipped for brevity -->
</cfg>
```

Run this in `init` mode to load `NorthWindFlat` into 
SQL Server. Then, create a new arrangement:

```xml
<cfg name="NorthWind">
    <connections>
        <add name="input" provider="sqlserver" server="localhost" database="TflNorthWind" />
        <add name="output" provider="ssas" server="localhost" database="NorthWind" />
    </connections>
    <entities>
        <add name="NorthWindFlat" version="TflBatchId" alias="Properties" >
            <fields>
                <add name="TflKey" type="int" primarykey="true" alias="Key" />
                <add name="TflBatchId" type="int" alias="Version" />
                <add name="Revenue" type="decimal" scale="2" measure="true" format="$###,###,###.00" />
                <add name="Freight" type="decimal" precision="19" scale="4" measure="true" format="$###,###,###.00" />
                <add name="OrderYear" type="int" dimension="true" />
                <add name="OrderMonthSortable" />
                <add name="Country" length="15" />
                <add name="EmployeeID" type="int" measure="true" aggregate-function="distinctcount" label="Employees" />
                <add name="CategoryName" length="15" />
            </fields>
        </add>
    </entities>
</cfg>
```

Save this as *NorthWindToSSAS.xml* and run it:

<pre style="font-size:smaller;">
<strong>>tfl -a c:\Temp\NorthWindToSSAS.xml -m init</strong>
info  | NorthWind | Properties | Creating new OLAP database: NorthWind
info  | NorthWind | Properties | Creating new data source: TflNorthWind
info  | NorthWind | Properties | Creating new data source view: NorthWind
info  | NorthWind | Properties | Creating new dimension: Properties
info  | NorthWind | Properties | Creating new cube: NorthWind
info  | NorthWind | Properties | Processing OLAP database NorthWind
info  | NorthWind |            | Time elapsed: 00:00:03.52

<strong>>tfl -a c:\Temp\NorthWindToSSAS.xml</strong>
info  | NorthWind | Properties | Change Detected: No.
info  | NorthWind |            | Time elapsed: 00:00:00.58
</pre>

This example marks some fields as [measures](https://en.wikipedia.org/wiki/Measure_(data_warehouse)) 
and others as [dimension](https://en.wikipedia.org/wiki/Dimension_(data_warehouse)) attributes.  This 
is needed to accurately describe the cube.  Here is a short video showing Excel browse the resulting cube.

[![NorthWind in Excel](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/northwind-in-excel-youtube.png)](https://youtu.be/X23pVSuxN64 "Northwind in Excel")

Note: The SSAS output is still under development and only tested on SQL Server 2008 R2.

### Leveraging the Orchard CMS Module

> * Introducing the **Orchard CMS** module
> * the **`parameters`** section
> * the **`filter`** section within an `entity`
> * the **`page`**, **`page-size`**, and **`sortable`** attributes for an `entity`
> * and the label attribute for a`field`

The [Orchard CMS](http://www.orchardproject.net) Transformalize module allows you to:

* edit, store, and secure your arrangements
* run your arrangements (like the CLI does)
* view and page through your output in *report mode*
* export search results (to csv, and xlsx) in *report mode*

Although arranging a report in Transformalize can add some 
complexity, it still makes sense since reporting is just a 
form of ETL.

Here's a quick video of a Northwind report using the Elasticsearch 
provider we loaded earlier:

[![NorthWind in Orchard CMS](https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/northwind-in-orchard-cms-youtube.png)](https://youtu.be/CCTvjsrUtHk "Northwind in Orchard CMS")

The arrangement for this is:

```xml
<cfg name="NorthWind">
  <parameters>
    <add name="orderyear" label="Year" value="*" prompt="true" multiple="true" />
    <add name="categoryname" label="Category" value="*" prompt="true" />
  </parameters>
  <connections>
    <add name="input" provider="elasticsearch" index="northwind" />
  </connections>
  <entities>
    <add name="northwindflat" alias="NorthWind" page="1" page-size="10" sortable="true" >
      <filter>
        <add field="orderyear" value="@[orderyear]" type="facet" min="0" />
        <add field="categoryname" value="@[categoryname]" type="facet" />
      </filter>
      <fields>
        <add name="orderyear" type="long" label="Year" />
        <add name="ordermonthsortable" label="Month" />
        <add name="orderdate" type="datetime" label="Date" format="yyyy-MM-dd" />
        <add name="tflkey" alias="Key" type="long" primary-key="true" output="false" />
        <add name="country" label="Country" length="15" />
        <add name="categoryname" length="15" label="Category" />
        <add name="freight" label="Freight" type="decimal" precision="19" scale="4" format="$#,###,###.00" />
        <add name="revenue" label="Revenue" type="decimal" precision="19" scale="2" format="$#,###,###.00" />
      </fields>
    </add>
  </entities>
</cfg>
```

#### Parameters

> Introducing the **`name`**, **`label`**, **`value`**, **`prompt`**, and **`multiple`** attributes for **`parameters`**

Parameters allow you to pass in data from outside your arrangement. 
They may be used to manipulate attribute values in the arrangement. 
The parameter place-holders (e.g. `@[orderyear]`) are replaced with 
a provided or default value before validation.

Parameters are visible in report mode 
when `prompt` is set to `true`.

#### Filter

> Introducing the **`field`**, **`operator`**, **`value`**, **`expression`**, and **`type`** attributes for each **`filter`**

Filters allow you to limit your output. A filter is set in two ways:

1. by setting `field`, `operator`, and `value` 
2. by setting a provider-specific `expression`

Either way, you may use parameters to manipulate 
your filters at run-time.

SOLR and Elasticsearch support faceted navigation.  When you set 
the filter's type to *facet*, Transformalize takes 
care of mapping the facet values to a parameter's choices.

#### Paging

Without paging, web-based reporting gets too big for 
the browser.  All providers are capable of paging, 
but SOLR and Elasticsearch do it the best.

#### Credits &amp; Thanks

**TODO**: list open source projects

<a href="https://www.jetbrains.com/resharper">
    <img src="https://raw.githubusercontent.com/dalenewman/Transformalize/master/Files/resharper-logo.png" alt="Resharper" width="100px" style="width: 100;"/>
</a>
