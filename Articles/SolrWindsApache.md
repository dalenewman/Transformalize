# SOLR on Windows 8.1 Pro with Apache Tomcat

<img src="http://www.transformalize.com/Media/Default/site/images/solar_system.jpg" class="img-thumbnail pull-right" style="margin: 0px 0px 10px 10px;" />

[SOLR](http://lucene.apache.org/solr/) is:

> the popular, blazing-fast, open source enterprise search platform built on Apache Lucene.

It's FREE and it is amazing.  It's an immediate value-add to nearly any data-set.

---

This is a short article explaining how I setup SOLR on my Windows development environment.  It includes setup for using the Data Import Handler with Microsoft SQL Server.

### Components

* Java
* Apache Tomcat Windows Service.
* SOLR
* JDBC Driver for Microsoft SQL Server

At the time of this writing:

* Java 1.8.0_25
* Apache Tomcat 8.0.14
* SOLR 4.10.2
* JDBC 4.0 March 2012

### Java
Change the default install location of Java to `c:\Java`.  This avoids problems with the spaces in the default location path (e.g. `c:\Program Files\...`)

### Apache Tomcat
Install using the defaults.  Then, check [http://localhost:8080](http://localhost:8080) to make sure it's working.

### SOLR
Extract the SOLR zip into your Java folder (e.g. `c:\Java\Solr-x.x.x`)

#### Cores
Make a new folder for SOLR cores.  Then, copy the example cores folder from `c:\Java\Solr-x.x.x\example\solr` into it.  This gives you the example core `collection1` to start with.

Then, modify `*\collection1\conf\solrconfig.xml`:

* Update the relative library paths (e.g. ../../..) with where you extracted SOLR to (e.g. `c:/Java/Solr-x.x.x`)
* Remove the sample `elevate` request handler (or not, I do this because it causes an error in my logs, and I don't use the evelate feature).
* Add a request handler for the data import handler:

```xml
<requestHandler name="/dataimport" class="org.apache.solr.handler.dataimport.DataImportHandler">
	<lst name="defaults">
		<str name="config">data-config.xml</str>
	</lst>
</requestHandler>
```

* Let Tomcat know where the your cores are.  Using the Tomcat Monitor, add `-Dsolr.solr.home=C:\Java\solr-x.x.x\cores` to Java options.

Going forward, when you need another core:

* Make a copy of `collection1`
* Rename it
* Update core.properties with the new name.
* Restart Apache Tomcat

### SOLR / Apache Tomcat Setup

#### Logging
Copy the jars in `C:\Java\solr-x.x.x\example\lib\ext` to `C:\Program Files\Apache Software Foundation\Tomcat x.x\lib`

#### WAR Deployment
Copy the war (e.g. `c:\Java\Solr-x.x.x\dist\solr-x.x.x.war`) into Tomcat webapps folder (e.g. `C:\Program Files\Apache Software Foundation\Tomcat x.x\webapps`).

#### Data Import Handler for SQL Server
* Copy `C:\Java\solr-x.x.x\dist\solr-dataimporthandler-x.x.x.jar` into `C:\Program Files\Apache Software Foundation\Tomcat x.x\webapps\solr-x.x.x\WEB-INF\lib`.
* Copy jars from `C:\Java\solr-x.x.x\contrib\dataimporthandler\lib` into `C:\Program Files\Apache Software Foundation\Tomcat x.x\webapps\solr-x.x.x\WEB-INF\lib`. (note: there may not be any jars in here)
* Download Microsoft's JDBC driver for SQL Server and copy `sqljdbc4.jar` into `C:\Program Files\Apache Software Foundation\Tomcat x.x\webapps\solr-x.x.x\WEB-INF\lib`.

### Restart and Verify
Using Tomcat Monitor, restart the service.  Check [http://localhost:8080/solr-x.x.x](http://localhost:8080/solr-x.x.x) to make sure SOLR is working..

### Trouble-shooting
If anything goes wrong, use the SOLR and Tomcat logs (e.g. `C:\Program Files\Apache Software Foundation\Tomcat x.x\logs`).

### Updates
A lot has changed in SOLR 5.*.  No more war file.  Check out this [post](http://www.norconex.com/how-to-run-solr5-as-a-service-on-windows) to 
see how to install SOLR 5 as a service on Windows.
