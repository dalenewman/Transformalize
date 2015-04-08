#Backup SQL Express

###Purpose###
For fun, I decided to use Transformalize (TFL) to backup some Sql Express databases. 
The process defined below creates unique names so it can 
scheduled to run everyday with a task manager.

###Instructions###

The contents of **full-backups.xml**:

<pre class="prettyprint" lang="xml">
&lt;?xml version="1.0" encoding="utf-8"?&gt;
&lt;transformalize&gt;
	&lt;processes&gt;
		&lt;add name="FullBackup" enabled="true"&gt;
		  &lt;connections&gt;
			&lt;add name="input" provider="sqlserver" server="@(Server)" database="master" /&gt;
			&lt;add name="output" provider="file" file="c:\temp\backup.sql" header="" /&gt;
		  &lt;/connections&gt;
		  &lt;actions&gt;
			&lt;add action="run" file="c:\temp\backup.sql" connection="input" /&gt;
		  &lt;/actions&gt;
		  &lt;entities&gt;
			&lt;add name="DatabaseBackups" 
                 detect-changes="false"
                 query="
                    SELECT [name] AS [Database]
                    FROM sys.databases WITH (NOLOCK)
                    WHERE [state] = 0
                    AND database_id &amp;gt; 4;" /&gt;
			  &lt;fields&gt;
				&lt;add name="Database" length="128" output="false" /&gt;
			  &lt;/fields&gt;
			  &lt;calculated-fields&gt;
				&lt;add name="Now" type="datetime" output="false"&gt;
				  &lt;transforms&gt;
					&lt;add method="now" to-time-zone="Eastern Standard Time" /&gt;
				  &lt;/transforms&gt;
				&lt;/add&gt;
				&lt;add name="Sql" length="2000" primary-key="true"&gt;
				  &lt;transforms&gt;
					&lt;add method="format" 
                         format="BACKUP DATABASE [{0}] TO DISK = N'@(Folder)\{0}-{1:yyyyMMdd-HHmmss}.bak' WITH NOFORMAT, NOINIT, NAME = '{0} Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;"&gt;
					  &lt;parameters&gt;
						&lt;add field="Database" /&gt;
						&lt;add field="Now" /&gt;
					  &lt;/parameters&gt;
					&lt;/add&gt;
				  &lt;/transforms&gt;
				&lt;/add&gt;
			  &lt;/calculated-fields&gt;
			&lt;/add&gt;
		  &lt;/entities&gt;
		&lt;/add&gt;
	&lt;/processes&gt;
&lt;/transformalize&gt;
</pre>

###Running###

Instead of a hard-coding a server and folder, 
I used Razor style place-holders `@(Server)` and `@(Folder)` above. 
Then, when running tfl.exe, pass a query string in with the values like this:

<pre class="prettyprint" lang="bash">
tfl.exe "full-backups.xml?Server=localhost\SQLEXPRESS&Folder=c:\SqlBackups"
</pre>

**Note**: Make sure to double quote the command line argument to ensure it is read as a single argument.

###Explanation###

####Connections####
There is an input and output connection:

+ The input is _from_ a SQL Server's master database.
+ The output is _to_ a file (`c:\temp\backup.sql` in this case).

<pre class="prettyprint" lang="xml">
    ...
    &lt;add name=&quot;input&quot; server=&quot;(@Server)&quot; database=&quot;master&quot; /&gt;
    &lt;add name=&quot;output&quot; provider=&quot;file&quot; file=&quot;c:\temp\backup.sql&quot; header=&quot;&quot; /&gt;
    ...
</pre>

####Entities####

Normally, TFL figures out how to get data for an entity, 
but in this configuration, a query over-rides that behavior. 
This query pulls the online (state=0) user (database_id>4) database names:

<pre class="prettyprint" lang="sql">
SELECT [name] AS [Database]
FROM sys.databases WITH (NOLOCK)
WHERE [state] = 0
AND database_id > 4;
</pre>

####Fields####

I define the expected database field and then two calculated-fields. 
The first field, with a `now` transform generates the current date time. 
The second composes a SQL statement to backup the database:

<pre class="prettyprint" lang="xml">
...
&lt;add name="Sql" length="1000" primary-key="true"&gt;
  &lt;transforms&gt;
    &lt;add method="format" 
         format="
            BACKUP DATABASE [{0}] TO DISK = N'@(Folder)\{0}.{1:yyyy.MM.dd}.bak' 
            WITH NOFORMAT, NOINIT, NAME = '{0} Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;"&gt;
      &lt;parameters&gt;
        &lt;add field="Database" /&gt;
        &lt;add field="Now" /&gt;
      &lt;/parameters&gt;
    &lt;/add&gt;
  &lt;/transforms&gt;
&lt;/add&gt;
...
</pre>

The transform uses the `format` method, which is .NET's `string.Format()` method.  I have included the `Database` and `Now` fields as parameters.  I set place-holders `{0}` and `{1}` for them.  When the data is output to the file, it comes out transformed like this:

<pre class="prettyprint" lang="sql" >
BACKUP DATABASE [Northwind] TO DISK = N'c:\temp\SqlBackups\Northwind-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'Northwind Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
BACKUP DATABASE [pubs] TO DISK = N'c:\temp\SqlBackups\pubs-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'pubs Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
BACKUP DATABASE [AdventureWorks] TO DISK = N'c:\temp\SqlBackups\AdventureWorks-20140613-160308.bak' WITH NOFORMAT, NOINIT, NAME = 'AdventureWorks Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
...
</pre>

####Actions####
By default, actions run after a process. The action below runs the newly created SQL script:

<pre class="prettyprint" lang="xml">
&lt;add action=&quot;run&quot; file=&quot;c:\temp\backup.sql&quot; connection=&quot;input&quot; /&gt;
</pre>

This says "Run the contents of `c:\temp\backup.sql` against the input connection."

If everything goes as expected, TFL will output something like this:

<pre class="prettyprint" lang="bash">
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in NowOperation: 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in TimeZoneOperation (Now): 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in FormatOperation (Sql): 0 seconds.
16:03:08 | Info | FullBackup | DatabaseBackups.. | Completed 39 rows in TruncateOperation: 0 seconds.
16:03:09 | Info | FullBackup | DatabaseBackups.. | Processed 39 inserts, and 0 updates in DatabaseBackups.
16:14:55 | Info | FullBackup | All.............. | backup.sql ran successfully.
16:14:55 | Info | FullBackup | All.............. | Process affected 39 records in 00:11:46.2697843.
</pre>

In my case, I had 39 databases on my computer, and a couple of them were **pretty big**!