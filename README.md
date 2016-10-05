This tool expedites mundane data processing tasks 
like denormalization and reporting.








### Data Sources - Inputs and Outputs

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

### <a name="CFG"></a>Configurable
TFL arrangements are designed in an [XML](https://en.wikipedia.org/wiki/XML) or
[JSON](https://en.wikipedia.org/wiki/JSON) editor.  One is included in the Orchard CMS module (as seen below):

![Edit in Orchard CMS](Files/edit-hello-world-in-orchard-cms.png)

---

### Hello World

This first example reads data from a url:

```xml
<cfg name="Hello World">
  <connections>
    <add name="input" 
         provider="web" 
         url="http://www.transformalize.com/Pipeline/File/View/25"
         delimiter=","
         start="2" />
  </connections>
  <entities>
    <add name="Countries" page="1">
      <fields>
        <add name="Code" primary-key="true" />
        <add name="Name" />
      </fields>
      <calculated-fields>
        <add name="Combined">
            <transforms>
                <add method="format" format="Hello {0} ({1})">
                    <parameters>
                        <add name="Code" />
                        <add name="Name" />
                    </parameters>
                </add>
            </transforms>
        </add>
      </calculated-fields>
    </add>
  </entities>
</cfg>
```

This example introduces **connections**, and **entities**.
The connection points to a [delimited file with country names and codes](http://www.transformalize.com/Pipeline/File/View/25).  The file 
is hosted on my website.  The entity describes the data.  In this case; two 
fields: `Code`, and `Name`.
 
To demonstrate a transform, a calculated field combines `Code` and `Name`.

This is running on my Orchard CMS website [transformalize.com](http://www.transformalize.com). You may 
view the output in different ways:

* in [report](http://www.transformalize.com/Pipeline/Report/24) mode
* as [xml](http://www.transformalize.com/Pipeline/Api/Run/24)
* as [json](http://www.transformalize.com/Pipeline/Api/Run/24?format=json) 

*to be continued...*

**NOTE**: This code-base is the 2nd implementation.  To find out more about
how Transformalize works, you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind)
I posted to Code Project (based on the 1st implementation).




