Transformalize
==============
Released under GNU General Public License, version 3 (GPL-3.0).

Combining de-normalization, transformation, replication, and awesome-ness.

### !!! Update !!!

This code-base has "expired" (so to speak).  I am re-writing 
Transformalize with dependency injection in mind.  I will 
publish the code here shortly.  It will be licensed under 
Apache 2 instead of GPL3.

=======

Transformalize aims to transform and denormalize relational data in near real-time. The resulting data may be used in several ways:

* As an OLAP cube data source
* To feed a SOLR, Elasticsearch, or Lucene index.
* To provide faster, simpler, non-blocking access for SQL queries
* Or, use your imagination... (e.g. to feed a Redis cache server, to load a NoSql database, etc.)

Transformalize is an open source .NET 4.5 class library. It may be
referenced and run directly in code, or run within
[Orchard CMS](http://www.orchardproject.net), or with
an included console application (`tfl.exe`).
It's source code is hosted on [GitHub](https://github.com/dalenewman/Transformalize).

### Demo

Start with a process configuration:

```xml
<transformalize>
    <processes>
        <add name="NorthWind">
            <connections>
                <add name="input" />
                <add name="output" />
            </connections>
        </add>
    </processes>
</transformalize>
```

First, setup connections:

```xml
<connections>
    <add name="input" database="NorthWind"/>
    <add name="output" database="NorthWindOutput"/>
</connections>
```

I set the input to `NorthWind` and the output to `NorthWindOutput`. These are both
SQL Server databases.  Connections are trusted *sqlserver* by default.
If you're following along at home, create these databases first.  Then, populate the
Northwind database with this [sql script](http://www.microsoft.com/en-us/download/details.aspx?id=23654).

#### The NorthWind Schema...

<img src="http://www.codeproject.com/KB/database/658971/NorthWindOrderDetails.png" class="img-responsive img-thumbnail" alt="Northwind Schema" />

The schema pictured above shows 8 tables in the `NorthWind` database.  The most important *fact* table is `Order Details`.
So, I add it as the first entity and save the configuration as *NorthWind.xml*.

```xml
<entities>
    <add name="Order Details"/>
</entities>
```

Using the console application (`tfl.exe`), run Transformalize in &quot;metadata&quot; mode:

`tfl NorthWind.xml {'mode':'metadata'}`

Metadata mode reads the information schema of the database.
&nbsp;Then, it writes and opens an XML file with Order Detail&#39;s
primary key and field definitions. Copy them into _NorthWind.xml_:

```xml
<entities>
    <add name="Order Details">
        <fields>
            <add name="OrderID" type="int" primary-key="true" />
            <add name="ProductID" type="int" primary-key="true" />
            <add name="Discount" type="single" />
            <add name="Quantity" type="short" />
            <add name="UnitPrice" type="decimal" precision="19" scale="4"/>
        </fields>
    </add>
</entities>
```

Now, run Transformalize in Initialize mode:

```bash
tfl NorthWind.xml {'mode':'init'}
23:38:57 | Info | NorthWind | All | Initialized TrAnSfOrMaLiZeR.
23:38:57 | Info | NorthWind | All | Initialized NorthWindOrderDetails in NorthWindOutput on localhost.
23:38:57 | Info | NorthWind | All | Process completed in 00:00:00.5585967.
```

Initialize mode initializes the output, preparing a place to store the data. Now run Tfl without specifying a mode:

```bash
tfl NorthWind.xml
23:43:01 | Info | NorthWind | Order Details....... | Processed 2155 inserts, and 0 updates in Order Details.
23:43:01 | Info | NorthWind | Order Details....... | Process completed in 00:00:00.7455880.
```

Transformalize copied the data that is configured in Northwind.xml. If we run it again, this happens:&nbsp;

```bash
tfl NorthWind.xml
23:44:18 | Info | NorthWind | Order Details....... | Processed 0 inserts, and 2155 updates in Order Details.
23:44:18 | Info | NorthWind | Order Details....... | Process completed in 00:00:01.0926105.
```

It updates the data. It copies new and updates existing data, but it is inefficient. The 2155 records have not been modified in the source, but they have been updated unnecessarily in the destination. So, we need to add a _version_ column to `Order Details` entity.  A version column should be a value that will increment anytime a record is inserted or updated.  Conveniently, SQL Server offers a ROWVERSION type that gives us a version column without having to modify the application or add a trigger.

```sql
ALTER TABLE [Order Details] ADD RowVersion ROWVERSION;
```

Update the `Order Details` entity to use RowVersion:&nbsp;

```xml
<entities>
    <add name="Order Details" version="RowVersion">
        <fields>
            <add name="OrderID" type="System.Int32" primary-key="true" />
            <add name="ProductID" type="System.Int32" primary-key="true" />
            <add name="Discount" type="System.Single" />
            <add name="Quantity" type="System.Int16" />
            <add name="RowVersion" type="System.Byte[]" length="8" />
            <add name="UnitPrice" type="System.Decimal" precision="19" scale="4"/>
        </fields>
    </add>
</entities>
```

Re-initialize and run twice:

```bash
tfl NorthWind.xml {'mode':'init'}
23:58:52 | Info | NorthWind | All | Initialized TrAnSfOrMaLiZeR.
23:58:52 | Info | NorthWind | All | Initialized NorthWindOrderDetails in NorthWindOutput on localhost.
23:58:52 | Info | NorthWind | All | Process completed in 00:00:00.5504415.
tfl NorthWind.xml
00:00:18 | Info | NorthWind | Order Details....... | Processed 2155 inserts, and 0 updates in Order Details.
00:00:18 | Info | NorthWind | Order Details....... | Process completed in 00:00:00.7417452.
tfl NorthWind.xml
00:00:23 | Info | NorthWind | Order Details....... | Processed 0 inserts, and 0 updates in Order Details.
00:00:23 | Info | NorthWind | Order Details....... | Process completed in 00:00:00.6042720.
```

Now it doesn't update data unnecessarily.  It's using the version field to sense that the data hasn't been updated.  Let's view the output.

```sql
SELECT TOP 10
Discount,
OrderID,
ProductID,
Quantity,
UnitPrice
FROM NorthWindStar;
```

<pre>
Discount   OrderID     ProductID   Quantity UnitPrice
---------- ----------- ----------- -------- ---------
0.2        10248       11          12       14.0000
0          10248       42          10       9.8000
0          10248       72          5        34.8000
0          10249       14          9        18.6000
0          10249       51          40       42.4000
0          10250       41          10       7.7000
0.15       10250       51          35       42.4000
0.15       10250       65          15       16.8000
0.05       10251       22          6        16.8000
0.05       10251       57          15       15.6000
</pre>

Review the NorthWind diagram. The next closest tables to `Order Details` are `Orders` and `Products`. Add the `Orders` entity. Hint: Add entity <add name="Orders" /> and run Tfl in metadata mode.

```xml
<add name="Orders" version="RowVersion">
    <fields>
        <add name="OrderID" type="System.Int32" primary-key="true" ></add>
        <add name="Discount" type="System.Single" ></add>
        <add name="Quantity" type="System.Int16" ></add>
        <add name="RowVersion" type="System.Byte[]" length="8" ></add>
        <add name="UnitPrice" type="System.Decimal" precision="19" scale="4"></add>
        <add name="CustomerID" type="System.Char" length="5" ></add>
        <add name="EmployeeID" type="System.Int32" ></add>
        <add name="Freight" type="System.Decimal" precision="19" scale="4"></add>
        <add name="OrderDate" type="System.DateTime" ></add>
        <add name="RequiredDate" type="System.DateTime" ></add>
        <add name="RowVersion" type="System.Byte[]" length="8" ></add>
        <add name="ShipAddress" length="60" ></add>
        <add name="ShipCity" length="15" ></add>
        <add name="ShipCountry" length="15" ></add>
        <add name="ShipName" length="40" ></add>
        <add name="ShippedDate" type="System.DateTime" ></add>
        <add name="ShipPostalCode" length="10" ></add>
        <add name="ShipRegion" length="15" ></add>
        <add name="ShipVia" type="System.Int32" ></add>
    </fields>
</add>
```

Re-initialize.

```bash
tfl NorthWind.xml {'mode':'init'}
22:32:14 | Error | NorthWind | The entity Orders must have a relationship to the master entity Order Details.
```

When another table is added, it must be related to the master table. The master table is the first table defined. In this case, it's `Order Details`. So, we have to add a relationship:

```xml
<!-- snip --->
</entities>
    <relationships>
        <add left-entity="Order Details" left-field="OrderID" 
             right-entity="Orders"       right-field="OrderID"/>
    </relationships>
</process>
```

Re-initialize.

```bash
tfl NorthWind.xml {'mode':'init'}
23:13:31 | Error | NorthWind | field overlap error in Orders. The field: RowVersion is already defined in a previous entity.  You must alias (rename) it.
```

Just like in SQL views, multiple entities (or tables) joined together can introduce identical field names. &nbsp;So, you have to re-name (or alias) any columns that have the same name. &nbsp;In this case, it&#39;s our RowVersion column that we&#39;re using to detect changes. &nbsp;So, alias the RowVersion in the Orders entity to OrdersRowVersion like this:&nbsp;

```bash
<add name="Orders" version="RowVersion">
	<fields>
		<!-- snip -->
		<add name="RowVersion" alias="OrdersRowVersion" type="byte[]" length="8" />
	</fields>
</add>
```

Re-initialize and run twice.

```bash
tfl NorthWind.xml {'mode':'init'}
23:23:47 | Info | NorthWind | All | Initialized TrAnSfOrMaLiZeR.
23:23:47 | Info | NorthWind | All | Initialized NorthWindOrderDetails in NorthWindOutput on localhost.
23:23:47 | Info | NorthWind | All | Initialized NorthWindOrders in NorthWindOutput on localhost.
23:23:47 | Info | NorthWind | All | Process completed in 00:00:00.6609756.
tfl NorthWind.xml
23:24:30 | Info | NorthWind | Order Details....... | Processed 2155 inserts, and 0 updates in Order Details.
23:24:30 | Info | NorthWind | Orders.............. | Processed 830 inserts, and 0 updates in Orders.
23:24:30 | Info | NorthWind | Orders.............. | Process completed in 00:00:00.9719255.
tfl NorthWind.xml
23:24:35 | Info | NorthWind | Order Details....... | Processed 0 inserts, and 0 updates in Order Details.
23:24:35 | Info | NorthWind | Orders.............. | Processed 0 inserts, and 0 updates in Orders.
23:24:35 | Info | NorthWind | Orders.............. | Process completed in 00:00:00.7284382.
```

View the output:

```sql
SELECT TOP 10
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
FROM NorthWindStar;
```

<pre>
Disc OrderID PId Qty UnitPrice  CustId EId Freight  OrderDate  RequiredDate ShipAddress            ShipCity        ShippedDate ShipPostalCode ShipRegion Sid
---- ------- --- --- ---------  ------ --- -------- ---------- ------------ ---------------------- --------------- ----------- -------------- ---------- ---
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

Now, rinse and repeat. &nbsp;That is, consult the NorthWind diagram and continue adding related entities until the relationships configuration look like this:

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

As you might expect, adding all these entities creates many duplicate field names. Instead of renaming each one, we can add a prefix to the entity. A prefix aliases all the fields as prefix + name.

```xml
<add name="Employees" version="RowVersion" prefix="Employee">
    <fields>
		<!-- ... -->
    </fields>
</add>
```

Initialize, and run twice. Console output should look like this:

```bash
tfl NorthWind.xml {'mode':'init'}
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
```

Now there are 81 fields available in the output `NorthWindStar`:

```sql
SELECT COUNT(*) AS FieldCount
From INFORMATION_SCHEMA.COLUMNS
Where TABLE_NAME = 'NorthWindStar';
```
<pre>
FieldCount
----------
81
</pre>

<img src="http://www.codeproject.com/KB/database/658971/NorthWindOrderDetailsStar.png" class="img-responsive img-thumbnail" alt="Northwind Star Schema" />

As you can see in the diagram above, I haven&#39;t completely de-normalized the data. &nbsp;Instead, I have created a [star schema](http://en.wikipedia.org/wiki/Star_schema), where every related table has a single join to the master table. &nbsp;In addition, I have created a view (e.g. NorthWind**Star**) so users may query it as if it we&#39;re a single table.&nbsp; Having this single view of the duplicated data allows us to quickly create simple cubes or integrate with search engines:&nbsp;

####Leveraging SQL Server Analysis Services

Open up [BIDS](http://technet.microsoft.com/en-us/library/ms173767%28v=sql.105%29.aspx), and create an Analysis Services Project (or cube) to browse the data.

* Set the data source to your NorthWindOutput database&nbsp;
* Set the data view to the NorthWindStar view
* Create a dimension using all the fields (except the binary ones) in the view. Call it Properties.
* Create a cube with a couple measures (e.g. UnitPrice, Quantity) and connect it to the Properties dimension.
* Process the cube and browse it. Note: You may have to grant NT AUTHORITY\LOCAL SERVICE read writes to the NorthWindOutput database.

<img src="http://www.codeproject.com/KB/database/658971/NorthWindBrowse1.png" class="img-responsive img-thumbnail" alt="Business Intelligence Development Studio" />

As you can see, slicing the measures by order date isn&#39;t ideal. Moreover, the unit price and quantity measures don&#39;t help much by themselves. This cube needs a time hierarchy and revenue calculation. We can add them with Transformalize. First, add three calculated fields based on &quot;order date&quot; to create a time hierarchy:

```xml
<add name="Orders" version="RowVersion" prefix="Orders">
    <fields>
      <!-- snip -->
    </fields>
    <calculated-fields>
      <add name="TimeDate" length="10" default="9999-12-31">
        <transforms>
            <add method="toString" format="yyyy-MM-dd" parameter="OrderDate" />
        </transforms>
      </add>
      <add name="TimeMonth" length="6" default="12-DEC">
        <transforms>
            <add method="toString" format="MM-MMM" parameter="OrderDate" />
            <add method="toUpper" />
        </transforms>
      </add>
      <add name="TimeYear" type="System.Int16" default="9999">
        <transforms>
            <add method="toString" format="yyyy" parameter="OrderDate" />
		</transforms>
      </add>
    </calculated-fields>
</add>
```

Calculated fields project new fields based on the values of other fields and previously defined other calculated fields. They are used at the entity level, or at the process level. In an entity, they have access to any field within their entity. In a process, they have access to all of the data. To control which fields they have access to, use parameters like this:

```xml
<transform method="format" format="{0} is a big city!">
    <parameters>
        <add field="City" />
    </parameters>
</transform>
```

You may add multiple parameters in this way. &nbsp;However, if you only have a single parameter, you can specify it in the parameter attribute in the transform element itself, like this:

```xml
<transform method="format" format="{0} is a big city!" parameter="City" /> 
```

Another short-cut is to set the parameter attribute to &quot;*&quot; to include all fields.

There are many built-in [Transforms](https://github.com/dalenewman/Transformalize/wiki/Transforms).  If you can&#39;t find one that fits your needs, you can use the C#, JavaScript, or the Razor template transforms to define your own. Let&#39;s use a JavaScript transform to calculate revenue:

```xml
<calculated-fields>
    <add name="Revenue" type="System.Decimal" >
        <transforms>
            <add method="javascript" script="(UnitPrice * (1 - Discount)) * Quantity" parameter="*" />
        </transforms>
    </add>
</calculated-fields>
```

Re-initialize and run Tfl. Then, using the new time fields and revenue, see if it improves the cube browsing experience.

<img src="http://www.codeproject.com/KB/database/658971/NorthWindBrowse2.png" class="img-responsive img-thumbnail" alt="Business Intelligence Development Studio (after)" />

The cube looks better now, but we&#39;ll need it to update whenever Transformalize runs. &nbsp;So, &nbsp;add a connection to Analysis Services and a corresponding template action:&nbsp;

```xml
<connections>
    <add name="input" connection-string="server=localhost;Database=NorthWind;Trusted_Connection=True;"/>
    <add name="output" connection-string="Server=localhost;Database=NorthWindOutput;Trusted_Connection=True;"/>
    <add name="cube" connection-string="Data Source=localhost;Catalog=NorthWind;" provider="AnalysisServices"/>
</connections>
<!-- snip -->
<templates path="C:\Tfl\">
    <add name="process-cube" file="process-cube.xmla">
        <settings>
            <add name="DatabaseID" value="NorthWind2"></add>
        </settings>    
        <actions>
            <add action="run" connection="cube"></add>
        </actions>
    </add>
</templates>
```

Transformalize &quot;templates&quot; use [C# Razor syntax](http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx). Settings are passed into the template and used like this:

```xml
<Batch>
  <Process>
    <Object>
      <DatabaseID>@(Model.Settings.DatabaseID)</DatabaseID>
    </Object>
    <Type>ProcessFull</Type>
    <WriteBackTableCreation>UseExisting</WriteBackTableCreation>
  </Process>
</Batch>
```

The `@(Model.Settings.DatabaseID)` will be replaced with `NorthWind2`.  Transformalize&#39;s template manager will render the template, and subsequently run defined &quot;actions.&quot; The &quot;run&quot; action executes the rendered content against the designated connection. &nbsp;This allows you to dynamically build data manipulation queries, or XMLA commands in this case, and execute them.

```bash
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
```

#### Leveraging Apache SOLR

With more complex templates, and an Apache [SOLR](http://lucene.apache.org/solr/) server, it is possible to integrate full text search into the process as well. &nbsp;Transformalize comes with a pair of templates that can build the necessary SOLR configuration files for schema, and data import handling.&nbsp;

```xml
<templates>
        <add name = "solr-data-handler" file="solr-data-handler.cshtml" cache="true">
      <actions>
        <add action = "copy" file="C:\Solr\NorthWind\conf\data-config.xml"/>
      </actions>
    </add>
    <add name = "solr-schema" file="solr-schema.cshtml" cache="true">
      <actions>
        <add action = "copy" file="C:\Solr\NorthWind\conf\schema.xml"/>
        <add action = "web" url="http://localhost:8983/solr/NorthWind/dataimport?command=full-import&amp;clean=true&amp;commit=true&amp;optimize=true"/>
      </actions>
    </add>
</templates>
<search-types>
    <add name = "default" />
    <add name="facet" analyzer="lowercase" store="true" index="true"/>
        <add name = "standard" analyzer="standard_lowercase" store="false" index="true"/>
</search-types>
```

The Razor templates "solr-data-handler.cshtml" And "solr-schema.cshtml" render the SOLR configuration files. This Is possible because the template manager passes the entire NorthWind configuration (that we built with XML above) into the templates.

To control how the fields are handled in SOLR, &quot;search types&quot; are applied to each `<field/>` Or `<calculated-field/>`. By default, each field is indexed and stored in the search index according to it&#39;s data type. To assign more complex text analysis, you can set the search-type attribute to facet, or standard, or any others you define. To exclude a field from search, set search-type to &quot;none.&quot;

Running Tfl now produces:  

```bash
tfl NorthWind.xml
...
00:48:25 | Info | NorthWind | Products.... | Processed 2155 rows. Updated Order Details with Products.
00:48:28 | Info | NorthWind | Categories.. | process-cube ran successfully.
00:48:28 | Info | NorthWind | Categories.. | Copied solr-data-handler template output to C:\Solr\NorthWind\conf\data-config.xml.
00:48:29 | Info | NorthWind | Categories.. | Copied solr-schema template output to C:\Solr\NorthWind\conf\schema.xml.
00:48:29 | Info | NorthWind | Categories.. | Made web request to http://localhost:8983/solr/NorthWind/dataimport?command=full-import&amp;clean=true&amp;commit=true&amp;optimize=true.
00:48:29 | Info | NorthWind | Categories.. | Process completed in 00:00:04.8287386.
```

In this example, the template action &quot;web&quot; triggers SOLR to clean and re-import the index. In a production environment, you&#39;d want to reload the schema when it changes, and make use of full and delta imports appropriately. If all goes well, you see something like this in the SOLR admin:

<img src="http://www.codeproject.com/KB/database/658971/NorthWindSolrDataImport.png" class="img-responsive img-thumbnail" alt="SOLR Admin" />

Now, if you schedule Transformalize to run every couple minutes, you have near real-time OLAP and search engine services on top of your OLTP data. &nbsp;An OLAP cube supports more performant and complex reporting requirements, and a search engine allows for lightning fast and _fuzzy_&nbsp;searches for specific records. &nbsp;If your users want to see the data in different ways, and they will, all you have to do is add transforms and/or new calculated fields and re-initialize your output. &nbsp;

When Transformalize reads your production databases, it attempts to do so introducing as little contention as possible.  Using version fields, it can keep a star-schema copy of very large databases up to date very quickly.

### Summary

The NorthWind data Is fairly clean. In reality, you&#39;ll face more challenging data sources.

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

Where possible, the source code from these projects Is included rather than the Nuget packages. The upside &nbsp;of doing this Is I get to step into And learn from other people&#39;s code. The downside is it&#39;s a bit harder to keep these libraries up to date.
