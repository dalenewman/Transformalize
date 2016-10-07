## Transformalize

### Intro
This tool expedites mundane data processing tasks
like denormalization and reporting.

It works with multipe data sources:
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

Here is Hello World:

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

Run:

```bash
> tfl -a HelloWorld.xml
Greeting
Hello World
```

*to be continued...*

**NOTE**: This is the 2nd implementation.  To find out more Transformalize,
read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind)
I posted to Code Project (based on the 1st implementation).




