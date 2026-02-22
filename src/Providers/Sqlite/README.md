### Overview

This is a `sqlite` provider to Transformalize using [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core). You may have to add this package to your final application in order to get the right platform SQLite interop dll.

### Write Usage

```xml
<add name='Bogus' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='sqlite' file='junk.sqlite3' />
  </connections>
  <entities>
    <add name='Contact' size='1000'>
      <fields>
        <add name='Identity' type='int' primary-key='true' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>
```

This writes 1000 rows of bogus data to a Sqlite database.

### Read Usage

```xml
<add name='BogusRead' read-only='true' >
  <connections>
    <add name='input' provider='sqlite' file='junk.sqlite3' />
  </connections>
  <entities>
    <add name='BogusStar' page='1' size='10'>
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

This reads 10 rows of bogus data from a Sqlite database:

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
