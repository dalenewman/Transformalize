This is a [Transformalize](https://github.com/dalenewman/Transformalize) file provider 
using [CsvHelper](https://joshclose.github.io/CsvHelper/) to read/write files.

### Write Usage

```xml
<add name='BogusWrite' read-only='true'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='file' file='c:\temp\bogus.csv' synchronous='true' />
  </connections>
  <entities>
    <add name='Contact' size='1000'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>
```

This writes 1000 rows of bogus data to a file. ⚠️ Presently you have to add `synchronous='true'`.

### Read Usage

```xml
<add name='BogusRead' >
  <connections>
    <add name='input' provider='file' file='c:\temp\bogus.csv' synchronous='true' />
  </connections>
  <entities>
    <add name='Bogus' page='1' size='10'>
      <order>
        <add field='Identity' />
      </order>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' />
        <add name='Reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>
```

This reads 10 rows of bogus data from a the file:

<pre>
<strong>Identity,FirstName,LastName,Stars,Reviewers</strong>
1,Justin,Konopelski,3,153
2,Eula,Schinner,2,41
3,Tanya,Shanahan,4,412
4,Emilio,Hand,4,469
5,Rachel,Abshire,3,341
6,Doyle,Beatty,4,458
7,Delbert,Durgan,2,174
8,Harold,Blanda,4,125
9,Willie,Heaney,5,342
10,Sophie,Hand,2,176</pre>