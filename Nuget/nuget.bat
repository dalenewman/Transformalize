nuget pack Transformalize.nuspec -OutputDirectory "c:\temp\modules"

REM CONTAINERS
nuget pack Transformalize.Container.Autofac.nuspec -OutputDirectory "c:\temp\modules"

REM REM MISC
nuget pack Transformalize.Logging.NLog.nuspec -OutputDirectory "c:\temp\modules"

REM PROVIDERS
nuget pack Transformalize.Provider.OpenXml.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Kml.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.File.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.File.Autofac.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Console.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Console.Autofac.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Trace.nuspec -OutputDirectory "c:\temp\modules"

REM TRANSFORMS
nuget pack Transformalize.Transform.Globalization.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Compression.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Geography.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Xml.nuspec -OutputDirectory "c:\temp\modules"
