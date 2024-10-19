REM Main
nuget push "c:\temp\modules\Transformalize.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Containers 
nuget push "c:\temp\modules\Transformalize.Container.Autofac.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Providers
nuget push "c:\temp\modules\Transformalize.Provider.Console.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.Console.Autofac.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.File.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.File.Autofac.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.Trace.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.OpenXml.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.Kml.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Transforms
nuget push "c:\temp\modules\Transformalize.Transform.Compression.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Transform.Globalization.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Transform.Geography.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Transform.Xml.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Misc
nuget push "c:\temp\modules\Transformalize.Logging.NLog.0.11.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
