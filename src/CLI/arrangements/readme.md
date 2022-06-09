## Arrangements

[internal.xml](internal.xml)

This self-contained configuration returns two rows with full name transformations.

`tfl -a arrangements/internal.xml`

[bogus.xml](bogus.xml)

This reads from the [bogus](https://github.com/dalenewman/Transformalize.Provider.Bogus) provider.  It accepts a `seed` and `size` parameter controlling 
the random seed and how many bogus records to generate.

```bash
tfl -a arrangements/bogus.xml
tfl -a arrangements/bogus.xml -p Seed=2
tfl -a arrangements/bogus.xml -p Seed=2 Size=10
```

[bogus-to-rdbms.xml](bogus-to-rdbms.xml)

This one is like *bogus.xml* but writes 100 bogus records to a database server.  It expects you 
have the server running with a database named *junk*.  It expects parameters: `provider`, `user`, and `password`.

```
tfl -a arrangements/bogus-to-rdbms.xml -p Provider=sqlserver User=sa Password=<password>
tfl -a arrangements/bogus-to-rdbms.xml -p Provider=mysql User=root Password=<password>
tfl -a arrangements/bogus-to-rdbms.xml -p Provider=postgresql User=postgres Password=<password>
```

[bogus-to-razor.xml](bogus-to-razor.xml)

This writes 10 bogus records out to bogus.html using a razor template.

```
tfl -a arrangements/bogus-to-razor.xml -p Template=arrangements/template.cshtml Output=bogus.html
```

[denormalize-northwind.xml](denormalize-northwind.xml)

This one does what the main Transformalize read me page does. It denormalizes a Northwind database with row versions 
on a SQL Server to a flat table on PostgreSql.

```
tfl -a arrangements/denormalize-northwind.xml -p Mode=init Password=<password>
tfl -a arrangements/denormalize-northwind.xml -p Password=<password>
```

### Setup
You'll want to have Docker running all the database servers.  Examples below:

**SQL Server** => `docker run --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=<password> -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest`

**MySql** => `docker run --name mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=<password> -e MYSQL_DATABASE=junk -d mysql:latest`

**Postgres** => `docker run --name postgres -p 5432:5432 -e POSTGRES_PASSWORD=<password> -d postgres:latest`