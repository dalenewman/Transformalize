cd c:\code\Transformalize\Pipeline.Portable
nuget pack Pipeline.Portable.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Desktop
nuget pack Pipeline.Desktop.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Logging.NLog
nuget pack Pipeline.Logging.NLog.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Ado
nuget pack Pipeline.Provider.Ado.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Elastic
nuget pack Pipeline.Provider.Elastic.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Excel
nuget pack Pipeline.Provider.Excel.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.File
nuget pack Pipeline.Provider.File.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Lucene
nuget pack Pipeline.Provider.Lucene.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.MySql
nuget pack Pipeline.Provider.MySql.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.PostgreSql
nuget pack Pipeline.Provider.PostgreSql.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Solr
nuget pack Pipeline.Provider.Solr.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.SQLite
nuget pack Pipeline.Provider.SQLite.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.SqlServer
nuget pack Pipeline.Provider.SqlServer.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Provider.Web
nuget pack Pipeline.Provider.Web.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Scheduler.Quartz
nuget pack Pipeline.Scheduler.Quartz.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Transform.Humanizer
nuget pack Pipeline.Transform.Humanizer.nuspec -OutputDirectory "c:\temp\modules"
cd ..
cd Pipeline.Transform.Jint
nuget pack Pipeline.Transform.Jint.nuspec -OutputDirectory "c:\temp\modules"