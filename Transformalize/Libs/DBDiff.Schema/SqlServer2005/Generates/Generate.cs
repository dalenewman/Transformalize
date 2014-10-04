using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Libs.DBDiff.Schema.Errors;
using Transformalize.Libs.DBDiff.Schema.Events;
using Transformalize.Libs.DBDiff.Schema.Misc;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Compare;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.Util;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Options;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class Generate
    {
        private readonly List<MessageLog> messages;
        private string connectionString;
        private ProgressEventArgs currentlyReading;
        private SqlOption _options;

        public Generate()
        {
            messages = new List<MessageLog>();
            OnReading += Generate_OnReading;
        }

        public static int MaxValue
        {
            get { return Constants.READING_MAX; }
        }

        public string ConnectionString
        {
            set { connectionString = value; }
        }

        private string Name
        {
            get
            {
                string name;
                using (var conn = new SqlConnection(connectionString))
                {
                    name = conn.Database;
                }
                return name;
            }
        }

        public SqlOption Options
        {
            get {
                return _options ?? (_options = new SqlOption() {
                    Ignore = {
                        FilterConstraintCheck = true,
                        FilterConstraintPK = true,
                        FilterConstraint = true,
                        FilterTrigger = true,
                        FilterConstraintFK = true,
                        FilterFullText = true,
                        FilterIndex = true,
                        FilterFunction = true,
                        FilterStoreProcedure = true,
                        FilterSynonyms = true,
                        FilterTable = true,
                        FilterConstraintUK = true,
                        FilterView = true,
                        FilterIndexFillFactor = false,
                        FilterAssemblies = false,
                        FilterCLRFunction = false,
                        FilterCLRStoreProcedure = false,
                        FilterCLRTrigger = false,
                        FilterCLRUDT = false,
                        FilterDDLTriggers = false,
                        FilterPartitionFunction = false,
                        FilterPartitionScheme = false,
                        FilterUsers = false,
                        FilterRoles = false,
                        FilterXMLSchema = false,
                        FilterColumnCollation = false,
                        FilterFullTextPath = false,
                        FilterRules = false,
                        FilterTableChangeTracking = false
                    }
                });
            }
            set { _options = value; }
        }

        private event ProgressEventHandler.ProgressHandler OnReading;
        public event ProgressEventHandler.ProgressHandler OnProgress;

        private void Generate_OnReading(ProgressEventArgs e)
        {
            if (OnProgress != null) OnProgress(e);
        }

        public void RaiseOnReading(ProgressEventArgs e)
        {
            this.currentlyReading = e;
            if (OnReading != null) OnReading(e);
        }

        public void RaiseOnReadingOne(object name)
        {
            if (name != null && this.OnReading != null && this.currentlyReading != null)
            {
                var eOne = new ProgressEventArgs(this.currentlyReading.Message, this.currentlyReading.Progress);
                eOne.Message = eOne.Message.Replace("...", String.Format(": [{0}]", name));
                this.OnReading(eOne);
            }
        }

        /// <summary>
        /// Genera el schema de la base de datos seleccionada y devuelve un objeto Database.
        /// </summary>
        public Database Process()
        {
            var error = string.Empty;
            var databaseSchema = new Database {Options = Options, Name = Name};

            databaseSchema.Info = (new GenerateDatabase(connectionString, Options)).Get(databaseSchema);
            (new GenerateRules(this)).Fill(databaseSchema, connectionString);
            (new GenerateTables(this)).Fill(databaseSchema, connectionString, messages);
            (new GenerateViews(this)).Fill(databaseSchema, connectionString, messages);
            (new GenerateIndex(this)).Fill(databaseSchema, connectionString);
            (new GenerateFullTextIndex(this)).Fill(databaseSchema, connectionString);
            (new GenerateUserDataTypes(this)).Fill(databaseSchema, connectionString, messages);
            (new GenerateXMLSchemas(this)).Fill(databaseSchema, connectionString);
            (new GenerateSchemas(this)).Fill(databaseSchema, connectionString);

            //not supported in azure yet
            if (databaseSchema.Info.Version != DatabaseInfo.VersionTypeEnum.SQLServerAzure10) {
                (new GeneratePartitionFunctions(this)).Fill(databaseSchema, connectionString);
                (new GeneratePartitionScheme(this)).Fill(databaseSchema, connectionString);
                (new GenerateFileGroups(this)).Fill(databaseSchema, connectionString);
            }
            
            (new GenerateDDLTriggers(this)).Fill(databaseSchema, connectionString);
            (new GenerateSynonyms(this)).Fill(databaseSchema, connectionString);
            
            //not supported in azure yet
            if (databaseSchema.Info.Version != DatabaseInfo.VersionTypeEnum.SQLServerAzure10)
            {
                (new GenerateAssemblies(this)).Fill(databaseSchema, connectionString);
                (new GenerateFullText(this)).Fill(databaseSchema, connectionString);
            }

            (new GenerateStoreProcedures(this)).Fill(databaseSchema, connectionString);
            (new GenerateFunctions(this)).Fill(databaseSchema, connectionString);
            (new GenerateTriggers(this)).Fill(databaseSchema, connectionString, messages);
            (new GenerateTextObjects(this)).Fill(databaseSchema, connectionString);
            (new GenerateUsers(this)).Fill(databaseSchema, connectionString);

            if (String.IsNullOrEmpty(error))
            {
                /*Las propiedades extendidas deben ir despues de haber capturado el resto de los objetos de la base*/
                (new GenerateExtendedProperties(this)).Fill(databaseSchema, connectionString, messages);
                databaseSchema.BuildDependency();
                return databaseSchema;
            }
            else
                throw new SchemaException(error);
        }

        private void tables_OnTableProgress(object sender, ProgressEventArgs e)
        {
            ProgressEventHandler.RaiseOnChange(e);
        }

        // TODO: Static because Compare method is static; static events are not my favorite
        public static event ProgressEventHandler.ProgressHandler OnCompareProgress;

        internal static void RaiseOnCompareProgress(string formatString, params object[] formatParams)
        {
            if (Generate.OnCompareProgress != null)
            {
                Generate.OnCompareProgress(new ProgressEventArgs(String.Format(formatString, formatParams), -1));
            }
        }
        
        public static Database Compare(Database databaseOriginalSchema, Database databaseCompareSchema)
        {
            Database merge = CompareDatabase.GenerateDiferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}