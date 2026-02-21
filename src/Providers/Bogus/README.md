This is a [bogus](https://github.com/bchavez/Bogus) input provider for [Transformalize](https://github.com/dalenewman/Transformalize).

It is still a work in progress.  Some of the [Bogus API Support](https://github.com/bchavez/Bogus/blob/master/README.md#bogus-api-support) is 
implemented.  You can use Transformalize arrangements to generate test data 
like this:

```xml
<cfg name="Bogus">

    <connections>
        <add name="input" provider="bogus" seed="1" />
    </connections>

    <entities>
        <add name="Contact" size="5">
            <fields>
                <add name="Identity" type="int" />
                <add name="FirstName" />
                <add name="LastName" />
                <add name="Email" />
                <add name="Phone" format="(###)###-####" />
                <add name="Stars" type="byte" min="1" max="5" />
                <add name="Recent" alias="LastSeen" type="datetime" />
            </fields>
        </add>
    </entities>

</cfg>
```

Saving this as *bogus.xml* and running produces:

<pre>
<i>c:\> tfl bogus.xml</i>
<strong>Identity,FirstName,LastName,Email,Phone,Stars,LastSeen</strong>
1,Delores,Brown,Delores.Brown@yahoo.com,(460)120-3539,5,11/20/2017 10:10:14 PM
2,Dakota,Bradtke,Dakota_Bradtke@hotmail.com,(102)209-4891,5,11/21/2017 12:37:18 AM
3,Tanner,Becker,Tanner8@hotmail.com,(682)933-1094,5,11/21/2017 11:59:27 AM
4,Ila,Schamberger,Ila28@gmail.com,(010)148-1661,2,11/21/2017 12:55:25 AM
5,Darren,Ledner,Darren_Ledner@gmail.com,(246)962-0037,4,11/21/2017 3:59:14 AM
</pre>

If the field's `name` is on the [Bogus API](https://github.com/bchavez/Bogus/blob/master/README.md#bogus-api-support), 
fake data is created.

In addition, the `format`, `min`, and `max` attributes 
are used to create other forms of test data (e.g. the `Stars` field above).
