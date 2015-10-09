# Fantasy Football Review with Kibana

In this article, I demonstrate how to view the past few years of Fantasy Football statistics on a [Kibana](http://www.elasticsearch.org/overview/kibana) dashboard. More specifically, I document:

* [Finding _the Data_](#FindingTheData)
* [Profiling _the Data_](#ProfilingTheData)
* [Loading _the Data_](#LoadingTheData) into [Elasticsearch](http://www.elasticsearch.org) (which is a Kibana requirement)
* [Viewing _the Data_](#ViewingTheData) (including dashboard configuration)

<a id="FindingTheData" name="FindingTheData"></a>

### Finding the Data

I Googled, and found some usable statistics on the website: [Pro Football Reference](http://www.pro-football-reference.com). The annual links are [2011](http://www.pro-football-reference.com/years/2011/fantasy.htm), [2012](http://www.pro-football-reference.com/years/2012/fantasy.htm), and [2013](http://www.pro-football-reference.com/years/2013/fantasy.htm). Clicking on the CSV, or Export link provides a comma delimited format like this:

<img alt="Stats" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/Stats640.png" />

---

The only problem with this data is that the column headers are two lines and they repeat every 29 records. So, I combine them and remove the repeats. I put _Fantasy2011.txt_, _Fantasy2012.txt_, and _Fantasy2013.txt_ in a folder called _c:\temp\Fantasy_.

<a id="ProfilingTheData" name="ProfilingTheData"></a>
### Profile the Data

When dealing with new data, it&#39;s a good idea to profile it. To do this, I use an open source tool called [Data Profiler](https://github.com/dalenewman/DataProfiler). It comes with a command line tool: _Dp.exe_. I give it a file name argument, and it profiles the data and opens a summary in my web browser.

`dp.exe c:\temp\Fantasy2011.txt`

---

<img alt="Data Profiler" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/DataProfiler640.png" />

At a glance:

*   I see I have just 4 positions (QB, RB, TE, and WR).
*   I don&#39;t recognize VBD. So, I Google it to find out it stands for _Value Based Drafting. _It may help rank players. Here&#39;s an explanation from [ESPN](http://sports.espn.go.com/fantasy/football/ffl/story?page=nfldk2k12_vbdwork):

> _&quot;Simply put, VBD developed as a way to retroactively compare the fantasy usefulness of players at different positions. It seeks to establish a &quot;baseline&quot; player at each position, and then measure the degree to which the best players at each position exceeded their respective baselines. It may have a fancy acronym, but it&#39;s the furthest thing imaginable from fancy. In fact, it&#39;s kind of a ramshackle, kludgy tool that may get more credit for prescience than it deserves. But I still really like it.&quot;_

*   The fact that there are 564 ranks and only 561 names suggests there may be duplicates, but for my purposes it&#39;s no big deal.
*   There are many numeric data types, which is good for aggregation on dashboards.

Overall, the data looks clean and ready to use.

<a id="LoadingTheData" name="LoadingTheData"></a>

### Loading the Data

To load the data from files into Elasticsearch, I use an open source tool called [Transformalize](https://github.com/dalenewman/Transformalize). Transformalize needs a configuration.

I setup [NotePad++](http://notepad-plus-plus.org/) to create and execute the Transformalize process. You can see how to execute commands with NotePad++ [here](http://stackoverflow.com/questions/4614608/execute-commands-with-notepad). I create this and save it as _Fantasy.xml_.

```xml
<transformalize>
    <processes>
        <add name="Fantasy">
            <connections>
                <add name="input"
                     provider="folder"
                     folder="c:\temp\fantasy"
                     search-pattern="Fantasy*.txt"
                     delimiter=","
                     start="2" />
            </connections>
            <entities>
                <add name="Stats"/>
            </entities>
        </add>
    </processes>
</transformalize>
```

The configuration above defines a process named `Fantasy`. It includes a file input connection, and an entity named `Stats`. In this case, `Stats` represents the _Fantasy*.txt_ files. In order to import the files, I have to define their fields (as seen above in the data profile). To do this quickly, I run Transformalize in `metadata` mode like so:

`tfl.exe c:\temp\Fantasy\Fantasy.xml {'mode':'metadata'}`

In `metadata` mode, Transformalize generates and opens a file called _Metadata.xml_. It contains the file&#39;s field definitions. I copy-paste them into _Fantasy.xml _like this:

```xml
<transformalize>
    <processes>
        <add name="Fantasy">
            <connections>
                <add name="input"
                     provider="folder"
                     folder="c:\temp\fantasy"
                     search-pattern="Fantasy*.txt"
                     delimiter=","
                     start="2" />
            </connections>
            <entities>
                <add name="Stats">
                    <fields>
                        <add name="Rank" type="int" ></add>
                        <add name="Name" length="23" ></add>
                        <add name="Team" length="4" ></add>
                        <add name="Age" type="int" ></add>
                        <add name="Games" type="int" ></add>
                        <add name="GamesStarted" type="int" ></add>
                        <add name="PassingCmp" type="int" ></add>
                        <add name="PassingAtt" type="int" ></add>
                        <add name="PassingYds" type="int" ></add>
                        <add name="PassingTD" type="int" ></add>
                        <add name="PassingInt" type="int" ></add>
                        <add name="RushingAtt" type="int" ></add>
                        <add name="RushingYds" type="int" ></add>
                        <add name="RushingYA" type="single" ></add>
                        <add name="RushingTD" type="int" ></add>
                        <add name="ReceivingRec" type="int" ></add>
                        <add name="ReceivingYds" type="int" ></add>
                        <add name="ReceivingYR" type="single" ></add>
                        <add name="ReceivingTD" type="int" ></add>
                        <add name="FantPos" length="3" ></add>
                        <add name="FantPoints" type="int" ></add>
                        <add name="VBD" type="int" ></add>
                        <add name="PosRank" type="int" ></add>
                        <add name="OvRank" type="int" ></add>
                    </fields>
                </add>
            </entities>
        </add>
    </processes>
</transformalize>
```

With the fields defined, I execute Transformalize to test the import process:

`tfl.exe c:\temp\Fantasy\Fantasy.xml`

It produces log output like this:

```bash
00:14:13 | Info | Fantasy | All..... | Detected configuration update.
00:14:14 | Warn | Fantasy | All..... | No output connection detected.  Defaulting to internal.
00:14:14 | Info | Fantasy | All..... | Adding TflHashCode primary key for Stats.
00:14:14 | Info | Fantasy | Stats... | Reading Fantasy2011.txt
00:14:14 | Info | Fantasy | Stats... | Completed 564 rows in ConcatOperation (TflHashCode): 0 seconds.
00:14:14 | Info | Fantasy | Stats... | Completed 564 rows in GetHashCodeOperation (TflHashCode): 0 seconds.
00:14:14 | Info | Fantasy | Stats... | Completed 564 rows in TruncateOperation: 0 seconds.
00:14:14 | Info | Fantasy | Stats... | Processed 0 inserts, and 0 updates in Stats.
00:14:14 | Info | Fantasy | All..... | Process affected 0 records in 00:00:00.2831019.
```

This is normal Transformalize output. It confirms I am able to import 564 rows from the file.

At this point, it doesn&#39;t store them. Furthermore, it only loads one file (not three). So, I add an output connection to Elasticsearch for storage, and change the input connection from a `file` provider to a `folder` provider. I set a search pattern to import all three _Fantasy*.txt_ files. Here are the updated connections:

```xml
<connections>
    <add name="input"
         provider="folder"
         folder="c:\temp\fantasy"
         search-pattern="Fantasy*.txt"
         delimiter=","
         start="2" />
    <add name="output"
         provider="elasticsearch"
         server="localhost"
         port="9200" />
</connections>
```

#### Elasticsearch Setup

The folks at Elasticsearch have made it very easy to setup. All you do is:

*   Have a Java runtime
*   Set your `JAVA_HOME `variable
*   Download Elasticsearch
*   Decompress it
*   Switch to its _bin_ folder and run `elasticsearch`

It even comes with a slick utility to install and run a 32-bit or 64-bit service.

---

Transformalize needs to initialize the Elasticsearch output. To do this, it:

*   Creates an index named after the process: `fantasy`
*   Creates a type mapping named and modeled after the entity: `stats`.

By default, Elasticsearch will use the `standard` full-text analyzer on text. Instead, I want to use the `keyword` analyzer. So, underneath the `connections` collection, I add this:

```xml
<search-types>
    <add name="default" analyzer="keyword" />
</search-types>
```

The `keyword` analyzer treats all the text like single terms, even if there is more than one word. This is what I want, since I plan to navigate the data with Kibana, rather than search it.

I am ready to initialize the output, so I run Transformalize in `init` mode.

`tfl.exe c:\temp\Fantasy\Fantasy.xml {'mode':'init'}`

It produces a brief output log:

```bash
00:35:14 | Info | Fantasy | All.... | Adding TflHashCode primary key for Stats.
00:35:14 | Info | Fantasy | All.... | Initialized TrAnSfOrMaLiZeR output connection.
00:35:14 | Info | Fantasy | All.... | Initialized output in 00:00:00.3962501.
```

The log output _says_ it initialized the output, but I need proof, so I check for the mapping using my browser. I indicate the index (fantasy) and type name (stats) and specify `_mapping` like this:

<pre class="prettyprint">
http://localhost:9200/fantasy/stats/_mapping
</pre>

Elasticsearch returns a JSON response like this:

```js
{
    "fantasy" : {
        "mappings" : {
            "stats" : {
                "properties" : {
                    "age" : {
                        "type" : "integer"
                    },
                    "fantpoints" : {
                        "type" : "integer"
                    },
                    "fantpos" : {
                        "type" : "string",
                        "analyzer" : "keyword"
                    },
                    "games" : {
                        "type" : "integer"
                    },
                    "gamesstarted" : {
                        "type" : "integer"
                    },
                    "name" : {
                        "type" : "string",
                        "analyzer" : "keyword"
                    },
                    "ovrank" : {
                        "type" : "integer"
                    },
                    "passingatt" : {
                        "type" : "integer"
                    },
                    "passingcmp" : {
                        "type" : "integer"
                    },
                    "passingint" : {
                        "type" : "integer"
                    },
                    "passingtd" : {
                        "type" : "integer"
                    },
                    "passingyds" : {
                        "type" : "integer"
                    },
                    "posrank" : {
                        "type" : "integer"
                    },
                    "rank" : {
                        "type" : "integer"
                    },
                    "receivingrec" : {
                        "type" : "integer"
                    },
                    "receivingtd" : {
                        "type" : "integer"
                    },
                    "receivingyds" : {
                        "type" : "integer"
                    },
                    "receivingyr" : {
                        "type" : "double"
                    },
                    "rushingatt" : {
                        "type" : "integer"
                    },
                    "rushingtd" : {
                        "type" : "integer"
                    },
                    "rushingya" : {
                        "type" : "double"
                    },
                    "rushingyds" : {
                        "type" : "integer"
                    },
                    "team" : {
                        "type" : "string",
                        "analyzer" : "keyword"
                    },
                    "tflbatchid" : {
                        "type" : "long"
                    },
                    "tflhashcode" : {
                        "type" : "integer"
                    },
                    "vbd" : {
                        "type" : "integer"
                    }
                }
            }
        }
    }
}
```

The mapping looks right. So I run Transformalize in regular (default) mode:

`tfl.exe c:\temp\Fantasy\Fantasy.xml`

---

```bash
00:49:20 | Info | Fantasy | All..... | Adding TflHashCode primary key for Stats.
00:49:21 | Info | Fantasy | Stats... | Reading Fantasy2011.txt
00:49:21 | Info | Fantasy | Stats... | Reading Fantasy2012.txt
00:49:21 | Info | Fantasy | Stats... | Reading Fantasy2013.txt
00:49:21 | Info | Fantasy | Stats... | Completed 1740 rows in ConcatOperation (TflHashCode): 1 second.
00:49:21 | Info | Fantasy | Stats... | Completed 1740 rows in GetHashCodeOperation (TflHashCode): 1 second.
00:49:21 | Info | Fantasy | Stats... | Completed 1740 rows in TruncateOperation: 1 second.
00:49:21 | Info | Fantasy | Stats... | Processed 1740 inserts, and 0 updates in Stats.
00:49:21 | Info | Fantasy | All..... | Process affected 1740 records in 00:00:01.1119670.
```

It appears to have loaded everything correctly. I&#39;ll do a quick check in Elasticsearch. This time I specify the **_search** action, a query for everything (q=\*:\*) and a size of zero, indicating I don&#39;t want any rows returned:

`http://localhost:9200/fantasy/stats/_search?q=*:*&amp;size=0`

---

```js
{
    "took" : 1,
    "timed_out" : false,
    "_shards" : {
        "total" : 5,
        "successful" : 5,
        "failed" : 0
    },
    "hits" : {
        "total" : 1740,
        "max_score" : 0.0,
        "hits" : []
    }
}
```

Yeap, 1740 hits. This matches the Transformalize import.

<a id="ViewingTheData" name="ViewingTheData"></a>

### Viewing the Data

#### Kibana Setup

Kibana is a client-side web application. It is all HTML, JavaScript, and CSS. Put it on a web server and you&#39;re ready to go. I put the latest version in my _c:\inetpub\wwwroot\kibana_ folder and browse to it.

---

<img alt="Kibana 3 Introduction" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/Kibana3Introduction640.png" />

---

I click on the Blank Dashboard link, then the cog (settings) button. I just set the index for now:

<img alt="Kibana 3 Settings Index" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/DashboardSettingsIndex640.png" class="img-responsive img-thumbnail" />

---

Then, I add a row with a table panel. This will let me see the data points:

<img alt="Kibana 3 Table" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/Kibana3Table640.png" />

I can see right away that the year is missing. Moreover, I don&#39;t want the asterisk and plus sign next to the name.

#### Loading the Data (again)

Because I want new fields and want to modify the `name` field, I need to re-visit my Transformalize configuration.

As it happens, when you load files with Transformalize, it adds a `TflFileName` to the row. So, I can grab the year from the file name. I&#39;ll go one step further and make an actual date out of it as well, because I know Kibana requires dates in some panels.

```xml
<fields>
    <add name="Rank" type="int" ></add>
    <add name="Name" length="24" >
        <transforms>
            <add method="trimend" trim-chars="*+" />
        </transforms>
    </add>
    ...
</fields>
<calculated-fields>
    <add name="Year" type="int">
        <transforms>
            <add method="trimend" trim-chars=".txt" parameter="TflFileName" />
            <add method="right" length="4" />
            <add method="convert" to="int" />
        </transforms>
    </add>
    <add name="YearDate" type="datetime">
        <transforms>
            <add method="csharp" script="return new DateTime(Year,1,1);" parameter="Year" />
        </transforms>
    </add>
</calculated-fields>
...
```

Here is an outline of the transformations added above:

* **Name**
  * Trim off the `*` and `+` characters.
* **Year**
  * Trim off the extension _.txt_ from `TflFileName`
     * `"c:\temp\fantasy\Fantasy2011.txt"` becomes `"c:\temp\fantasy\Fantasy2011"`
  * Take the right 4 characters.
     * `"c:\temp\fantasy\Fantasy2011"` becomes `"2011"`
  * Convert it to an integer.
     * `"2011"` becomes _2011_
* **YearDate**
  * Use the newly created `Year` and C# code to create a date.
     * `2011` becomes `2011-01-01`

Now that I&#39;ve cleaned up the data, I go back to the dashboard and set the timepicker to use `yeardate`.

<img alt="Kibana 3 Time Picker" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/SetTimePicker640.png" />

---

<img alt="Kibana 3 Table" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/WithHistogram640.png" />

---

Before I forget, I want to mention that each row in the table panel can expand to show all the data:

<img alt="Kibana 3 Table Detail" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/TableDetail640.png" />

Seeing the data gives me ideas. I decide to make some `term` charts for year, age, team, and position:

<img alt="Kibana 3 Terms Widget" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/TermsWidget640.png" />

---

<img alt="Kibana 3 Charts" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/Charts640.png" />

---

The cool thing about most of the panels is that they are interactive. In the charts above, it appears that New Orleans is the highest scoring team and 25 is the optimal age. But, if I click on the RB position, things change. Apparently running backs do better when they are 26 years old:

<img alt="Kibana 3 Interactive" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/Interactive640.png" />

After you click on the panel&#39;s charts, you can always remove the filter from the green &quot;Filtering&quot; section. As a final example, here are a couple ways to look at players (by points or VBD):

<img alt="Kibana 3 Terms by Players" class="img-responsive img-thumbnail" src="http://www.codeproject.com/KB/web-image/791182/ByPlayers640.png" />

Hovering over the bars shows their name. The main reason I chose the `keyword` analyzer is so these show up as full names. The `standard` analyzer would break them into first and last names. In that case, the last name `Johnson` may have done well because of Calvin and Chris&#39; high numbers, but this doesn&#39;t help us distinguish the best players.

I&#39;ve only demonstrated the term and table panels above. The dashboard gets even better if you have many time stamped events occurring in real time on the `histogram` panel, and/or you have longitude and latitude to plot their location on the `bettermap` panel. What&#39;s more, the dashboard is responsive to your screen size, so you can use big screen monitors, or your cell phones to view it.

### Conclusion

With the help of four freely available tools, I demonstrated how to get a visual look at the last three seasons of Fantasy Football statistics. There wasn&#39;t any coding involved; just configuration. An important take away is that what I demonstrated **may be applied to any structured data**. You&#39;d be amazed how a few charts can make your users so happy :-)