### Overview

This is an `Elasticsearch` provider for [Transformalize](https://github.com/dalenewman/Transformalize) 
using [Elasticsearch.Net](https://github.com/elastic/elasticsearch-net). 
It has been tested and works with Elasticsearch 7 through 8.

#### 🎱 Elasticsearch v8 Changes

Security is on by default in version 8.  This includes user, password, and SSL.  Therefore, you may need to add 
these properties to your connection:

- `use-ssl`: true|false and is used to build the URL if you are using `server` instead of `url`.
- `certificate-fingerprint`: output on first ES start.
- `user`: output on first ES start (defaults to _elastic_)
- `password`: output on first ES start

#### Current Issues/Constraints
- Every identifier is lower-cased before going into Elasticsearch.
- If you don't specify a three part version in your connection you could default an incorrect version and experience issues.
 
### Write Usage

```xml
<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='elasticsearch' index='bogus' server='localhost' port='9200' version='7.9.3' />
    <!-- example 8
    <add name='output' provider='elasticsearch' index='bogus' server='localhost' port='9200' version='8.3.2' user='elastic' password='Zgf4+hQ7+dmCnIUpk6Wr' use-ssl='true' certificate-fingerprint='37:84:43:71:D1:57:AE:34:9D:FE:FB:2A:A5:E2:DC:65:7B:62:16:9C:8E:E3:58:DB:30:EF:CD:9E:06:85:7D:03' />
    -->
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

This writes 1000 rows of bogus data.

### Read Usage

```xml
<add name='TestProcess' >
  <connections>
    <add name='input' provider='elasticsearch' index='bogus' port='9200' version='7.9.3' />
  </connections>
  <entities>
    <add name='contact' page='1' size='10'>
      <order>
        <add field='identity' />
      </order>
      <fields>
        <add name='identity' type='int' />
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>
```

This reads 10 rows of bogus data:

<pre>
<strong>identity,firstname,lastname,stars,reviewers</strong>
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

If using a multi-node cluster, you may specify servers in your connection with `url` or (`name` and `port`) attributes:

```xml
<connections>
   <add name="x" provider="elasticsearch" index="y" version="7.9.3">
      <servers>
         <add url="http://node1:9200" />
         <add url="http://node2:9200" />
      </servers>
   </add>
</connections>
```