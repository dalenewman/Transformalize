# Transformalize
Transformalize is released under the Apache 2 license.  Note: It is still under development.

## What is it?
Transformalize is a configurable [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load)
solution. It is used to prepare data for [data warehouses](https://en.wikipedia.org/wiki/Data_warehouse),
[search engines](https://en.wikipedia.org/wiki/Search_engine_%28computing%29), services, reports, and 
other forms of analysis and/or presentation.  It comes with a CLI (`tfl.exe`) and an [Orchard CMS](https://github.com/OrchardCMS/Orchard) 
web module

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
TFL arrangements are maintained in an [XML](https://en.wikipedia.org/wiki/XML) or
[JSON](https://en.wikipedia.org/wiki/JSON) editor.  You may also edit and store 
arrangements with the Orchard CMS module (as seen below):

![Edit in Orchard CMS](Files/edit-hello-world-in-orchard-cms.png)

---

### Hello World

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
        <add name="Combined" t="copy(Name,Code).format(Hello {0} ({1}))" />
      </calculated-fields>
    </add>
  </entities>
</cfg>
```

I have this running on [transformalize.com](http://www.transformalize.com), so you can see the output of this arrangement in [report](http://www.transformalize.com/Pipeline/Report/24) mode or [xml](http://www.transformalize.com/Pipeline/Api/Run/24), or [json](http://www.transformalize.com/Pipeline/Api/Run/24?format=json).

*to be continued...*

**NOTE**: This code-base is the 2nd implementation.  To find out more about
how Transformalize works, you can read the [article](http://www.codeproject.com/Articles/658971/Transformalizing-NorthWind)
I posted to Code Project (based on the 1st implementation).




