REM Main
nuget push "c:\temp\modules\Transformalize.0.11.8-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Dependency Injection 
nuget push "c:\temp\modules\Transformalize.Container.Autofac.0.11.8-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Providers
REM nuget push "c:\temp\modules\Transformalize.Provider.Ado.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.Ado.Autofac.0.11.3-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.Console.0.11.8-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.Console.Autofac.0.11.8-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.File.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.File.Autofac.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.Trace.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.OpenXml.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.Kml.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.SqlServer.0.11.2-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.SqlServer.Autofac.0.11.2-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Transforms
REM nuget push "c:\temp\modules\Transformalize.Transform.Ado.0.11.3-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Ado.Autofac.0.11.3-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Compression.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Globalization.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Geography.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Xml.0.11.4-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json

REM Misc
nuget push "c:\temp\modules\Transformalize.Logging.NLog.0.11.7-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
