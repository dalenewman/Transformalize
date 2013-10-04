#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.RazorEngine;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main {
    public class Process {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        public Fields CalculatedFields = new Fields();
        public Dictionary<string, AbstractConnection> Connections = new Dictionary<string, AbstractConnection>();
        public Entities Entities = new Entities();
        public IKernel Kernal = new StandardKernel();
        public Dictionary<string, Map> MapEndsWith = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapEquals = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapStartsWith = new Dictionary<string, Map>();
        public Entity MasterEntity;
        public string Name;
        public Options Options = new Options();
        public Dictionary<string, string> Providers = new Dictionary<string, string>();
        public List<Relationship> Relationships = new List<Relationship>();
        public Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public Dictionary<string, SearchType> SearchTypes = new Dictionary<string, SearchType>();
        public Encoding TemplateContentType = Encoding.Raw;
        public Dictionary<string, Template> Templates = new Dictionary<string, Template>();
        public AbstractConnection OutputConnection;
        public string Star;

        public Process()
            : this("TEST") {
        }

        public Process(string name) {
            Name = name;
            GlobalDiagnosticsContext.Set("process", name);
            
            // MySql
            Kernal.Bind<AbstractProvider>().To<MySqlProvider>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IConnectionChecker>().To<DefaultConnectionChecker>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IScriptRunner>().To<DefaultScriptRunner>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IEntityRecordsExist>().To<MySqlEntityRecordsExist>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IEntityDropper>().To<MySqlEntityDropper>().WhenInjectedInto<MySqlConnection>();
            Kernal.Bind<IEntityExists>().To<MySqlEntityExists>().WhenInjectedInto<MySqlEntityDropper>();

            //SqlServer
            Kernal.Bind<AbstractProvider>().To<SqlServerProvider>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IConnectionChecker>().To<DefaultConnectionChecker>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IScriptRunner>().To<DefaultScriptRunner>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IProviderSupportsModifier>().To<SqlServerProviderSupportsModifier>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IEntityRecordsExist>().To<SqlServerEntityRecordsExist>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IEntityDropper>().To<SqlServerEntityDropper>().WhenInjectedInto<SqlServerConnection>();
            Kernal.Bind<IEntityExists>().To<SqlServerEntityExists>().WhenInjectedInto<SqlServerEntityDropper>();

            //Analysis Services
            Kernal.Bind<AbstractProvider>().To<AnalysisServicesProvider>().WhenInjectedInto<AnalysisServicesConnection>();
            Kernal.Bind<IConnectionChecker>().To<AnalysisServicesConnectionChecker>().WhenInjectedInto<AnalysisServicesConnection>();
            Kernal.Bind<IScriptRunner>().To<AnalysisServicesScriptRunner>().WhenInjectedInto<AnalysisServicesConnection>();
            Kernal.Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<AnalysisServicesConnection>();
            Kernal.Bind<IEntityRecordsExist>().To<FalseEntityRecordsExist>().WhenInjectedInto<AnalysisServicesConnection>();
            Kernal.Bind<IEntityDropper>().To<FalseEntityDropper>().WhenInjectedInto<AnalysisServicesConnection>();

            //File
            Kernal.Bind<AbstractProvider>().To<FileProvider>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IConnectionChecker>().To<FileConnectionChecker>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IScriptRunner>().To<EmptyScriptRunner>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IEntityRecordsExist>().To<FileEntityRecordsExist>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IEntityDropper>().To<FileEntityDropper>().WhenInjectedInto<FileConnection>();
            Kernal.Bind<IEntityExists>().To<FileEntityExists>().WhenInjectedInto<FileEntityDropper>();
        }

        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }

        public Fields OutputFields() {
            var fields = new Fields();
            foreach (var entity in Entities) {
                fields.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields, CalculatedFields).Output().ToArray());
            }
            return fields;
        }

        public IEnumerable<Field> SearchFields() {
            return new StarFields(this).Fields().Where(f => !f.SearchTypes.Any(st => st.Name.Equals("none")));
        }

        public IParameters Parameters() {
            var parameters = new Parameters();

            foreach (var calculatedField in CalculatedFields) {
                if (calculatedField.Value.HasTransforms) {
                    foreach (AbstractTransform transform in calculatedField.Value.Transforms) {
                        if (transform.HasParameters) {
                            foreach (var parameter in transform.Parameters) {
                                parameters[parameter.Key] = parameter.Value;
                            }
                        }
                    }
                }
            }
            return parameters;
        }

        public Entity this[string entity] {
            get {
                return Entities.Find(e => e.Alias.Equals(entity, IC) || e.Name.Equals(entity, IC));
            }
        }

        public void Drop(Entity entity) {
            OutputConnection.Drop(entity);
        }

        public bool OutputRecordsExist(string schema, string outputName) {
            return OutputConnection.RecordsExist(schema, outputName);
        }

        public int GetNextBatchId() {
            return OutputConnection.NextBatchId(Name);
        }
    }
}