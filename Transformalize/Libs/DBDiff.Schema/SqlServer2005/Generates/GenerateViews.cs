using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Libs.DBDiff.Schema.Errors;
using Transformalize.Libs.DBDiff.Schema.Events;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.Util;
using Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateViews
    {
        private Generate root;

        public GenerateViews(Generate root)
        {
            this.root = root;
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            try
            {
                root.RaiseOnReading(new ProgressEventArgs("Reading views...", Constants.READING_VIEWS));
                if (database.Options.Ignore.FilterView)
                {
                    FillView(database,connectionString);
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message, ex.StackTrace, MessageLog.LogType.Error));
            }
        }

        private void FillView(Database database, string connectionString)
        {
            int lastViewId = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(ViewSQLCommand.GetView(database.Info.Version), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        View item = null;                        
                        while (reader.Read())
                        {
                            root.RaiseOnReadingOne(reader["name"]);
                            if (lastViewId != (int)reader["object_id"]) {
                                item = new View(database) {
                                    Id = (int) reader["object_id"],
                                    Name = reader["name"].ToString(),
                                    Owner = reader["owner"].ToString(),
                                    IsSchemaBinding = reader["IsSchemaBound"].ToString().Equals("1"),
                                    CreateDate = (DateTime) reader["create_date"],
                                    ModifyDate = (DateTime) reader["modify_date"]
                                };
                                database.Views.Add(item);
                                lastViewId = item.Id;
                            }
                            if (item.IsSchemaBinding)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("referenced_major_id")))
                                    database.Dependencies.Add(database,(int)reader["referenced_major_id"], item);
                                if (!String.IsNullOrEmpty(reader["TableName"].ToString()))
                                    item.DependenciesIn.Add(reader["TableName"].ToString());
                                if (!String.IsNullOrEmpty(reader["DependOut"].ToString()))
                                    item.DependenciesOut.Add(reader["DependOut"].ToString());
                            }                            
                        }
                    }
                }                    
            }
        }
    }
}
