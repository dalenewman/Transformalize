cd c:\code\Transformalize\Pipeline.Portable
nuget pack Transformalize.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Desktop
nuget pack Transformalize.Desktop.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Logging.NLog
nuget pack Transformalize.Logging.NLog.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Ado
nuget pack Transformalize.Provider.Ado.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Elastic
nuget pack Transformalize.Provider.Elastic.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Excel
nuget pack Transformalize.Provider.Excel.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.File
nuget pack Transformalize.Provider.File.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Lucene
nuget pack Transformalize.Provider.Lucene.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.MySql
nuget pack Transformalize.Provider.MySql.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.PostgreSql
nuget pack Transformalize.Provider.PostgreSql.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Solr
nuget pack Transformalize.Provider.Solr.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.SQLite
nuget pack Transformalize.Provider.SQLite.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.SqlServer
nuget pack Transformalize.Provider.SqlServer.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Web
nuget pack Transformalize.Provider.Web.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Scheduler.Quartz
nuget pack Transformalize.Scheduler.Quartz.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Transform.Humanizer
nuget pack Transformalize.Transform.Humanizer.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Transform.Jint
nuget pack Transformalize.Transform.Jint.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Transform.JavaScriptEngineSwitcher
nuget pack Transformalize.Transform.JavaScriptEngineSwitcher.nuspec -OutputDirectory "c:\temp\modules"