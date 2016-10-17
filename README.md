## Transformalize

### Intro
This tool expedites mundane data processing tasks
like reporting and [denormalization](https://en.wikipedia.org/wiki/Denormalization).

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

### Configuration

Jobs are designed in [XML](https://en.wikipedia.org/wiki/XML)
or [JSON](https://en.wikipedia.org/wiki/JSON).
They are executed with a provided [CLI](https://en.wikipedia.org/wiki/Command-line_interface).

#### Hello World:

```xml
<add name="Process">
    <entities>
        <add name="Entity">
            <rows>
                <add Noun="World" />
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

Save the above in *HelloWorld.xml* and run like this:

```shell
> tfl -a HelloWorld.xml
Greeting
Hello World
```

The `csv` output includes the header column name; `Greeting`, and a single row; `Hello World`.

#### Hello Planets

Hello Planets demonstrates reading from a file called *HelloPlanets.csv*.  It contains:

```shell
Planet,Distance,Year,Mass,Day,Diameter,Gravity
Mercury,0.39,0.24,0.055,1407.6,3.04,0.37
Venus,0.72,0.61,0.815,5832.2,7.52,0.88
Earth,1,1,1,24.0,7.92,1
...
```

Here is the arrangement we start with:

```xml
<add name="Process">
    <connections>
        <add name="input" provider="file" file="c:\temp\Planets.csv" />
    </connections>
</add>
```

The (above) is only the connection.  It needs an entity with
fields but I don't want to type that.  So, I use the
CLI:

```shell
c:\> tfl -a c:\temp\HelloPlanets.xml -m check
```

`Check` mode detects and returns the schema so I can add it to my arrangement.

```xml
<add name="Process">
    <connections>
        <add name="input" provider="file" file="c:\temp\Planets.csv" />
    </connections>
    <entities>
        <add name="input">
            <fields>
                <add name="Planet" length="8" />
                <add name="Distance" length="6" />
                <add name="Year" length="7" />
                <add name="Mass" length="7" />
                <add name="Day" length="7" />
                <add name="Diameter" length="5" />
                <add name="Gravity" length="5" />
            </fields>
        </add>
    </entities>
</add>
```

*to be continued...*

**NOTE**: This is the 2nd implementation.  To find out more Transformalize,
read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind)
I posted to Code Project (based on the 1st implementation).




