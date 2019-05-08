nuget pack Transformalize.nuspec -OutputDirectory "c:\temp\modules"
REM 
REM nuget pack Transformalize.Logging.NLog.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Scheduler.Quartz.nuspec -OutputDirectory "c:\temp\modules"
REM 
REM rem PROVIDERS
REM nuget pack Transformalize.Provider.OpenXml.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.File.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.GeoJson.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.Kml.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.Console.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.Trace.nuspec -OutputDirectory "c:\temp\modules"
REM 
REM REM TRANSFORMS
nuget pack Transformalize.Transform.Globalization.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Compression.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Geography.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Html.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Json.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Xml.nuspec -OutputDirectory "c:\temp\modules"


