### Overview

This adds a `lucene` provider to Transformalize using [Lucene.Net](https://lucenenet.apache.org).

### Write Usage

```xml
<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='lucene' folder='c:\temp\bogus-lucene-index' />
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

This writes 1000 rows of bogus data to lucene 4.* index at *c:\temp\bogus-lucene-index\Contact*.

### Read Usage

```xml
<add name='TestProcess' >
  <connections>
    <add name='input' provider='lucene' folder='c:\temp\bogus-lucene-index' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' />
        <add name='Reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>
```

This reads 1000 rows of bogus data from the Lucene 4.* index at *c:\temp\bogus-lucene-index\Contact*:

<pre>
<strong>FirstName,LastName,Stars,Reviewers</strong>
Justin,Konopelski,3,153
Eula,Schinner,2,41
Tanya,Shanahan,4,412
Emilio,Hand,4,469
Rachel,Abshire,3,341
Doyle,Beatty,4,458
Delbert,Durgan,2,174
Harold,Blanda,4,125
...
</pre>