# Backup SQL Express

### Purpose
For fun, I decided to use Transformalize (TFL) to backup some Sql Express databases. 
The process defined below creates unique names so it can 
scheduled to run everyday with a task manager.

### Instructions

The contents of **full-backups.xml**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<transformalize>
	<processes>
		<add name="FullBackup" enabled="true">
		  <connections>
			<add name="input" provider="sqlserver" server="@(Server)" database="master" />
			<add name="output" provider="file" file="c:\temp\backup.sql" header="" />
		  </connections>
		  <actions>
			<add action="run" file="c:\temp\backup.sql" connection="input" />
		  </actions>
		  <entities>
			<add name="DatabaseBackups" 
                 detect-changes="false"
                 query="
                    SELECT [name] AS [Database]
                    FROM sys.databases WITH (NOLOCK)
                    WHERE [state] = 0
                    AND database_id &gt; 4;" />
			  <fields>
				<add name="Database" length="128" output="false" />
			  </fields>
			  <calculated-fields>
				<add name="Now" type="datetime" output="false">
				  <transforms>
					<add method="now" to-time-zone="Eastern Standard Time" />
				  </transforms>
				</add>
				<add name="Sql" length="2000" primary-key="true">
				  <transforms>
					<add method="format" 
                         format="BACKUP DATABASE [{0}] TO DISK = N'@(Folder)\{0}-{1:yyyyMMdd-HHmmss}.bak' WITH NOFORMAT, NOINIT, NAME = '{0} Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;">
					  <parameters>
						<add field="Database" />
						<add field="Now" />
					  </parameters>
					</add>
				  </transforms>
				</add>
			  </calculated-fields>
			</add>
		  </entities>
		</add>
	</processes>
</transformalize>
```

### Running

Instead of a hard-coding a server and folder, 
I used Razor style place-holders `@(Server)` and `@(Folder)` above. 
Then, when running tfl.exe, pass a query string in with the values like this:

`tfl.exe "full-backups.xml?Server=localhost\SQLEXPRESS&Folder=c:\SqlBackups"`

**Note**: Make sure to double quote the command line argument to ensure it is read as a single argument.

### Explanation

#### Connections
There is an input and output connection:

+ The input is _from_ a SQL Server's master database.
+ The output is _to_ a file (`c:\temp\backup.sql` in this case).

```xml
<!-- ... -->
<add name="input" server="(@Server)" database="master" />
<add name="output" provider="file" file="c:\temp\backup.sql" header="" />
<!-- ... -->
```

#### Entities

Normally, TFL figures out how to get data for an entity, 
but in this configuration, a query over-rides that behavior. 
This query pulls the online (state=0) user (database_id>4) database names:

```sql
SELECT [name] AS [Database]
FROM sys.databases WITH (NOLOCK)
WHERE [state] = 0
AND database_id > 4;
```

#### Fields

I define the expected database field and then two calculated-fields. 
The first field, with a `now` transform generates the current date time. 
The second composes a SQL statement to backup the database:

```xml
<!-- snip -->
<add name="Sql" length="1000" primary-key="true">
  <transforms>
    <add method="format" 
         format="
            BACKUP DATABASE [{0}] TO DISK = N'@(Folder)\{0}.{1:yyyy.MM.dd}.bak' 
            WITH NOFORMAT, NOINIT, NAME = '{0} Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;">
      <parameters>
        <add field="Database" />
        <add field="Now" />
      </parameters>
    </add>
  </transforms>
</add>
<!-- snip -->
```

The transform uses the `format` method, which is .NET's `string.Format()` method.  I have included the `Database` and `Now` fields as parameters.  I set place-holders `{0}` and `{1}` for them.  When the data is output to the file, it comes out transformed like this:

```sql
BACKUP DATABASE [Northwind] TO DISK = N'c:\temp\SqlBackups\Northwind-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'Northwind Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
BACKUP DATABASE [pubs] TO DISK = N'c:\temp\SqlBackups\pubs-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'pubs Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
BACKUP DATABASE [AdventureWorks] TO DISK = N'c:\temp\SqlBackups\AdventureWorks-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'AdventureWorks Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
/* snip */
```

#### Actions
By default, actions run after a process. The action below runs the newly created SQL script:

```xml
<add action="run" file="c:\temp\backup.sql" connection="input" />
```

This says "Run the contents of `c:\temp\backup.sql` against the input connection."

If everything goes as expected, TFL will output something like this:

```bash
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in NowOperation: 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in TimeZoneOperation (Now): 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in FormatOperation (Sql): 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in TruncateOperation: 0 seconds.
16:03:09 | Info | FullBackup | DatabaseBackups.. | Processed 39 inserts, and 0 updates in DatabaseBackups.
16:14:55 | Info | FullBackup | All.............. | backup.sql ran successfully.
16:14:55 | Info | FullBackup | All.............. | Process affected 39 records in 00:11:46.2697843.
```

In my case, I had 39 databases on my computer, and a couple of them were **pretty big**!