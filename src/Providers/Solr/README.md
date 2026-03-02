### Overview

This adds a `Solr` provider to Transformalize using [SolrNet](https://github.com/SolrNet/SolrNet).

### Write Usage

```xml
<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' 
         provider='solr' 
         core='bogus' 
         folder='c:\temp\mysolrhome' 
         path='solr' 
         port='8983' 
         version='7.7.3' 
         max-degree-of-parallelism="2" 
         request-timeout="120" />
  </connections>
  <entities>
    <add name='Contact' size='1000'>
      <fields>
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>
```

This writes 1000 rows of bogus data to Solr.

### Read Usage

```xml
<add name='TestProcess' >
  <connections>
    <add name='input' provider='solr' core='bogus' folder='c:\temp\mysolrhome' path='solr' port='8983' />
  </connections>
  <entities>
    <add name='Contact' page='1' size='10'>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>
```

This reads 10 rows of bogus data from Solr:

<pre>
<strong>firstname,lastname,stars,reviewers</strong>
Justin,Konopelski,3,153
Eula,Schinner,2,41
Tanya,Shanahan,4,412
Emilio,Hand,4,469
Rachel,Abshire,3,341
Doyle,Beatty,4,458
Delbert,Durgan,2,174
Harold,Blanda,4,125
Willie,Heaney,5,342
Sophie,Hand,2,176</pre>

Or you could just look at it in SOLR admin:

<img width="553" alt="image" src="https://user-images.githubusercontent.com/933086/186578326-e7dbaa1f-3906-4044-ac25-b6228f9cd198.png">

### Notes

- Only tested with Solr 7.7.3 (using Docker) so far.
- Field names go into Solr as lower case.

### ☀️ SOLR Docker Commands

```
# windows
docker run -p 8983:8983 -v %cd%/mysolrhome:/mysolrhome -e SOLR_HOME=/mysolrhome -e INIT_SOLR_HOME=yes -t solr:7.7.3
# linux
docker run -p 8983:8983 -v $PWD/mysolrhome:/mysolrhome -e SOLR_HOME=/mysolrhome -e INIT_SOLR_HOME=yes -t solr:7.7.3
```

**Note**: I ran ☝️ from _c:\temp_ on Windows which is why `folder` is set to _c:\temp\mysolrhome_ above.
