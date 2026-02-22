This is a .NET Standard mail (output) provider for [Transformalize](https://github.com/dalenewman/Transformalize). It uses [MailKit](https://github.com/jstedfast/MailKit).
 
```xml
<cfg name="Mail">

    <connections>
        <add name="input" provider="internal" />
        <add name="output" provider="mail" server="smtp.gmail.com" port="465" useSsl='true' user='*' password='*' />
    </connections>

    <entities>
        <add name="Messages" >
            <rows>
                <add From="dale@dale.com" To="dale@dale.com" Cc="" Bcc="" Subject="Test" Body="I am a test message." />
            </rows>
            <fields>
                <add name="From" />
                <add name="To" />
                <add name="Cc" />
                <add name="Bcc" />
                <add name="Subject" />
                <add name="Body" />
            </fields>
        </add>
    </entities>

</cfg>
```

Saving this as *email.xml* and running should send the message.  When using a mail 
output, your entity must have `from`, `to`, and `body` fields.

This has been tested using

- a local mail relay server
- [mailtrap.io](https://mailtrap.io)
- sendgrid with `port="587" startTls="true" user="apikey" password="the actual key"`.
- gmail with `port="465" useSsl="true"` and Google settings *allow less secure apps* on.
