## Transformalize

This tool expedites mundane data processing tasks
like cleaning, reporting, and [denormalization](https://en.wikipedia.org/wiki/Denormalization).

It works with many data sources:

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
            <td>Microsoft SQL Server</td>
            <td>&#10004;</td>
            <td>&#10004;</td>
        </tr>
        <tr>
            <td>MySql</td>
            <td>&#10004;</td>
            <td>&#10004;</td>
        </tr>
        <tr>
            <td>PostgreSql</td>
            <td>&#10004;</td>
            <td>&#10004;</td>
        </tr>
        <tr>
            <td>SQLite</td>
            <td>&#10004;</td>
            <td>&#10004;</td>
        </tr>
        <tr>
            <td>Files</td>
            <td>&#10004;</td>
            <td>&#10003;</td>
        </tr>
        <tr>
            <td>Web</td>
            <td>&#10003;</td>
            <td> </td>
        </tr>
        <tr>
            <td>Elasticsearch</td>
            <td>&#10003;</td>
            <td>&#10003;</td>
        </tr>
        <tr>
            <td>SOLR</td>
            <td>&#10003;</td>
            <td></td>
        </tr>
        <tr>
            <td>Lucene</td>
            <td>&#10003;</td>
            <td>&#10003;</td>
        </tr>
    </tbody>
</table>

### Hello World

Design jobs in [XML](https://en.wikipedia.org/wiki/XML)
or [JSON](https://en.wikipedia.org/wiki/JSON) 
and execute with a [CLI](https://en.wikipedia.org/wiki/Command-line_interface).

```xml
<add name="Process">
    <entities>
        <add name="Entity">
            <rows>
                <add Noun="World" />
                <add Noun="Earth" />
            </rows>
            <fields>
                <add name="Noun" output="false" />
            </fields>
            <calculated-fields>
                <add name="Greeting" t="copy(Noun).format(Hello {0})" />
            </calculated-fields>
        </add>
    </entities>
</add>
```

Save as *HelloWorld.xml*. Running this reads it's rows and 
writes to the console.  

<pre>
<strong>tfl -a HelloWorld.xml</strong>
Greeting
Hello World
Hello Earth
</pre>

---
#### Hello File

Take this file (partially listed below):

<pre>
<strong>Planet,Distance,Year,Mass,Day,Diameter,Gravity</strong>
Mercury,0.39,0.24,0.055,1407.6,3.04,0.37
Venus,0.72,0.61,0.815,5832.2,7.52,0.88
Earth,1,1,1,24.0,7.92,1
...
</pre>

Arrange *HelloFile.xml* like this:

```xml
<add name="Process">
    <connections>
        <add name="input" provider="file" file="c:\temp\Planets.csv" delimiter="," />
    </connections>
    <entities>
        <add name="input">
            <fields>
                <add name="Planet" />
                <add name="Distance" />
                <add name="Year" />
                <add name="Mass" />
                <add name="Day" />
                <add name="Diameter" />
                <add name="Gravity" />
            </fields>
            <calculated-fields>
                <add name="Greeting" t="copy(Planet).format(Hello {0})" />
            </calculated-fields>
        </add>
    </entities>
</add>
```

Run...

<pre>
<strong>tfl -a HelloFile.xml</strong>
Planet,Distance,Year,Mass,Day,Diameter,Gravity,Greeting
Mercury,0.39,0.24,0.055,1407.6,3.04,0.37,Hello Mercury
Venus,0.72,0.61,0.815,5832.2,7.52,0.88,Hello Venus
Earth,1,1,1,24.0,7.92,1,Hello Earth
...
</pre>

---
#### Hello Database

*todo: write Hello Database example...*

---
### Build Notes

1. Use Visual Studio 2015+.
2. Add SolrNet package source: https://ci.appveyor.com/nuget/solrnet-022x5w7kmuba
3. Copy the dlls from the *x86* or *x64* folder to where *tfl.exe* is.

**NOTE**: This is the 2nd implementation.  To find out more Transformalize,
read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind)
I posted to Code Project (based on the 1st implementation).