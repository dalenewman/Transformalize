## Arrangements

[internal.xml](internal.xml)

This self-contained configuration returns two rows with full name transformations.

`tfl -a internal.xml`

[bogus.xml](bogus.xml)

This reads from the [bogus](https://github.com/dalenewman/Transformalize.Provider.Bogus) provider.  It accepts a `seed` and `size` parameter controlling 
the random seed and how many bogus records to generate.

```bash
tfl -a bogus.xml
tfl -a bogus.xml -p Seed=2
tfl -a bogus.xml -p Seed=2 Size=10
```

[bogus-to-rdbms.xml](bogus-to-rdbms.xml)

This one is like *bogus.xml* but writes 100 bogus records to a database server.  It expects you 
have the server running with a database named *junk*.  It expects parameters: `provider`, `user`, and `password`.

```
tfl -a bogus-to-rdbms.xml -p Provider=sqlserver User=sa Password=<your-password>
tfl -a bogus-to-rdbms.xml -p Provider=mysql User=root Password=<your-password>
tfl -a bogus-to-rdbms.xml -p Provider=postgresql User=postgres Password=<your-password>
```

[denormalize-northwind.xml](denormalize-northwind.xml)

This one does what the main Transformalize read me page does. It denormalizes a Northwind database with row versions 
on a SQL Server to a flat table on PostgreSql.

```
tfl -a denormalize-northwind.xml -p Mode=init Password=<your-password>
tfl -a denormalize-northwind.xml -p Password=<your-password>
```

### Setup
You'll want to have Docker (Desktop) running to run all the 
database servers.  Example run statements provided:

**SQL Server** => `docker run --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=<your-password> -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest`

**MySql** => `docker run --name mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=<your-password> -e MYSQL_DATABASE
=Junk -d mysql:latest`

**Postgres** => `docker run --name postgres -p 5432:5432 -e POSTGRES_PASSWORD=<your-password> -d postgres:latest`

**PgAdmin** => `docker run --name pgadmin -p 8080:80 -d -e PGADMIN_DEFAULT_EMAIL=dalenewman@gmail.com -e PGAD
MIN_DEFAULT_PASSWORD=<your-password> dpage/pgadmin4`

To interact with the servers, I used:
- Azure Data Studio for SQL Server
- HeidiSQL for MySql
- pgAdmin4 for Postgres
