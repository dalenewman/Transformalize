This adds several methods to Transformalize using [Humanizer](https://github.com/Humanizr/Humanizer).  It is a plug-in compatible with Transformalize 0.3.0-beta.

Build the Autofac project and put it's output into Transformalize's *plugins* folder.

Use like this:

```xml
<cfg name="Test">
    <entities>
        <add name="Test">
            <rows>
                <add text="SomethingWonderful" />
            </rows>
            <fields>
                <add name="text" />
            </fields>
            <calculated-fields>
                <add name="humanized" length="4000" t="copy(text).humanize()" />
            </calculated-fields>
        </add>
    </entities>
</cfg>
```

This would produce `Something Wonderful` for humans.